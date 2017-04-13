using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Xml;
using System.Collections.ObjectModel;
using Tc.Crm.Common.Models;
using Microsoft.Crm.Sdk.Messages;
using System.Collections.Generic;

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
        /// Execute assignment requests
        /// </summary>
        /// <param name="assignRequests">The requests</param>
        void BulkAssign(Collection<AssignInformation> assignRequests);

        /// <summary>
        /// Execute bulk update of entities
        /// </summary>
        /// <param name="entities">The entities</param>
        void BulkUpdate(IEnumerable<Entity> entities);
    }
}
