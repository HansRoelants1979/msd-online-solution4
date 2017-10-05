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

        public OutboundSynchronisationService(ILogger logger,
            IOutboundSynchronisationDataService outboundSynchronisationService,
            IJwtService jwtService,
            IRequestPayloadCreator createRequestPayloadCreator,
            IRequestPayloadCreator updateRequestPayloadCreator,
            IOutboundSyncConfigurationService configurationService)
        {
            this.outboundSynchronisationDataService = outboundSynchronisationService;
            this.logger = logger;
            this.jwtService = jwtService;
            this.configurationService = configurationService;
            this.createRequestPayloadCreator = createRequestPayloadCreator;
            this.updateRequestPayloadCreator = updateRequestPayloadCreator;
        }

        public void Run()
        {
            ProcessCreateEntityCache();
            ProcessUpdateEntityCache();
        }

        #region Private methods

        private void ProcessCreateEntityCache()
        {
            var entityName = configurationService.EntityName;
            logger.LogInformation($"Configuration create record: {entityName}");
            var batchSize = configurationService.BatchSize;
            logger.LogInformation($"The number of records: {batchSize}");

            var entityCacheCollection = outboundSynchronisationDataService.GetCreatedEntityCacheToProcess(entityName, batchSize);
            if (entityCacheCollection == null || entityCacheCollection.Count == 0) return;

            var token = jwtService.CreateJwtToken(outboundSynchronisationDataService.GetSecretKey(), CreateTokenPayload());
            var serviceUrl = configurationService.CreateServiceUrl;
            logger.LogInformation($"The create endpoint: {serviceUrl}");
            foreach (EntityCache entityCache in entityCacheCollection)
            {
                var entityCacheMessage = new EntityCacheMessage
                {
                    EntityCacheId = entityCache.Id,
                    Name = string.Format(EntityCacheMessageName, entityCache.RecordId, entityCache.Id.ToString())
                };

                var entityCacheMessageId = CreateEntityCacheMessage(entityCacheMessage);
                logger.LogInformation($"EntityCache/EntityCacheMessage : {entityCache.Name}/{entityCacheMessage.Name}");
                UpdateEntityCacheStatus(entityCache.Id, Status.Active, EntityCacheStatusReason.InProgress);

                try
                {
                    var entityModel = JsonConvert.DeserializeObject<EntityModel>(entityCache.Data);
                    var requestPayload = createRequestPayloadCreator.GetPayload(entityModel);
                    var response = jwtService.SendHttpRequest(HttpMethod.Post, serviceUrl, token, requestPayload, entityCacheMessageId.ToString());

                    if (IsResponseSuccessful(response.StatusCode))
                    {
                        UpdateEntityCacheMessageStatus(entityCacheMessageId, Status.Inactive, EntityCacheMessageStatusReason.SuccessfullySentToIL);
                        logger.LogInformation(
                            $"EntityCacheMessage Status Reason: {Enum.GetName(typeof(EntityCacheMessageStatusReason), EntityCacheMessageStatusReason.SuccessfullySentToIL)}");
                    }
                    else
                    {
                        var notes = AppendNote(entityCacheMessage.Notes, response.StatusCode, response.Content);
                        UpdateEntityCacheMessageStatus(entityCacheMessageId, Status.Inactive, EntityCacheMessageStatusReason.Failed, notes);
                        logger.LogInformation(
                            $"EntityCacheMessage Status Reason: {Enum.GetName(typeof(EntityCacheMessageStatusReason), EntityCacheMessageStatusReason.Failed)}");
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e.ToString());
                    var notes = AppendNote(entityCacheMessage.Notes, HttpStatusCode.InternalServerError, "Internal server error.");
                    UpdateEntityCacheMessageStatus(entityCacheMessageId, Status.Inactive, EntityCacheMessageStatusReason.Failed, notes);
                    logger.LogInformation(
                        $"EntityCacheMessage Status Reason: {Enum.GetName(typeof(EntityCacheMessageStatusReason), EntityCacheMessageStatusReason.Failed)}");
                }
            }
        }

        private void ProcessUpdateEntityCache()
        {
            var entityName = configurationService.EntityName;
            logger.LogInformation($"Configuration update record: {entityName}");
            var batchSize = configurationService.BatchSize;
            logger.LogInformation($"The number of records: {batchSize}");

            var entityCacheCollection = outboundSynchronisationDataService.GetUpdatedEntityCacheToProcess(entityName, batchSize);
            if (entityCacheCollection == null || entityCacheCollection.Count == 0) return;

            var token = jwtService.CreateJwtToken(outboundSynchronisationDataService.GetSecretKey(), CreateTokenPayload());
            var serviceUrl = configurationService.UpdateServiceUrl;
            logger.LogInformation($"The update endpoint: {serviceUrl}");
            var skippedrecords = new List<string>();
            foreach (EntityCache entityCache in entityCacheCollection)
            {
                if (skippedrecords.Contains(entityCache.RecordId))
                    continue;

                var entityCacheMessage = new EntityCacheMessage
                {
                    EntityCacheId = entityCache.Id,
                    Name = string.Format(EntityCacheMessageName, entityCache.RecordId, entityCache.Id.ToString())
                };

                var entityCacheMessageId = CreateEntityCacheMessage(entityCacheMessage);
                logger.LogInformation($"EntityCache/EntityCacheMessage : {entityCache.Name}/{entityCacheMessage.Name}");
                UpdateEntityCacheStatus(entityCache.Id, Status.Active, EntityCacheStatusReason.InProgress);
                logger.LogInformation($"EntityCache Status Reason: {Enum.GetName(typeof(EntityCacheStatusReason), EntityCacheStatusReason.InProgress)}");

                try
                {
                    var entityModel = JsonConvert.DeserializeObject<EntityModel>(entityCache.Data);
                    var requestPayload = updateRequestPayloadCreator.GetPayload(entityModel);

                    var url = CreateServiceUrl(serviceUrl, entityCache.SourceSystemId);
                    var response = jwtService.SendHttpRequest(HttpMethod.Patch, url, token, requestPayload, entityCacheMessageId.ToString());

                    if (IsResponseSuccessful(response.StatusCode))
                    {
                        UpdateEntityCacheMessageStatus(entityCacheMessageId, Status.Inactive, EntityCacheMessageStatusReason.SuccessfullySentToIL);
                        logger.LogInformation($"EntityCacheMessage Status Reason: {Enum.GetName(typeof(EntityCacheMessageStatusReason), EntityCacheMessageStatusReason.SuccessfullySentToIL)}");
                    }
                    else
                    {
                        var notes = AppendNote(entityCacheMessage.Notes, response.StatusCode, response.Content);
                        UpdateEntityCacheMessageStatus(entityCacheMessageId, Status.Inactive, EntityCacheMessageStatusReason.Failed, notes);
                        skippedrecords.Add(entityCache.RecordId);
                        logger.LogInformation($"EntityCacheMessage Status Reason: {Enum.GetName(typeof(EntityCacheMessageStatusReason), EntityCacheMessageStatusReason.Failed)}");
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e.ToString());
                    var notes = AppendNote(entityCacheMessage.Notes, HttpStatusCode.InternalServerError, "Internal server error.");
                    UpdateEntityCacheMessageStatus(entityCacheMessageId, Status.Inactive, EntityCacheMessageStatusReason.Failed, notes);
                    logger.LogInformation($"EntityCacheMessage Status Reason: {Enum.GetName(typeof(EntityCacheMessageStatusReason), EntityCacheMessageStatusReason.Failed)}");
                }
            }
        }

        private static string CreateServiceUrl(string serviceUrl, string sourceSystemId)
        {
            if (!serviceUrl.EndsWith("/"))
                return serviceUrl + "/" + sourceSystemId;

            return serviceUrl + sourceSystemId;
        }

        private bool IsResponseSuccessful(HttpStatusCode statusCode)
        {
            return (int) statusCode >= 200 && (int) statusCode <= 299;
        }

        private string AppendNote(string notes, HttpStatusCode statusCode, string content)
        {
            var errorNote = $"Status code: {(int)statusCode}. Error message: {content}";
            if (string.IsNullOrEmpty(notes))
                notes = errorNote;
            else
                notes += Environment.NewLine + errorNote;

            return notes;
        }

        /// <summary>
        /// To create entitycachemessage record
        /// </summary>
        /// <param name="entityCacheMessageModel"></param>
        /// <returns></returns>
        private Guid CreateEntityCacheMessage(EntityCacheMessage entityCacheMessageModel)
        {
            return outboundSynchronisationDataService.CreateEntityCacheMessage(entityCacheMessageModel);
        }

        /// <summary>
        /// To update status of entitycache
        /// </summary>
        /// <param name="entityCacheId"></param>
        /// <param name="status"></param>
        /// <param name="statusReason"></param>
        private void UpdateEntityCacheStatus(Guid entityCacheId, Status status, EntityCacheStatusReason statusReason)
        {
            outboundSynchronisationDataService.UpdateEntityCacheStatus(entityCacheId, (int)status, (int)statusReason);
        }

        /// <summary>
        /// To update status of entitycachemessage
        /// </summary>
        /// <param name="entityCacheMessageId"></param>
        /// <param name="status"></param>
        /// <param name="statusReason"></param>
        /// <param name="notes"></param>
        private void UpdateEntityCacheMessageStatus(Guid entityCacheMessageId, Status status, EntityCacheMessageStatusReason statusReason, string notes = null)
        {
            outboundSynchronisationDataService.UpdateEntityCacheMessageStatus(entityCacheMessageId, (int)status, (int)statusReason, notes);
        }

        private OutboundJsonWebTokenPayload CreateTokenPayload()
        {
            var payload = new OutboundJsonWebTokenPayload
            {
                IssuedAtTime = jwtService.GetIssuedAtTime().ToString(CultureInfo.InvariantCulture),
                Expiry = outboundSynchronisationDataService.GetExpiry(),
                NotBefore = outboundSynchronisationDataService.GetNotBeforeTime(),
				Issuer = "msd"
            };

            return payload;
        }

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