using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using Newtonsoft.Json;
using Tc.Crm.Common.IntegrationLayer.Jti.Models;
using Tc.Crm.Common.IntegrationLayer.Jti.Service;
using Tc.Crm.Common.Models;
using Tc.Crm.Common.Services;
using EntityModel = Tc.Crm.Common.IntegrationLayer.Model.EntityModel;
using Tc.Crm.Common.IntegrationLayer.Model;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Tc.Crm.Common.IntegrationLayer.Helper;

namespace Tc.Crm.Common.IntegrationLayer.Service.Synchronisation.Outbound
{
    public class OutboundSynchronisationService : IOutboundSynchronisationService
    {
        private const string EntityCacheMessageName = "RecordId: {0}, EntityCacheId: {1}";

        private bool disposed;
        private readonly IOutboundSynchronisationDataService outboundSynchronisationDataService;
        private readonly IOutboundSyncConfigurationService configurationService;
        private readonly ILogger logger;
        private readonly IJwtService jwtService;
        private readonly IRequestPayloadCreator createRequestPayloadCreator;
        private readonly IRequestPayloadCreator updateRequestPayloadCreator;
		private readonly IEntityModelDeserializer entityModelDeserializer;

		private int MaxRetries { get; }
		private int[] RetrySchedule { get; }

		public OutboundSynchronisationService(ILogger logger,
            IOutboundSynchronisationDataService outboundSynchronisationService,
            IJwtService jwtService,
            IRequestPayloadCreator createRequestPayloadCreator,
            IRequestPayloadCreator updateRequestPayloadCreator,
            IOutboundSyncConfigurationService configurationService,
			IEntityModelDeserializer entityModelDeserializer)
        {
            this.outboundSynchronisationDataService = outboundSynchronisationService;
            this.logger = logger;
            this.jwtService = jwtService;
            this.configurationService = configurationService;
            this.createRequestPayloadCreator = createRequestPayloadCreator;
            this.updateRequestPayloadCreator = updateRequestPayloadCreator;
			this.entityModelDeserializer = entityModelDeserializer;

			RetrySchedule = outboundSynchronisationDataService.GetRetries();
			MaxRetries = RetrySchedule.Length;
		}

        public void ProcessEntityCacheOperation(Operation operation)
		{
			// base initialize
			var entityName = configurationService.EntityName;
			var batchSize = configurationService.BatchSize;
			var serviceUrl = operation == Operation.Create ? configurationService.CreateServiceUrl : configurationService.UpdateServiceUrl;
			HttpMethod httpMethod = operation == Operation.Create ? HttpMethod.Post : HttpMethod.Patch;
			IRequestPayloadCreator payloadCreator = operation == Operation.Create ? createRequestPayloadCreator : updateRequestPayloadCreator;

			logger.LogInformation($"Processing {Enum.GetName(typeof(EntityCacheMessageStatusReason), operation)} EntityCache for entity: {entityName}");
			logger.LogInformation($"Integration layer endpoint: {serviceUrl}");
            logger.LogInformation($"Retrieving entity cache records to process with maximum batch size: {batchSize}");
			// retrieve records
			var entityCacheCollection = operation == Operation.Create ?
				outboundSynchronisationDataService.GetCreatedEntityCacheToProcess(entityName, batchSize) : outboundSynchronisationDataService.GetUpdatedEntityCacheToProcess(entityName, batchSize);
			logger.LogInformation($"Found {entityCacheCollection?.Count} records to be processed");
			if (entityCacheCollection == null || entityCacheCollection.Count == 0) return;
			// prepare jwt token
            var token = jwtService.CreateJwtToken(outboundSynchronisationDataService.GetSecretKey(), CreateTokenPayload());			
            foreach (EntityCache entityCache in entityCacheCollection)
            {
				// don't do call if succeeded max retries (ex if request from integration layer to CRM failed and it was max retried)
	            if (entityCache.StatusReason == EntityCacheStatusReason.InProgress && entityCache.RequestsCount > MaxRetries)
	            {
					logger.LogInformation($"EntityCache record: {entityCache.Name} reached maximum retries {MaxRetries} of calls to integration layer and will be failed");
					outboundSynchronisationDataService.UpdateEntityCacheStatus(entityCache.Id, Status.Inactive, EntityCacheStatusReason.Failed);
					continue;
	            }
				// create entity cache message
	            var entityCacheMessage = new EntityCacheMessage
                {
                    EntityCacheId = entityCache.Id,
                    Name = string.Format(EntityCacheMessageName, entityCache.RecordId, entityCache.Id)
                };
				var entityCacheMessageId = outboundSynchronisationDataService.CreateEntityCacheMessage(entityCacheMessage);
                logger.LogInformation($"Processing EntityCache/EntityCacheMessage : {entityCache.Name}/{entityCacheMessage.Name}");
				// update entity cache
				outboundSynchronisationDataService.UpdateEntityCacheStatus(entityCache.Id, Status.Active, EntityCacheStatusReason.InProgress);
				// calculate next retry time in case if failure
	            var eligibleRetryTime = GetEligibleRetryTime(RetrySchedule, entityCache.RequestsCount);
	            string note = null;
				var statusReason = EntityCacheMessageStatusReason.Failed;
				var success = false;
				try
                {
					var entityModel = entityModelDeserializer.Deserialize(entityCache.Data);
                    var requestPayload = payloadCreator.GetPayload(entityModel);					
					var url = operation == Operation.Update ? GetUpdateUrl(serviceUrl, entityCache.SourceSystemId) : serviceUrl;
					var response = jwtService.SendHttpRequest(httpMethod, url, token, requestPayload, entityCacheMessageId.ToString());

					success = IsResponseSuccessful(response.StatusCode);
					statusReason = success ? EntityCacheMessageStatusReason.SuccessfullySentToIL : EntityCacheMessageStatusReason.Failed;
					note = success ? null : AppendNote(entityCacheMessage.Notes, response.StatusCode, response.Content);
					logger.LogInformation($"Executed call to integration layer.  EntityCacheMessage Status Reason: {Enum.GetName(typeof(EntityCacheMessageStatusReason), statusReason)}");
                }
                catch (Exception e)
                {
					note = AppendNote(entityCacheMessage.Notes, HttpStatusCode.InternalServerError, "Internal server error.");
	                logger.LogError(e.ToString());
					logger.LogInformation($"Exception thrown while executing call to service layer. EntityCacheMessage Status Reason: {Enum.GetName(typeof(EntityCacheMessageStatusReason), statusReason)}");
                }
				finally
				{
					// do crash in case of connectivity problems to CRM
					outboundSynchronisationDataService.UpdateEntityCacheSendToIntegrationLayerStatus(entityCache.Id, success, success ? (DateTime?)null : eligibleRetryTime);
					outboundSynchronisationDataService.UpdateEntityCacheMessageStatus(entityCacheMessageId, Status.Inactive, statusReason, success ? null : note);
				}
            }
        }

