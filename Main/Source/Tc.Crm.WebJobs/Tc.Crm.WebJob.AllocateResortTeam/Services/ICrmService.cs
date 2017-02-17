using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using Tc.Crm.WebJob.AllocateResortTeam.Models;

namespace Tc.Crm.WebJob.AllocateResortTeam.Services
{
    public interface ICrmService : IDisposable
    {
        IList<BookingAllocation> GetBookingAllocations(BookingAllocationRequest bookingRequest);
        void Update(BookingAllocation bookingAllocation);
        IOrganizationService GetOrganizationService();
        EntityCollection RetrieveMultipleRecords(string entityName, string[] columns, string[] filterKeys, string[] filterValues, IOrganizationService service);
        EntityCollection GetRecordsUsingQuery(QueryExpression queryExpr, IOrganizationService service);
        EntityCollection RetrieveMultipleRecordsFetchXml(string query);
        ExecuteMultipleResponse BulkUpdate(EntityCollection entityCollection, IOrganizationService service);
    }
}
