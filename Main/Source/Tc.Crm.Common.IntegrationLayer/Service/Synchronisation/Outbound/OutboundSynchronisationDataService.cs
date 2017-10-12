using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Tc.Crm.Common.Constants;
using Tc.Crm.Common.Helper;
using Tc.Crm.Common.Services;
using Attributes = Tc.Crm.Common.Constants.Attributes;
using EntityCache = Tc.Crm.Common.Models.EntityCache;
using EntityCacheMessage = Tc.Crm.Common.Models.EntityCacheMessage;
using EntityRecords = Tc.Crm.Common.Constants.EntityRecords;

namespace Tc.Crm.Common.IntegrationLayer.Service.Synchronisation.Outbound
{
    public class OutboundSynchronisationDataService : IOutboundSynchronisationDataService
    {
        private const string ContactAlias = "cntct";
	    private const string EntityCacheMessagAlias = "ecma";
		private const string EntityCacheMessagCountAlias = "EntityCacheMessageCount";
			
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

        public Guid CreateEntityCacheMessage(EntityCacheMessage entityCacheMessageModel)
        {
            var entityCacheMessage = PrepareEntityCacheMessage(entityCacheMessageModel);
            return CreateRecord(entityCacheMessage);
        }    

        public void UpdateEntityCacheStatus(Guid id, Enum stateCode, Enum statusCode)
        {
            var entity = new Entity(EntityName.EntityCache);
            entity.Id = id;
            entity.Attributes[Attributes.EntityCache.State] = new OptionSetValue(stateCode.GetHashCode());
            entity.Attributes[Attributes.EntityCache.StatusReason] = new OptionSetValue(statusCode.GetHashCode());
            UpdateRecord(entity);
        }

	    public void UpdateEntityCacheSendToIntegrationLayerStatus(Guid id, bool wasSuccess, DateTime? eligibleRetryTime = null)
	    {
			var entity = new Entity(EntityName.EntityCache);
		    entity.Id = id;
		    entity.Attributes[Attributes.EntityCache.WasLastOperationSuccessful] = wasSuccess;
		    if (!wasSuccess)
		    {
			    entity.Attributes[Attributes.EntityCache.EligibleRetryTime] = eligibleRetryTime;
		    }
		    UpdateRecord(entity);
		}

		public void UpdateEntityCacheMessageStatus(Guid id, Enum stateCode, Enum statusCode, string notes = null)
        {
            var entity = new Entity(EntityName.EntityCacheMessage);
            entity.Id = id;
            entity.Attributes[Attributes.EntityCacheMessage.State] = new OptionSetValue(stateCode.GetHashCode());
            entity.Attributes[Attributes.EntityCacheMessage.StatusReason] = new OptionSetValue(statusCode.GetHashCode());
            if (!string.IsNullOrEmpty(notes))
                entity.Attributes[Attributes.EntityCacheMessage.Notes] = notes;
            UpdateRecord(entity);
        }

        public string GetExpiry()
        {
            var expiry = GetConfig(EntityRecords.Configuration.OutboundSynchronisationSsoTokenExpired);
            return expiry;
        }

