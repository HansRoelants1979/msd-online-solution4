using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;

namespace Tc.Crm.WebJob.AllocateResortTeam.Services
{
    public interface ICrmService : IDisposable
    {        
        IOrganizationService GetOrganizationService();
        EntityCollection RetrieveMultipleRecords(string entityName, string[] columns, string[] filterKeys, string[] filterValues);
        EntityCollection GetRecordsUsingQuery(QueryExpression queryExpr);
        EntityCollection RetrieveMultipleRecordsFetchXml(string query);
        ExecuteMultipleResponse BulkUpdate(EntityCollection entityCollection);        
    }
}
