using Microsoft.Xrm.Sdk;
using System.Collections.ObjectModel;
using Tc.Crm.Common.Models;
using System.Collections.Generic;
using System;

namespace Tc.Crm.Common.Services
{
    public interface ICrmService
    {
        /// <summary>
        /// Search CRM with the fetch query
        /// </summary>
        /// <param name="query">FetchXml query</param>
        /// <returns></returns>
        EntityCollection RetrieveMultipleRecordsFetchXml(string query);

        /// <summary>
        /// Search CRM with the fetch query
        /// </summary>
        /// <param name="query">FetchXml query</param>
        /// <param name="numberOfElements">Number of retrieved elements</param>
        /// <returns></returns>
        EntityCollection RetrieveMultipleRecordsFetchXml(string query, int numberOfElements);

        /// <summary>
        /// Execute assignment requests
        /// </summary>
        /// <param name="assignRequests">The requests</param>
        /// <param name="batchSize"></param>
        void BulkAssign(Collection<AssignInformation> assignRequests, int batchSize);

        /// <summary>
        /// Execute bulk update of entities
        /// </summary>
        /// <param name="entities">The entities</param>
        /// <param name="batchSize"></param>
        void BulkUpdate(IEnumerable<Entity> entities, int batchSize);

        Guid Create(Entity entity);

        void Update(Entity entity);
    }
}
