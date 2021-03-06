﻿using System;
using Tc.Crm.Common;
using Tc.Crm.Common.Constants;
using Tc.Crm.Common.Services;
using Tc.Crm.Common.Models;


namespace Tc.Crm.OutboundSynchronisation.Customer.Services
{
    public class OutboundSynchronisationService : IOutboundSynchronisationService
    {
        private bool disposed;
        private readonly IOutboundSynchronisationDataService outboundSynchronisationDataService;
        private readonly IConfigurationService configurationService;
        private readonly ILogger logger;

        public OutboundSynchronisationService(ILogger logger, IOutboundSynchronisationDataService outboundSynchronisationService, IConfigurationService configurationService)
        {
            this.outboundSynchronisationDataService = outboundSynchronisationService;
            this.logger = logger;
            this.configurationService = configurationService;
        }

        public string SecretKey { get; set; }

        public void Run()
        {
            ProcessEntityCache();
        }

        /// <summary>
        /// To process all entitycache records which are of type contact and of operation create
        /// </summary>
        public void ProcessEntityCache()
        {
            var entityCacheCollection = outboundSynchronisationDataService.GetEntityCacheToProcess(configurationService.OutboundSyncEntityName, configurationService.OutboundSyncBatchSize);
            if (entityCacheCollection == null) return;
            foreach(EntityCache entityCache in entityCacheCollection)
            {
                var entityCacheMessage = new EntityCacheMessage();
                entityCacheMessage.Id = Guid.NewGuid();
                entityCacheMessage.EntityCacheId = entityCache.Id;
                entityCacheMessage.Name = entityCacheMessage.Id.ToString();

                var entityCacheMessageId = CreateEntityCacheMessage(entityCacheMessage);
                UpdateEntityCacheStatus(entityCache.Id, Status.Active, EntityCacheStatusReason.InProgress);
                UpdateEntityCacheMessageStatus(entityCacheMessageId, Status.Inactive, EntityCacheMessageStatusReason.SuccessfullySenttoIL);
            }

        }

        /// <summary>
        /// To create entitycachemessage record
        /// </summary>
        /// <param name="entityCacheMessageModel"></param>
        /// <returns></returns>
        public Guid CreateEntityCacheMessage(EntityCacheMessage entityCacheMessageModel)
        {
            return outboundSynchronisationDataService.CreateEntityCacheMessage(entityCacheMessageModel);
        }


        /// <summary>
        /// To update status of entitycache
        /// </summary>
        /// <param name="entityCacheId"></param>
        /// <param name="status"></param>
        /// <param name="statusReason"></param>
        public void UpdateEntityCacheStatus(Guid entityCacheId, Status status, EntityCacheStatusReason statusReason)
        {
            outboundSynchronisationDataService.UpdateEntityStatus(entityCacheId, EntityName.EntityCache, (int)status, (int)statusReason);
        }
              
        /// <summary>
        /// To update status of entitycachemessage
        /// </summary>
        /// <param name="entityCacheMessageId"></param>
        /// <param name="status"></param>
        /// <param name="statusReason"></param>
        public void UpdateEntityCacheMessageStatus(Guid entityCacheMessageId, Status status, EntityCacheMessageStatusReason statusReason)
        {
            outboundSynchronisationDataService.UpdateEntityStatus(entityCacheMessageId, EntityName.EntityCacheMessage, (int)status, (int)statusReason);
        }

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