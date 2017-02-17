using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Tc.Crm.WebJob.AllocateResortTeam.Models;
using Tc.Crm.WebJob.AllocateResortTeam.Services;
using Microsoft.Xrm.Tooling.Connector;
using System.Configuration;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;

namespace Tc.Crm.WebJob.AllocateResortTeam
{
    public class CrmService : ICrmService
    {
        IOrganizationService organizationService;
        IConfigurationService configurationService;
        ILogger logger;
        public CrmService(IConfigurationService configurationService,ILogger logger)
        {
            this.configurationService = configurationService;
            this.organizationService = GetOrganizationService();
            this.logger = logger;
        }

        public IList<BookingAllocation> GetBookingAllocations()
        {
            throw new NotImplementedException();
        }

        public  EntityCollection RetrieveMultipleRecords(string entityName, string[] columns, string[] filterKeys, string[] filterValues, IOrganizationService service)
        {
            var query = new QueryExpression(entityName);
            query.ColumnSet = new ColumnSet(columns);


            for (int i = 0; i < filterKeys.Length; i++)
            {
                var condExpr = new ConditionExpression();
                condExpr.AttributeName = filterKeys[i];
                condExpr.Operator = ConditionOperator.Equal;
                condExpr.Values.Add(filterValues[i]);

                var fltrExpr = new FilterExpression(LogicalOperator.And);
                fltrExpr.AddCondition(condExpr);

                query.Criteria.AddFilter(fltrExpr);
            }
            return GetRecordsUsingQuery(query, service);

        }

        public EntityCollection RetrieveMultipleRecordsFetchXml(string Query, IOrganizationService service)
        {
            //paging has to  impliment 
            FetchExpression fetch = new FetchExpression(Query);
            return service.RetrieveMultiple(fetch);          

        }
        public EntityCollection GetRecordsUsingQuery(QueryExpression queryExpr, IOrganizationService service)
        {
            int pageNumber = 1;
            int recordCount = 1;
            queryExpr.PageInfo = new PagingInfo();
            queryExpr.PageInfo.PageNumber = pageNumber;
            queryExpr.PageInfo.Count = recordCount;
            queryExpr.PageInfo.PagingCookie = null;
            EntityCollection entityCollection = null;
            while (true)
            {
                entityCollection = service.RetrieveMultiple(queryExpr);

                // Check for more records, if it returns true.
                if (entityCollection.MoreRecords)
                {
                    // Increment the page number to retrieve the next page.
                    queryExpr.PageInfo.PageNumber++;

                    // Set the paging cookie to the paging cookie returned from current results.
                    queryExpr.PageInfo.PagingCookie = entityCollection.PagingCookie;
                }
                else
                {
                    // If no more records are in the result nodes, exit the loop.
                    break;
                }

            }

            return entityCollection;
        }


        public  ExecuteMultipleResponse BulkUpdate(EntityCollection entityCollection ,IOrganizationService service)
        {
            ExecuteMultipleRequest request = new ExecuteMultipleRequest()
            {
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                },
                Requests = new OrganizationRequestCollection()
            };

            for (int i = 0; i < entityCollection.Entities.Count; i++)
            {
                request.Requests.Add(new UpdateRequest() { Target = entityCollection[i] });
            }

            return (ExecuteMultipleResponse)service.Execute(request);
        }



        public IOrganizationService GetOrganizationService()
        {
            if (organizationService != null) return organizationService;

            var connectionString = configurationService.ConnectionString;
            CrmServiceClient client = new CrmServiceClient(connectionString);
            return (IOrganizationService)client;
        }

        public void Update(BookingAllocation bookingAllocation)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                DisposeObject(organizationService);
                DisposeObject(configurationService);
                DisposeObject(logger);
            }

            disposed = true;
        }

        void DisposeObject(Object obj)
        {
            if (obj != null)
            {
                if (obj is IDisposable)
                    ((IDisposable)obj).Dispose();
                else
                    obj = null;
            }
                
        }
    }
}