	    public int[] GetRetries()
	    {
		    var notBeforeTime = GetConfig(EntityRecords.Configuration.OutboundSynchronisationMaxRetries);
		    var numbers = notBeforeTime.Split(',').Select(int.Parse).ToArray();
		    return numbers;
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

		#region Private Methods

		private List<EntityCache> PrepareEntityCacheModel(EntityCollection entityCacheCollection)
		{
			if (entityCacheCollection == null) return null;
			var entityCacheModelList = new List<EntityCache>();
			for (int i = 0; i < entityCacheCollection.Entities.Count; i++)
			{
				var entityCache = entityCacheCollection.Entities[i];
				var entityCacheModel = new EntityCache();

				entityCacheModel.Id = entityCache.Id;
				if (EntityHelper.HasAttributeNotNull(entityCache, Attributes.EntityCache.Name))
					entityCacheModel.Name = ((AliasedValue)entityCache.Attributes[Attributes.EntityCache.Name]).Value.ToString();

				if (EntityHelper.HasAttributeNotNull(entityCache, Attributes.EntityCache.SourceMarket))
					entityCacheModel.SourceMarket = ((AliasedValue)entityCache.Attributes[Attributes.EntityCache.SourceMarket]).Value.ToString();

				if (EntityHelper.HasAttributeNotNull(entityCache, Attributes.EntityCache.Type))
					entityCacheModel.Type = ((AliasedValue)entityCache.Attributes[Attributes.EntityCache.Type]).Value.ToString();

				if (EntityHelper.HasAttributeNotNull(entityCache, Attributes.EntityCache.RecordId))
					entityCacheModel.RecordId = ((AliasedValue)entityCache.Attributes[Attributes.EntityCache.RecordId]).Value.ToString();

				if (EntityHelper.HasAttributeNotNull(entityCache, Attributes.EntityCache.Operation))
					entityCacheModel.Operation = (EntityCacheOperation)((OptionSetValue)((AliasedValue)entityCache.Attributes[Attributes.EntityCache.Operation]).Value).Value;

				if (EntityHelper.HasAttributeNotNull(entityCache, Attributes.EntityCache.Data))
					entityCacheModel.Data = ((AliasedValue)entityCache.Attributes[Attributes.EntityCache.Data]).Value.ToString();

				if (EntityHelper.HasAttributeNotNull(entityCache, Attributes.Contact.SourceSystemId))
					entityCacheModel.SourceSystemId = ((AliasedValue)entityCache.Attributes[Attributes.Contact.SourceSystemId]).Value.ToString();

				if (EntityHelper.HasAttributeNotNull(entityCache, Attributes.EntityCache.StatusReason))
					entityCacheModel.StatusReason = (EntityCacheStatusReason)((OptionSetValue)((AliasedValue)entityCache.Attributes[Attributes.EntityCache.StatusReason]).Value).Value;

				if (EntityHelper.HasAttributeNotNull(entityCache, EntityCacheMessagCountAlias))
					entityCacheModel.RequestsCount = (int)((AliasedValue)entityCache.Attributes[EntityCacheMessagCountAlias]).Value;

				entityCacheModelList.Add(entityCacheModel);

			}
			return entityCacheModelList;
		}

		private Guid CreateRecord(Entity entity)
		{
			return crmService.Create(entity);
		}

		private void UpdateRecord(Entity entity)
		{
			crmService.Update(entity);
		}

		private Entity PrepareEntityCacheMessage(EntityCacheMessage entityCacheMessageModel)
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

		private EntityCollection RetrieveCreatedEntityCaches(string type, int numberOfElements)
        {
            if (string.IsNullOrEmpty(type))
                throw new ArgumentNullException(nameof(type), "Type parameter cannot be empty");

            var query = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' aggregate='true'>
								<entity name='{EntityName.EntityCache}'>
									<attribute name='{Attributes.EntityCache.EntityCacheId}' groupby='true' alias='{Attributes.EntityCache.EntityCacheId}' />
									<attribute name='{Attributes.EntityCache.Name}' groupby='true' alias='{Attributes.EntityCache.Name}' />
									<attribute name='{Attributes.EntityCache.CreatedOn}' groupby='true' dategrouping='day' alias='{Attributes.EntityCache.CreatedOn}' />
									<attribute name='{Attributes.EntityCache.Type}' groupby='true' alias='{Attributes.EntityCache.Type}' />
									<attribute name='{Attributes.EntityCache.StatusReason}' groupby='true' alias='{Attributes.EntityCache.StatusReason}' />
									<attribute name='{Attributes.EntityCache.State}' groupby='true' alias='{Attributes.EntityCache.State}' />
									<attribute name='{Attributes.EntityCache.SourceMarket}' groupby='true' alias='{Attributes.EntityCache.SourceMarket}' />
									<attribute name='{Attributes.EntityCache.RecordId}' groupby='true' alias='{Attributes.EntityCache.RecordId}' />
									<attribute name='{Attributes.EntityCache.Operation}' groupby='true' alias='{Attributes.EntityCache.Operation}' />
									<attribute name='{Attributes.EntityCache.Data}' groupby='true' alias='{Attributes.EntityCache.Data}' />
									<link-entity name='{EntityName.EntityCacheMessage}' from='{Attributes.EntityCache.EntityCacheId}' to='{Attributes.EntityCache.EntityCacheId}' link-type='outer' alias='{EntityCacheMessagAlias}'> 
										<attribute name='{Attributes.EntityCacheMessage.EntityCacheMessageId}' aggregate='countcolumn' alias='{EntityCacheMessagCountAlias}' />
									</link-entity> 
									<order alias='{Attributes.EntityCache.CreatedOn}' descending='false' />
									<filter type='and'>
										<filter type='or'>
											<condition attribute='{Attributes.EntityCache.StatusReason}' operator='eq' value='{(int)EntityCacheStatusReason.Active}' />
											<filter type='and'>
												<condition attribute='{Attributes.EntityCache.StatusReason}' operator='eq' value='{(int)EntityCacheStatusReason.InProgress}' />
												<condition attribute='{Attributes.EntityCache.EligibleRetryTime}' operator='le' value='{DateTime.UtcNow.ToString("o")}' />
												<condition attribute='{Attributes.EntityCache.WasLastOperationSuccessful}' operator='eq' value='{false}' />
											</filter>
										</filter>
										<filter type='and'>
											<condition attribute='{Attributes.EntityCache.Type}' operator='eq' value='{type}' />
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

            var query = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' aggregate='true'>
								<entity name='{EntityName.EntityCache}'>
									<attribute name='{Attributes.EntityCache.EntityCacheId}' groupby='true' alias='{Attributes.EntityCache.EntityCacheId}' />
									<attribute name='{Attributes.EntityCache.Name}' groupby='true' alias='{Attributes.EntityCache.Name}' />
									<attribute name='{Attributes.EntityCache.CreatedOn}' groupby='true' dategrouping='day' alias='{Attributes.EntityCache.CreatedOn}' />
									<attribute name='{Attributes.EntityCache.Type}' groupby='true' alias='{Attributes.EntityCache.Type}' />
									<attribute name='{Attributes.EntityCache.StatusReason}' groupby='true' alias='{Attributes.EntityCache.StatusReason}' />
									<attribute name='{Attributes.EntityCache.State}' groupby='true' alias='{Attributes.EntityCache.State}' />
									<attribute name='{Attributes.EntityCache.SourceMarket}' groupby='true' alias='{Attributes.EntityCache.SourceMarket}' />
									<attribute name='{Attributes.EntityCache.RecordId}' groupby='true' alias='{Attributes.EntityCache.RecordId}' />
									<attribute name='{Attributes.EntityCache.Operation}' groupby='true' alias='{Attributes.EntityCache.Operation}' />
									<attribute name='{Attributes.EntityCache.Data}' groupby='true' alias='{Attributes.EntityCache.Data}' />
									<link-entity name='{EntityName.EntityCacheMessage}' from='{Attributes.EntityCache.EntityCacheId}' to='{Attributes.EntityCache.EntityCacheId}' link-type='outer' alias='{EntityCacheMessagAlias}'> 
										<attribute name='{Attributes.EntityCacheMessage.EntityCacheMessageId}' aggregate='countcolumn' alias='{EntityCacheMessagCountAlias}' />
										<filter type='and'>
											<condition attribute='{Attributes.EntityCacheMessage.StatusReason}' operator='eq' value='{(int)EntityCacheMessageStatusReason.Failed}' />	
										</filter>
									</link-entity> 
									<link-entity name='{EntityName.Contact}' from='{Attributes.Contact.ContactId}' to='{Attributes.EntityCache.RecordId}' alias='{ContactAlias}'>
										<attribute name='{Attributes.Contact.SourceSystemId}' groupby='true' alias='{Attributes.Contact.SourceSystemId}' />
										<filter type='and'>
											<condition attribute='{Attributes.Contact.SourceSystemId}' operator='not-null' />
										</filter>
									</link-entity>
									<order alias='{Attributes.EntityCache.CreatedOn}' descending='false' />
									<filter type='and'>
										<filter type='or'>
											<condition attribute='{Attributes.EntityCache.StatusReason}' operator='eq' value='{(int)EntityCacheStatusReason.Active}' />
											<filter type='and'>
												<condition attribute='{Attributes.EntityCache.StatusReason}' operator='eq' value='{(int)EntityCacheStatusReason.InProgress}' />
												<condition attribute='{Attributes.EntityCache.EligibleRetryTime}' operator='le' value='{DateTime.UtcNow.ToString("o")}' />
												<condition attribute='{Attributes.EntityCache.WasLastOperationSuccessful}' operator='eq' value='{false}' />
											</filter>
										</filter>
										<condition attribute='{Attributes.EntityCache.Type}' operator='eq' value='{type}' />
										<condition attribute='{Attributes.EntityCache.Operation}' operator='eq' value='{(int)EntityCacheOperation.Update}' />
									</filter>
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