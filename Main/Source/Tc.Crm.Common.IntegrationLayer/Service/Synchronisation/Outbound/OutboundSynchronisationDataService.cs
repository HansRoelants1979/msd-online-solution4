using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Tc.Crm.Common.Constants;
using Tc.Crm.Common.Helper;
using Tc.Crm.Common.Services;
using Attributes = Tc.Crm.Common.Constants.Attributes;
using EntityRecords = Tc.Crm.Common.Constants.EntityRecords;
using EntityCache = Tc.Crm.Common.Models.EntityCache;
using EntityCacheMessage = Tc.Crm.Common.Models.EntityCacheMessage;

namespace Tc.Crm.Common.IntegrationLayer.Service.Synchronisation.Outbound
{
    public class OutboundSynchronisationDataService : IOutboundSynchronisationDataService
    {
        private const string ContactAlias = "cntct";
        private const string SourceSystemIdKey = ContactAlias + "." + Attributes.Contact.SourceSystemId;

        private readonly ICrmService crmService;
        private readonly ILogger logger;

        private bool disposed;

        private static readonly string ApplicationName = AppDomain.CurrentDomain.FriendlyName;

        public OutboundSynchronisationDataService(ILogger logger, ICrmService crmService)
        {
            this.logger = logger;
            this.crmService = crmService;
        }

        public List<EntityCache> GetCreatedEntityCacheToProcess(string type, int numberOfElements)
        {
            var entityCacheCollection = RetrieveCreatedEntityCaches(type, numberOfElements);
            return PrepareEntityCacheModel(entityCacheCollection);
        }

