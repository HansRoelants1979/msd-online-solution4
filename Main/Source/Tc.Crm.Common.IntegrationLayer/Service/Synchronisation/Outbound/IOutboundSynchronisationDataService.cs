using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Tc.Crm.Common.Models;

namespace Tc.Crm.Common.IntegrationLayer.Service.Synchronisation.Outbound
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

        /// <summary>
        /// Update Entity Cache record
        /// </summary>
        /// <param name="id">Record Id</param>
        /// <param name="stateCode"></param>
        /// <param name="statusCode"></param>
        void UpdateEntityCacheStatus(Guid id, int stateCode, int statusCode);

        /// <summary>
        /// Update Entity Cache Message record
        /// </summary>
        /// <param name="id"></param>
        /// <param name="stateCode"></param>
        /// <param name="statusCode"></param>
        /// <param name="notes"></param>
        void UpdateEntityCacheMessageStatus(Guid id, int stateCode, int statusCode, string notes = null);

        /// <summary>
        /// Get xpt value from CRM Configuration
        /// </summary>
        /// <returns></returns>
        string GetExpiry();

        /// <summary>
        /// 
        /// </summary>
        /// <returns>xpt value</returns>
        string GetNotBeforeTime();

        /// <summary>
        /// Get secret key value for JWT token from CRM Configuration
        /// </summary>
        /// <returns>Secret key value for JWT token</returns>
        string GetSecretKey();

        /// <summary>
        /// Get service URL value for Integration Layer from CRM Configuration
        /// </summary>
        /// <returns>Service URL</returns>
        string GetServiceUrl();
    }
}