		#region Private methods

		/// <summary>
		/// Get time for next retry
		/// </summary>
		/// <param name="retryMinutes"></param>
		/// <param name="retry"></param>
	    /// <returns></returns>
	    private DateTime GetEligibleRetryTime(int[] retryMinutes, int retry) => (retryMinutes.Length > 0 && retry < retryMinutes.Length) ? DateTime.UtcNow.AddMinutes(retryMinutes[retry]) : DateTime.UtcNow;

	    /// <summary>
		/// Get integration layer service url
		/// </summary>
		/// <param name="serviceUrl"></param>
		/// <param name="sourceSystemId"></param>
		/// <returns></returns>
		private static string GetUpdateUrl(string serviceUrl, string sourceSystemId) => $"{serviceUrl}{(serviceUrl.EndsWith("/") ? string.Empty : "/")}{sourceSystemId}";

		/// <summary>
		/// Check if service return successfull response 
		/// </summary>
		/// <param name="statusCode"></param>
		/// <returns></returns>
        private bool IsResponseSuccessful(HttpStatusCode statusCode) => (int)statusCode >= 200 && (int)statusCode <= 299;

        private string AppendNote(string notes, HttpStatusCode statusCode, string content)
        {
            var errorNote = $"Status code: {(int)statusCode}. Error message: {content}";
            if (string.IsNullOrEmpty(notes))
                notes = errorNote;
            else
                notes += Environment.NewLine + errorNote;

            return notes;
        }

        private OutboundJsonWebTokenPayload CreateTokenPayload() => new OutboundJsonWebTokenPayload
																	{
																		IssuedAtTime = jwtService.GetIssuedAtTime().ToString(CultureInfo.InvariantCulture),
																		Expiry = outboundSynchronisationDataService.GetExpiry(),
																		NotBefore = outboundSynchronisationDataService.GetNotBeforeTime(),
																		Issuer = "msd"
																	};

        #endregion Private methods

        #region Displosable members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                DisposeObject(outboundSynchronisationDataService);
                DisposeObject(logger);
                DisposeObject(configurationService);
            }

            disposed = true;
        }

        private void DisposeObject(object obj)
        {
            if (obj != null)
            {
                if (obj is IDisposable)
                    ((IDisposable)obj).Dispose();
                else
                    obj = null;
            }
        }

        #endregion
    }
}