        public List<EntityCache> GetUpdatedEntityCacheToProcess(string type, int numberOfElements)
        {
            var entityCacheCollection = RetrieveUpdatedEntityCaches(type, numberOfElements);
            return PrepareEntityCacheModel(entityCacheCollection);
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
                    entityCacheModel.Operation = ((OptionSetValue)entityCache.Attributes[Attributes.EntityCache.Operation]).Value;

                if (EntityHelper.HasAttributeNotNull(entityCache, Attributes.EntityCache.Data))
                    entityCacheModel.Data = entityCache.Attributes[Attributes.EntityCache.Data].ToString();
                if (EntityHelper.HasAttributeNotNull(entityCache, SourceSystemIdKey))
                    entityCacheModel.SourceSystemId = ((AliasedValue) entityCache.Attributes[SourceSystemIdKey]).Value.ToString();

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

        public void UpdateEntityCacheStatus(Guid id, int stateCode, int statusCode)
        {
            var entity = new Entity(EntityName.EntityCache);
            entity.Id = id;
            entity.Attributes[Attributes.EntityCache.State] = new OptionSetValue(stateCode);
            entity.Attributes[Attributes.EntityCache.StatusReason] = new OptionSetValue(statusCode);
            UpdateRecord(entity);
        }

        public void UpdateEntityCacheMessageStatus(Guid id, int stateCode, int statusCode, string notes = null)
        {
            var entity = new Entity(EntityName.EntityCacheMessage);
            entity.Id = id;
            entity.Attributes[Attributes.EntityCacheMessage.State] = new OptionSetValue(stateCode);
            entity.Attributes[Attributes.EntityCacheMessage.StatusReason] = new OptionSetValue(statusCode);
            if (!string.IsNullOrEmpty(notes))
                entity.Attributes[Attributes.EntityCacheMessage.Notes] = notes;
            UpdateRecord(entity);
        }

        public string GetExpiry()
        {
            var expiry = GetConfig(EntityRecords.Configuration.OutboundSynchronisationSsoTokenExpired);
            return expiry;
        }

        public string GetNotBeforeTime()
        {
            var notBeforeTime = GetConfig(EntityRecords.Configuration.OutboundSynchronisationSsoTokenNotBefore);
            return notBeforeTime;
        }

        public string GetSecretKey()
        {
            var query = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                    <entity name='{EntityName.SecurityConfiguration}'>
                      <attribute name='{Attributes.SecurityConfiguration.Name}' />
                      <attribute name ='{Attributes.SecurityConfiguration.Value}' />
                      <filter type='and'>
                        <condition attribute='tc_name' operator='eq' value='{EntityRecords.Configuration.OutboundSynchronisationJwtPrivateKeyConfigName}' />
                      </filter>
                    </entity>
                  </fetch>";

            var config = ExecuteQuery(query);
            var privateKey = config?.GetAttributeValue<string>(Attributes.SecurityConfiguration.Value);
            if (privateKey != null)
                logger.LogInformation(
                    $"Retrieved {EntityRecords.Configuration.OutboundSynchronisationJwtPrivateKeyConfigName} result {EntityRecords.Configuration.OutboundSynchronisationJwtPrivateKeyConfigName} is not null");

            return privateKey;
        }

        public Guid CreateRecord(Entity entity)
        {
            return crmService.Create(entity);
        }

        public void UpdateRecord(Entity entity)
        {
            crmService.Update(entity);
        }

        #region Private Methods

        private EntityCollection RetrieveCreatedEntityCaches(string type, int numberOfElements)
        {
            if (string.IsNullOrEmpty(type))
                throw new ArgumentNullException(nameof(type), "Type parameter cannot be empty");

            var query = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                     <entity name='{EntityName.EntityCache}'>
                       <attribute name='{Attributes.EntityCache.EntityCacheId}' />
                       <attribute name='{Attributes.EntityCache.Name}' />
                       <attribute name='{Attributes.EntityCache.CreatedOn}' />
                       <attribute name='{Attributes.EntityCache.Type}' />
                       <attribute name='{Attributes.EntityCache.StatusReason}' />
                       <attribute name='{Attributes.EntityCache.State}' />
                       <attribute name='{Attributes.EntityCache.SourceMarket}' />
                       <attribute name='{Attributes.EntityCache.RecordId}' />
                       <attribute name='{Attributes.EntityCache.Operation}' />
                       <attribute name='{Attributes.EntityCache.Data}' />
                       <order attribute='{Attributes.EntityCache.CreatedOn}' descending='false' />
                       <filter type='and'>
                         <filter type='and'>
                           <condition attribute='{Attributes.EntityCache.Type}' operator='eq' value='{type}' />
                           <condition attribute='{Attributes.EntityCache.StatusReason}' operator='eq' value='{(int)EntityCacheStatusReason.Active}' />
                           <condition attribute='{Attributes.EntityCache.Operation}' operator='eq' value='{(int)EntityCacheOperation.Create}' />
                         </filter>
                       </filter>
                     </entity>
                   </fetch>";

            EntityCollection entityCacheCollection = crmService.RetrieveMultipleRecordsFetchXml(query, numberOfElements);
            return entityCacheCollection;
        }

        private EntityCollection RetrieveUpdatedEntityCaches(string type, int numberOfElements)
        {
            if (string.IsNullOrEmpty(type))
                throw new ArgumentNullException(nameof(type), "Type parameter cannot be empty");

            var query = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                     <entity name='{EntityName.EntityCache}'>
                       <attribute name='{Attributes.EntityCache.EntityCacheId}' />
                       <attribute name='{Attributes.EntityCache.Name}' />
                       <attribute name='{Attributes.EntityCache.CreatedOn}' />
                       <attribute name='{Attributes.EntityCache.Type}' />
                       <attribute name='{Attributes.EntityCache.StatusReason}' />
                       <attribute name='{Attributes.EntityCache.State}' />
                       <attribute name='{Attributes.EntityCache.SourceMarket}' />
                       <attribute name='{Attributes.EntityCache.RecordId}' />
                       <attribute name='{Attributes.EntityCache.Operation}' />
                       <attribute name='{Attributes.EntityCache.Data}' />
                       <order attribute='{Attributes.EntityCache.CreatedOn}' descending='false' />
                       <filter type='and'>
                         <condition attribute='{Attributes.EntityCache.Type}' operator='eq' value='{type}' />
                         <condition attribute='{Attributes.EntityCache.StatusReason}' operator='eq' value='{(int)EntityCacheStatusReason.Active}' />
                         <condition attribute='{Attributes.EntityCache.Operation}' operator='eq' value='{(int)EntityCacheOperation.Update}' />
                       </filter>
                       <link-entity name='{EntityName.EntityCacheMessage}' from='{Attributes.EntityCache.EntityCacheId}' to='{Attributes.EntityCache.EntityCacheId}' link-type='outer' alias='au'>
                         <filter type='and'>
                           <condition attribute='{Attributes.EntityCacheMessage.StatusReason}' operator='ne' value='{(int)EntityCacheMessageStatusReason.Failed}' />
                           <condition attribute='{Attributes.EntityCacheMessage.StatusReason}' operator='ne' value='{(int)EntityCacheMessageStatusReason.Active}' />
                         </filter>
                       </link-entity>
                       <link-entity name='{EntityName.Contact}' from='{Attributes.Contact.ContactId}' to='{Attributes.EntityCache.RecordId}' alias='{ContactAlias}'>
                         <attribute name='{Attributes.Contact.ContactId}' />
                         <attribute name='{Attributes.Contact.SourceSystemId}' />
                           <filter type='and'>
                             <condition attribute='{Attributes.Contact.SourceSystemId}' operator='not-null' />
                           </filter>
                       </link-entity>
                     </entity>
                   </fetch>";

            EntityCollection entityCacheCollection = crmService.RetrieveMultipleRecordsFetchXml(query, numberOfElements);
            return entityCacheCollection;
        }

        private string GetConfig(string name)
        {
            var query = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='{EntityName.Configuration}'>
                        <attribute name='{Attributes.Configuration.Name}' />
                        <attribute name='{Attributes.Configuration.Value}' />
                        <filter type='and'>
                          <condition attribute='{Attributes.Configuration.Name}' operator='eq' value='{name}' />
                        </filter>
                      </entity>
                    </fetch>";

            var config = ExecuteQuery(query);
            var value = config?.GetAttributeValue<string>(Attributes.Configuration.Value);
            logger.LogInformation($"Retrieved {name} result: {value}");
            return value;
        }

        private Entity ExecuteQuery(string query)
        {
            try
            {
                var response = crmService.RetrieveMultipleRecordsFetchXml(query);
                var record = response?.Entities?.FirstOrDefault();
                return record;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                logger.LogError($"{ApplicationName} application terminated with an error::{ex}");
            }
            catch (TimeoutException ex)
            {
                logger.LogError($"{ApplicationName} application terminated with an error::{ex}");
            }
            catch (Exception ex)
            {
                logger.LogError($"{ApplicationName} application terminated with an error::{ex}");
            }

            return null;
        }

        #endregion Private Methods

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