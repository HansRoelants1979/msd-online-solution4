using System;
using Microsoft.Xrm.Sdk;
using Tc.Crm.OutboundSynchronisation.Customer.Model;
using System.Collections.Generic;

namespace Tc.Crm.OutboundSynchronisation.Customer.Services
{
    public interface IOutboundSynchronisationDataService : IDisposable
    {

        List<EntityCacheModel> GetEntityCacheToProcess(string type, int numberOfElements);

        /// <summary>
        /// Retrieve active antity cache entities for defined type
        /// </summary>
        /// <param name="type">Type of entity</param>
        /// <param name="numberOfElements">Number of retrieved entities</param>
        /// <returns>active antity cache entities for defined type</returns>
        EntityCollection RetrieveEntityCaches(string type, int numberOfElements);

        List<EntityCacheModel> PrepareEntityCacheModel(EntityCollection entityCacheCollection);

        EntityCollection PrepareEntityCacheMessages(List<EntityCacheMessageModel> entityCacheMessageModelColletion);

        Guid CreateEntityCacheMessage(EntityCacheMessageModel entityCacheMessageModel);

        Entity PrepareEntityCacheMessage(EntityCacheMessageModel entityCacheMessageModel);

        Guid CreateRecord(Entity entity);

        void UpdateRecord(Entity entity);
        
       
    }
}