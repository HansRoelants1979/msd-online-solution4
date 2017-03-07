using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Xml;
using System.Collections.ObjectModel;
using Tc.Crm.Common.Models;
using Microsoft.Crm.Sdk.Messages;

namespace Tc.Crm.Common.Services
{
    public interface ICrmService : IDisposable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        IOrganizationService GetOrganizationService();
        EntityCollection RetrieveMultipleRecords(string entityName, string[] columns, string[] filterKeys, string[] filterValues);
        EntityCollection GetRecordsUsingQuery(QueryExpression queryExpression);
        EntityCollection RetrieveMultipleRecordsFetchXml(string query);
        void BulkAssign(Collection<AssignInformation> assignRequests);
        string CreateXml(string xml, string cookie, int page, int count);
        string CreateXml(XmlDocument doc, string cookie, int page, int count);
        void ExecuteBulkAssignRequests(ExecuteMultipleRequest request);
        string FormatFaultException(AssignRequest assignRequest, OrganizationServiceFault fault);
    }
}
