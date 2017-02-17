using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using Tc.Crm.WebJob.DeallocateResortTeam.Models;

namespace Tc.Crm.WebJob.DeallocateResortTeam.Services
{
    public interface ICrmService : IDisposable
    {

        IList<BookingDeallocation> GetBookingDeallocations();
        void Update(BookingDeallocation bookingDeallocation);
        IOrganizationService GetOrganizationService();
        EntityCollection RetrieveMultipleRecords(string entityName, string[] columns, string[] filterKeys, string[] filterValues, IOrganizationService service);
        EntityCollection GetRecordsUsingQuery(QueryExpression queryExpr, IOrganizationService service);
        EntityCollection RetrieveMultipleRecordsFetchXml(string Query, IOrganizationService service);
        ExecuteMultipleResponse BulkUpdate(EntityCollection entityCollection, IOrganizationService service);
    }
}
