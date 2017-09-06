using System;
using Microsoft.Xrm.Sdk;
using Tc.Crm.Common;
using Tc.Crm.Common.Services;
using Tc.Crm.Common.Constants;
using Attributes = Tc.Crm.Common.Constants.Attributes;
using System.Collections.Generic;
using Tc.Crm.OutboundSynchronisation.Customer.Model;

namespace Tc.Crm.OutboundSynchronisation.Customer.Services
{
    public class OutboundSynchronisationDataService : IOutboundSynchronisationDataService
    {
        private readonly ICrmService crmService;
        private readonly ILogger logger;

        private bool disposed;

        public OutboundSynchronisationDataService(ILogger logger, ICrmService crmService)
        {
            this.logger = logger;
            this.crmService = crmService;
        }

        public List<EntityCacheModel> GetEntityCacheToProcess(string type, int numberOfElements)
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
                           <condition attribute='statuscode' operator='eq' value='1' />
                           <condition attribute='tc_operation' operator='eq' value='950000000' />
                         </filter>
                       </filter>
                     </entity>
                   </fetch>";

            EntityCollection entityCacheCollection = crmService.RetrieveMultipleRecordsFetchXml(query, numberOfElements);
            return entityCacheCollection;
        }

        public List<EntityCacheModel> PrepareEntityCacheModel(EntityCollection entityCacheCollection)
        {
            var entityCacheModelList = new List<EntityCacheModel>();
            for(int i=0; i < entityCacheCollection.Entities.Count; i++)
            {
                var entityCache = entityCacheCollection.Entities[i];
                var entityCacheModel = new EntityCacheModel();
                if (GeneralMethods.IsAttributeExistAndNotNull(entityCache, EntityName.EntityCache))
                    entityCacheModel.Name = entityCache.Attributes[Attributes.EntityCache.Name].ToString();

                if (GeneralMethods.IsAttributeExistAndNotNull(entityCache, Attributes.EntityCache.SourceMarket))
                    entityCacheModel.SourceMarket = entityCache.Attributes[Attributes.EntityCache.SourceMarket].ToString();

                if (GeneralMethods.IsAttributeExistAndNotNull(entityCache, Attributes.EntityCache.Type))
                    entityCacheModel.Type = entityCache.Attributes[Attributes.EntityCache.Type].ToString();

                if (GeneralMethods.IsAttributeExistAndNotNull(entityCache, Attributes.EntityCache.RecordId))
                    entityCacheModel.RecordId = entityCache.Attributes[Attributes.EntityCache.RecordId].ToString();

                if (GeneralMethods.IsAttributeExistAndNotNull(entityCache, Attributes.EntityCache.Operation))
                    entityCacheModel.Operation = ((OptionSetValue)entityCache.Attributes[Attributes.EntityCache.Operation]).Value;

                entityCacheModelList.Add(entityCacheModel);

            }
            return entityCacheModelList;
        }

        public EntityCollection PrepareEntityCacheMessages(List<EntityCacheMessageModel> entityCacheMessageModelCollection)
        {
            if (entityCacheMessageModelCollection == null || entityCacheMessageModelCollection.Count == 0) return null;
            var entityCacheMessageCollection = new EntityCollection();
            for(int i=0; i< entityCacheMessageModelCollection.Count;i++)
            {
                var entityCacheMessage = PrepareEntityCacheMessage(entityCacheMessageModelCollection[i]);
                if (entityCacheMessage != null)
                    entityCacheMessageCollection.Entities.Add(entityCacheMessage);
            }
            return entityCacheMessageCollection;
        }

        public Guid CreateEntityCacheMessage(EntityCacheMessageModel entityCacheMessageModel)
        {
            var entityCacheMessage = PrepareEntityCacheMessage(entityCacheMessageModel);
            return CreateRecord(entityCacheMessage);
        }

        public Entity PrepareEntityCacheMessage(EntityCacheMessageModel entityCacheMessageModel)
        {
            if (entityCacheMessageModel == null) return null;

            var entityCacheMessage = new Entity(EntityName.EntityCacheMessage);

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