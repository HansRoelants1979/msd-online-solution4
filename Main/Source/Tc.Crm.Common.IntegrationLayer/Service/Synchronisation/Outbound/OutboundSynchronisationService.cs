using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Tc.Crm.Common.Constants;
using Tc.Crm.Common.IntegrationLayer.Jti.Models;
using Tc.Crm.Common.IntegrationLayer.Jti.Service;
using Tc.Crm.Common.Models;
using Tc.Crm.Common.Services;
using EntityModel = Tc.Crm.Common.IntegrationLayer.Model.EntityModel;

namespace Tc.Crm.Common.IntegrationLayer.Service.Synchronisation.Outbound
{
    public class OutboundSynchronisationService : IOutboundSynchronisationService
    {
        private bool disposed;
        private readonly IOutboundSynchronisationDataService outboundSynchronisationDataService;
        private readonly IConfigurationService configurationService;
        private readonly ILogger logger;
        private readonly IJwtService jwtService;
        private readonly IRequestPayloadCreator requestPayloadCreator;


        public OutboundSynchronisationService(ILogger logger,
            IOutboundSynchronisationDataService outboundSynchronisationService,
            IJwtService jwtService,
            IRequestPayloadCreator requestPayloadCreator,
            IConfigurationService configurationService)
        {
            this.outboundSynchronisationDataService = outboundSynchronisationService;
            this.logger = logger;
            this.jwtService = jwtService;
            this.configurationService = configurationService;
            this.requestPayloadCreator = requestPayloadCreator;
        }

        public void Run()
        {
            ProcessEntityCache();
        }

        #region Private methods

        private void ProcessEntityCache()
        {
            List<EntityCache> entityCacheCollection = outboundSynchronisationDataService.GetEntityCacheToProcess(configurationService.OutboundSyncEntityName, configurationService.OutboundSyncBatchSize);
            if (entityCacheCollection == null) return;

            var token = jwtService.CreateJwtToken(outboundSynchronisationDataService.GetSecretKey(), CreateTokenPayload());
            var serviceUrl = outboundSynchronisationDataService.GetServiceUrl();

            foreach (EntityCache entityCache in entityCacheCollection)
            {
                var entityCacheMessage = new EntityCacheMessage();
                entityCacheMessage.Id = Guid.NewGuid();
                entityCacheMessage.EntityCacheId = entityCache.Id;
                entityCacheMessage.Name = entityCacheMessage.Id.ToString();

                var entityCacheMessageId = CreateEntityCacheMessage(entityCacheMessage);
                UpdateEntityCacheStatus(entityCache.Id, Status.Active, EntityCacheStatusReason.InProgress);

                try
                {
                    var entityModel = JsonConvert.DeserializeObject<EntityModel>(entityCache.Data);
                    var requestPayload = requestPayloadCreator.GetPayload(entityModel);
                    var response = jwtService.SendHttpRequest(HttpMethod.Post, serviceUrl, token, requestPayload);

                    if (IsResponseSuccessful(response.StatusCode))
                        UpdateEntityCacheMessageStatus(entityCacheMessageId, Status.Inactive, EntityCacheMessageStatusReason.SuccessfullySentToIL);
                    else
                        UpdateEntityCacheMessageStatus(entityCacheMessageId, Status.Inactive, EntityCacheMessageStatusReason.Failed);
                }
                catch (Exception e)
                {
                    logger.LogError(e.ToString());
                    UpdateEntityCacheMessageStatus(entityCacheMessageId, Status.Inactive, EntityCacheMessageStatusReason.Failed);
                }
            }
        }

        private bool IsResponseSuccessful(HttpStatusCode statusCode)
        {
            return (int) statusCode >= 200 && (int) statusCode <= 299;
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
            outboundSynchronisationDataService.UpdateEntityStatus(entityCacheId, EntityName.EntityCache, (int)status, (int)statusReason);
        }

        /// <summary>
        /// To update status of entitycachemessage
        /// </summary>
        /// <param name="entityCacheMessageId"></param>
        /// <param name="status"></param>
        /// <param name="statusReason"></param>
        private void UpdateEntityCacheMessageStatus(Guid entityCacheMessageId, Status status, EntityCacheMessageStatusReason statusReason)
        {
            outboundSynchronisationDataService.UpdateEntityStatus(entityCacheMessageId, EntityName.EntityCacheMessage, (int)status, (int)statusReason);
        }

        private OutboundJsonWebTokenPayload CreateTokenPayload()
        {
            var payload = new OutboundJsonWebTokenPayload
            {
                Expiry = outboundSynchronisationDataService.GetExpiry(),
                NotBefore = outboundSynchronisationDataService.GetNotBeforeTime()
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