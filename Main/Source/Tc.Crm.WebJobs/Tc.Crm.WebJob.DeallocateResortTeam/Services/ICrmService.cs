using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Xml;

namespace Tc.Crm.WebJob.DeallocateResortTeam.Services
{
    public interface ICrmService : IDisposable
    {

        IOrganizationService GetOrganizationService();
        EntityCollection RetrieveMultipleRecords(string entityName, string[] columns, string[] filterKeys, string[] filterValues);
        EntityCollection GetRecordsUsingQuery(QueryExpression queryExpr);
        EntityCollection RetrieveMultipleRecordsFetchXml(string query);
        ExecuteMultipleResponse BulkUpdate(EntityCollection entityCollection);
        string CreateXml(string xml, string cookie, int page, int count);
        string CreateXml(XmlDocument doc, string cookie, int page, int count);
    }
}
