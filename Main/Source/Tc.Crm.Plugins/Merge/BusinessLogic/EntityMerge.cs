using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Tc.Crm.Plugins.Merge.BusinessLogic
{
    public abstract class EntityMerge
    {
        protected readonly IOrganizationService service;

        protected EntityMerge(IOrganizationService service)
        {
            this.service = service;
        }

        protected abstract string EntityName { get; }

        protected abstract string SourceSystemIdName { get; }

        protected abstract string DuplicateSourceSystemIdName { get; }

        protected abstract string RecordId { get; }

        public virtual void UpdateSourceSystemId(Guid recordId, string outcomeId)
        {
            var entity = new Entity(EntityName);
            entity.Id = recordId;
            entity.Attributes[SourceSystemIdName] = outcomeId;
            entity.Attributes[DuplicateSourceSystemIdName] = outcomeId;

            service.Update(entity);
        }

        public virtual void UpdateDuplicateSourceSystemId(Guid recordId, string outcomeId)
        {
            var entity = new Entity(Entities.Contact);
            entity.Id = recordId;
            entity.Attributes[DuplicateSourceSystemIdName] = outcomeId;

            service.Update(entity);
        }

        public virtual Guid GetExistingEntity(string outcomeId)
        {
            var query = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                             <entity name='{EntityName}'>
                               <attribute name='{RecordId}' />
                               <filter type='and'>
                                 <condition attribute='{SourceSystemIdName}' operator='eq' value='{outcomeId}' />
                               </filter>
                             </entity>
                           </fetch>";

            var fetch = new FetchExpression(query);
            var collection = service.RetrieveMultiple(fetch);
            var entity = collection.Entities.FirstOrDefault();
            return entity?.Id ?? Guid.Empty;
        }

        public virtual void CreateEntityMerge(string entityMergeName, Guid masterId, Guid subordinateId)
        {
            var entityMerge = new Entity(Entities.EntityMerge);
            entityMerge.Attributes[Attributes.EntityMerge.Name] = entityMergeName;
            entityMerge.Attributes[Attributes.EntityMerge.Master] = new EntityReference(EntityName, masterId);
            entityMerge.Attributes[Attributes.EntityMerge.Subordinate] = new EntityReference(EntityName, subordinateId);
            var executionDate = DateTime.Now;
            entityMerge.Attributes[Attributes.EntityMerge.ExecutionTime] =
                new DateTime(executionDate.Year, executionDate.Month, executionDate.Day, 23, 0, 0, DateTimeKind.Local);

            service.Create(entityMerge);
        }
    }
}