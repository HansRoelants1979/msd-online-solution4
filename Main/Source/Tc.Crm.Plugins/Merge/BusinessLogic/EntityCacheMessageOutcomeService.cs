using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Tc.Crm.Plugins.Merge.Helper;
using Tc.Crm.Plugins.Merge.Models;

namespace Tc.Crm.Plugins.Merge.BusinessLogic
{
    public class EntityCacheMessageOutcomeService
    {
        private readonly ITracingService trace;
        private readonly IOrganizationService service;
        private readonly IEntityMergeFactory entityMergeFactory;

        public EntityCacheMessageOutcomeService(ITracingService trace, IOrganizationService service, IEntityMergeFactory entityMergeFactory)
        {
            if (trace == null)
                throw new ArgumentNullException(nameof(trace));
            this.trace = trace;

            if (service == null)
            {
                this.trace.Trace("Organization service is null");
                throw new ArgumentNullException(nameof(service));
            }

            this.service = service;
            this.entityMergeFactory = entityMergeFactory;
        }

        #region Public Methods

        public void Run(IPluginExecutionContext context)
        {
            if (context == null)
            {
                trace.Trace("Plugin execution context is null");
                throw new ArgumentNullException(nameof(context));
            }

            var entityCacheMessageId = ((Entity)context.InputParameters[InputParameters.Target]).Id;
            var entityCacheMessage = RetrieveEntityCacheMessage(entityCacheMessageId);
            trace.Trace($"EntityCacheMessage was retrieved: {entityCacheMessage.Name}");

            var entityCache = RetrieveEntityCache(entityCacheMessage.EntityCacheId);
            if (entityCache == null || entityCache.Id == Guid.Empty)
            {
                trace.Trace($"EntityCache for EntityCacheMessage: {entityCacheMessage.Name} does not exist");
                return;
            }
            trace.Trace($"EntityCache was retrieved: {entityCache.Name}");


            var entityMerge = entityMergeFactory.GetEntityMerge(entityCache.Type, service);
            var existingEntityId = entityMerge.GetExistingEntity(entityCacheMessage.OutcomeId);

            if (existingEntityId != Guid.Empty)
            {
                trace.Trace($"Contact was retrieved: {existingEntityId}");
                if (existingEntityId == entityCache.RecordId)
                {
                    entityMerge.UpdateSourceSystemId(entityCache.RecordId, entityCacheMessage.OutcomeId);
                    trace.Trace($"Record Id from EntityCache and existing record Id are the same. Source System Id for: {entityCache.RecordId} - was updated");
                }
                else
                {
                    entityMerge.UpdateDuplicateSourceSystemId(entityCache.RecordId, entityCacheMessage.OutcomeId);
                    trace.Trace($"Source System Id for Record Id from EntityCache: {entityCache.RecordId} - was updated");

                    entityMerge.UpdateDuplicateSourceSystemId(existingEntityId, entityCacheMessage.OutcomeId);
                    trace.Trace($"Source System Id for existing entity: {existingEntityId} - was updated");

                    entityMerge.CreateEntityMerge(entityCacheMessage.Name, existingEntityId, entityCache.RecordId);
                    trace.Trace($"Entity Merge: {entityCacheMessage.Name} - was created");
                }
            }
            else
            {
                entityMerge.UpdateSourceSystemId(entityCache.RecordId, entityCacheMessage.OutcomeId);
                trace.Trace($"Source System Id for Record Id from EntityCache: {entityCache.RecordId} - was updated");
            }
        }

        #endregion Public Methods

        #region Private Methods

        private EntityCache RetrieveEntityCache(Guid entityCacheId)
        {
            var columns = new ColumnSet(
                Attributes.EntityCache.Name,
                Attributes.EntityCache.EntityCacheId,
                Attributes.EntityCache.RecordId,
                Attributes.EntityCache.Type);
            var entity = service.Retrieve(Entities.EntityCache, entityCacheId, columns);

            return GetEntityCacheModel(entity);
        }

        private EntityCacheMessage RetrieveEntityCacheMessage(Guid entityCacheMessageId)
        {
            var columns = new ColumnSet(
                Attributes.EntityCacheMessage.Name, 
                Attributes.EntityCacheMessage.EntityCacheId,
                Attributes.EntityCacheMessage.OutcomeId);
            var entity = service.Retrieve(Entities.EntityCacheMessage, entityCacheMessageId, columns); 

            return GetEntityCacheMessageModel(entity);
        }

        private EntityCache GetEntityCacheModel(Entity entity)
        {
            if (entity == null)
                return null;

            var entityCacheModel = new EntityCache(entity.Id);

            if (EntityHelper.HasAttributeNotNull(entity, Attributes.EntityCache.Name))
                entityCacheModel.Name = entity.Attributes[Attributes.EntityCache.Name].ToString();

            if (EntityHelper.HasAttributeNotNull(entity, Attributes.EntityCache.RecordId))
                entityCacheModel.RecordId = Guid.Parse(entity.Attributes[Attributes.EntityCache.RecordId].ToString());

            if (EntityHelper.HasAttributeNotNull(entity, Attributes.EntityCache.Type))
                entityCacheModel.Type = ConvertEntityCacheType(entity.Attributes[Attributes.EntityCache.Type].ToString());

            return entityCacheModel;
        }

        private EntityCacheMessage GetEntityCacheMessageModel(Entity entity)
        {
            if (entity == null)
                return null;

            var entityCacheMessageModel = new EntityCacheMessage(entity.Id);

            if (EntityHelper.HasAttributeNotNull(entity, Attributes.EntityCacheMessage.Name))
                entityCacheMessageModel.Name = entity.Attributes[Attributes.EntityCacheMessage.Name].ToString();

            if (EntityHelper.HasAttributeNotNull(entity, Attributes.EntityCacheMessage.OutcomeId))
                entityCacheMessageModel.OutcomeId = entity.Attributes[Attributes.EntityCacheMessage.OutcomeId].ToString();

            if (EntityHelper.HasAttributeNotNull(entity, Attributes.EntityCacheMessage.EntityCacheId))
                entityCacheMessageModel.EntityCacheId = ((EntityReference)entity.Attributes[Attributes.EntityCacheMessage.EntityCacheId]).Id;

            return entityCacheMessageModel;
        }

        private EntityCacheType ConvertEntityCacheType(string type)
        {
            return type == EntityCacheTypeNames.Contact ? EntityCacheType.Customer : EntityCacheType.Account;
        }

        #endregion Private Methods
    }
}