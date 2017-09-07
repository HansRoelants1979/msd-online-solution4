using System;
using Microsoft.Xrm.Sdk;
using Tc.Crm.Common.Models;
using System.Collections.Generic;

namespace Tc.Crm.OutboundSynchronisation.Customer.Services
{
    public interface IOutboundSynchronisationDataService : IDisposable
    {

        List<EntityCache> GetEntityCacheToProcess(string type, int numberOfElements);

        /// <summary>
        /// Retrieve active antity cache entities for defined type
        /// </summary>
        /// <param name="type">Type of entity</param>
        /// <param name="numberOfElements">Number of retrieved entities</param>
        /// <returns>active antity cache entities for defined type</returns>
        EntityCollection RetrieveEntityCaches(string type, int numberOfElements);

        List<EntityCache> PrepareEntityCacheModel(EntityCollection entityCacheCollection);

        EntityCollection PrepareEntityCacheMessages(List<EntityCacheMessage> entityCacheMessageModelColletion);

        Guid CreateEntityCacheMessage(EntityCacheMessage entityCacheMessageModel);

        Entity PrepareEntityCacheMessage(EntityCacheMessage entityCacheMessageModel);

        Guid CreateRecord(Entity entity);

        void UpdateRecord(Entity entity);

        void UpdateEntityStatus(Guid id, string entityName, int StateCode, int StatusCode);
    }
}