using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Tc.Crm.Common;
using Tc.Crm.Common.Models;
using Tc.Crm.Common.Helper;
using Tc.Crm.Common.Services;
using Tc.Crm.Common.Constants;
using Tc.Crm.Common.IntegrationLayer.Jti.Models;
using Attributes = Tc.Crm.Common.Constants.Attributes;
using Tc.Crm.Common.IntegrationLayer.Jti.Service;
using Tc.Crm.Common.IntegrationLayer.Model;

namespace Tc.Crm.OutboundSynchronisation.Customer.Services
{
    public class OutboundSynchronisationDataService : IOutboundSynchronisationDataService
    {
        private readonly ICrmService crmService;
        private readonly ILogger logger;
        private readonly IJwtService jwtService;

        private bool disposed;

        public OutboundSynchronisationDataService(ILogger logger, ICrmService crmService, IJwtService jwtService)
        {
            this.logger = logger;
            this.crmService = crmService;
            this.jwtService = jwtService;
        }

        public List<EntityCache> GetEntityCacheToProcess(string type, int numberOfElements)
        {
            var entityCacheCollection = RetrieveEntityCaches(type, numberOfElements);
            return PrepareEntityCacheModel(entityCacheCollection);
        }

        public EntityCollection RetrieveEntityCaches(string type, int numberOfElements)
        {
            if (string.IsNullOrEmpty(type))
                throw new ArgumentNullException(nameof(type), "Type parameter cannot be empty");

            var query = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                     <entity name='tc_entitycache'>
                       <attribute name='tc_entitycacheid' />
                       <attribute name='tc_name' />
                       <attribute name='createdon' />
                       <attribute name='tc_type' />
                       <attribute name='statuscode' />
                       <attribute name='statecode' />
                       <attribute name='tc_sourcemarket' />
                       <attribute name='tc_recordid' />
                       <attribute name='tc_operation' />
                       <attribute name='tc_data' />
                       <order attribute='createdon' descending='false' />
                       <filter type='and'>
                         <filter type='and'>
                           <condition attribute='tc_type' operator='eq' value='{type}' />
                           <condition attribute='statuscode' operator='eq' value='{(int)EntityCacheStatusReason.Active}' />
                           <condition attribute='tc_operation' operator='eq' value='{(int)EntityCacheOperation.Create}' />
                         </filter>
                       </filter>
                     </entity>
                   </fetch>";

            EntityCollection entityCacheCollection = crmService.RetrieveMultipleRecordsFetchXml(query, numberOfElements);
            return entityCacheCollection;
        }

        public List<EntityCache> PrepareEntityCacheModel(EntityCollection entityCacheCollection)
        {
            if (entityCacheCollection == null) return null;
            var entityCacheModelList = new List<EntityCache>();
            for (int i = 0; i < entityCacheCollection.Entities.Count; i++)
            {
                var entityCache = entityCacheCollection.Entities[i];
                var entityCacheModel = new EntityCache();

                entityCacheModel.Id = entityCache.Id;
                if (EntityHelper.HasAttributeNotNull(entityCache, Attributes.EntityCache.Name))
                    entityCacheModel.Name = entityCache.Attributes[Attributes.EntityCache.Name].ToString();

                if (EntityHelper.HasAttributeNotNull(entityCache, Attributes.EntityCache.SourceMarket))
                    entityCacheModel.SourceMarket = entityCache.Attributes[Attributes.EntityCache.SourceMarket].ToString();

                if (EntityHelper.HasAttributeNotNull(entityCache, Attributes.EntityCache.Type))
                    entityCacheModel.Type = entityCache.Attributes[Attributes.EntityCache.Type].ToString();

                if (EntityHelper.HasAttributeNotNull(entityCache, Attributes.EntityCache.RecordId))
                    entityCacheModel.RecordId = entityCache.Attributes[Attributes.EntityCache.RecordId].ToString();

                if (EntityHelper.HasAttributeNotNull(entityCache, Attributes.EntityCache.Operation))
                    entityCacheModel.Operation = ((int?)entityCache.Attributes[Attributes.EntityCache.Operation]).Value;

                entityCacheModelList.Add(entityCacheModel);

            }
            return entityCacheModelList;
        }

        public EntityCollection PrepareEntityCacheMessages(List<EntityCacheMessage> entityCacheMessageModelCollection)
        {
            if (entityCacheMessageModelCollection == null) return null;
            var entityCacheMessageCollection = new EntityCollection();
            for (int i = 0; i < entityCacheMessageModelCollection.Count; i++)
            {
                var entityCacheMessage = PrepareEntityCacheMessage(entityCacheMessageModelCollection[i]);
                if (entityCacheMessage != null)
                    entityCacheMessageCollection.Entities.Add(entityCacheMessage);
            }
            return entityCacheMessageCollection;
        }

        public Guid CreateEntityCacheMessage(EntityCacheMessage entityCacheMessageModel)
        {
            var entityCacheMessage = PrepareEntityCacheMessage(entityCacheMessageModel);
            return CreateRecord(entityCacheMessage);
        }

        public Entity PrepareEntityCacheMessage(EntityCacheMessage entityCacheMessageModel)
        {
            if (entityCacheMessageModel == null) return null;

            var entityCacheMessage = new Entity(EntityName.EntityCacheMessage);

            entityCacheMessage.Id = entityCacheMessageModel.Id;

            if (entityCacheMessageModel.Name != null)
                entityCacheMessage.Attributes[Attributes.EntityCacheMessage.Name] = entityCacheMessageModel.Name;

            if (entityCacheMessageModel.EntityCacheId != null)
                entityCacheMessage.Attributes[Attributes.EntityCacheMessage.EntityCacheId] = new EntityReference(EntityName.EntityCache, entityCacheMessageModel.EntityCacheId);

            if (entityCacheMessageModel.OutcomeId != null)
                entityCacheMessage.Attributes[Attributes.EntityCacheMessage.OutcomeId] = entityCacheMessageModel.OutcomeId;

            if (entityCacheMessageModel.Notes != null)
                entityCacheMessage.Attributes[Attributes.EntityCacheMessage.Notes] = entityCacheMessageModel.Notes;

            return entityCacheMessage;
        }

        public void UpdateEntityStatus(Guid id, string entityName, int StateCode, int StatusCode)
        {
            var entity = new Entity(entityName);
            entity.Id = id;
            entity.Attributes[Attributes.EntityCache.State] = new OptionSetValue(StateCode);
            entity.Attributes[Attributes.EntityCache.StatusReason] = new OptionSetValue(StatusCode);
            UpdateRecord(entity);
        }

        public string CreateJwtToken<T>(string privateKey, T payloadObj) where T : JsonWebTokenPayloadBase
        {
            if (string.IsNullOrEmpty(privateKey))
                throw new ArgumentNullException(nameof(privateKey));
            if(payloadObj == null)
                throw new ArgumentNullException(nameof(payloadObj));

            return jwtService.CreateJwtToken(privateKey, payloadObj);
        }

        public ResponseEntity SendHttpPostRequest(string serviceUrl, string token, string data)
        {
            if (string.IsNullOrEmpty(serviceUrl))
                throw new ArgumentNullException(nameof(serviceUrl));

            return jwtService.SendHttpRequest(HttpMethod.Post, serviceUrl, token, data);
        }

        public Guid CreateRecord(Entity entity)
        {
            return crmService.Create(entity);
        }

        public void UpdateRecord(Entity entity)
        {
            crmService.Update(entity);
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
                DisposeObject(logger);
                DisposeObject(crmService);
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