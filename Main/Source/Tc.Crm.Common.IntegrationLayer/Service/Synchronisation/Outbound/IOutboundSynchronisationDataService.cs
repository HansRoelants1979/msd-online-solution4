using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Tc.Crm.Common.Models;

namespace Tc.Crm.Common.IntegrationLayer.Service.Synchronisation.Outbound
{
    public interface IOutboundSynchronisationDataService : IDisposable
    {
        /// <summary>
        /// Retrieve active antity cache entities for defined type with 'create' operation type
        /// </summary>
        /// <param name="type">Type of entity</param>
        /// <param name="numberOfElements">Number of retrieved entities</param>
        /// <returns>active antity cache entities for defined type</returns>
        List<EntityCache> GetCreatedEntityCacheToProcess(string type, int numberOfElements);

        /// <summary>
        /// Retrieve active antity cache entities for defined type with 'update' operation type
        /// </summary>
        /// <param name="type">Type of entity</param>
        /// <param name="numberOfElements">Number of retrieved entities</param>
        /// <returns>active antity cache entities for defined type</returns>
        List<EntityCache> GetUpdatedEntityCacheToProcess(string type, int numberOfElements);

		/// <summary>
		/// Create entity cache message
		/// </summary>
		/// <param name="entityCacheMessageModel">data model</param>
		/// <returns></returns>
		Guid CreateEntityCacheMessage(EntityCacheMessage entityCacheMessageModel);

        /// <summary>
        /// Update Entity Cache record
        /// </summary>
        /// <param name="id">Record Id</param>
        /// <param name="stateCode"></param>
        /// <param name="statusCode"></param>
	    void UpdateEntityCacheStatus(Guid id, Enum stateCode, Enum statusCode);

		/// <summary>
		/// Update Entity Cache record with send to IL status
		/// </summary>
		/// <param name="id">Record Id</param>
		/// <param name="wasSuccess">Whether call to IL was successful</param>
		/// <param name="eligibleRetryTime">Eligible time for future retry</param>
		void UpdateEntityCacheSendToIntegrationLayerStatus(Guid id, bool wasSuccess, DateTime? eligibleRetryTime = null);

	    /// <summary>
	    /// Update Entity Cache Message record
	    /// </summary>
	    /// <param name="id"></param>
	    /// <param name="stateCode"></param>
	    /// <param name="statusCode"></param>
	    /// <param name="notes"></param>
	    void UpdateEntityCacheMessageStatus(Guid id, Enum stateCode, Enum statusCode, string notes = null);

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
	    /// Get retry section from CRM Configuration
	    /// </summary>
	    /// <returns>array of retry times in minutes</returns>
	    int[] GetRetries();
	}
}