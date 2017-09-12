using System;
using Microsoft.Xrm.Sdk;
using Tc.Crm.Common.Models;
using System.Collections.Generic;
using Tc.Crm.Common.IntegrationLayer.Jti.Models;
using Tc.Crm.Common.IntegrationLayer.Model;

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

        void UpdateEntityStatus(Guid id, string entityName, int stateCode, int statusCode);

        /// <summary>
        /// Create JWT token
        /// </summary>
        /// <typeparam name="T">JsonWebTokenPayload class </typeparam>
        /// <param name="privateKey">Sekret key</param>
        /// <param name="payloadObj">Parametrized object with token data</param>
        /// <returns>Returns token</returns>
        string CreateJwtToken<T>(string privateKey, T payloadObj) where T : JsonWebTokenPayloadBase;

        /// <summary>
        /// Send Http post request 
        /// </summary>
        /// <param name="serviceUrl"></param>
        /// <param name="token"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        ResponseEntity SendHttpPostRequest(string serviceUrl, string token, string data);
    }
}