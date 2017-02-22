using System;
using System.Text;
using System.Xml;
using System.IO;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using System.Collections.ObjectModel;
using Microsoft.Crm.Sdk.Messages;
using System.Globalization;
using Tc.Crm.Common.Constants;
using Tc.Crm.Common.Models;

namespace Tc.Crm.Common.Services
{
    public class CrmService : ICrmService
    {
        IOrganizationService organizationService;
        IConfigurationService configurationService;
        ILogger logger;
        
        public CrmService(IConfigurationService configurationService, ILogger logger)
        {
            this.logger = logger;
            this.configurationService = configurationService;
            this.organizationService = GetOrganizationService();
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Tc.Crm.Common.Services.ILogger.LogInformation(System.String)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "RetrieveMultipleRecords")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Tc.Crm.WebJob.AllocateResortTeam.Services.ILogger.LogInformation(System.String)")]
        public EntityCollection RetrieveMultipleRecords(string entityName, string[] columns, string[] filterKeys, string[] filterValues)
        {
            logger.LogInformation("RetrieveMultipleRecords - start");
            
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
            logger.LogInformation("RetrieveMultipleRecords - end");
            return GetRecordsUsingQuery(query);

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "RetrieveMultipleRecordsFetchXml")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Tc.Crm.WebJob.AllocateResortTeam.Services.ILogger.LogInformation(System.String)")]
        public EntityCollection RetrieveMultipleRecordsFetchXml(string query)
        {
            logger.LogInformation("RetrieveMultipleRecordsFetchXml - start");
            EntityCollection entityCollection = new EntityCollection();
            
            int fetchCount = 4;
            int pageNumber = 1;           
            string pagingCookie = null;

            while (true)
            {
                string xml = CreateXml(query, pagingCookie, pageNumber, fetchCount);
                FetchExpression fetch = new FetchExpression(xml);
                EntityCollection returnCollection = organizationService.RetrieveMultiple(fetch);
                entityCollection.Entities.AddRange(returnCollection.Entities);
                if (returnCollection.MoreRecords)
                {
                    pageNumber++;
                }
                else
                {
                    break;
                }
            }
            logger.LogInformation("RetrieveMultipleRecordsFetchXml - end");
            return entityCollection;

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CreateXml")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Tc.Crm.WebJob.AllocateResortTeam.Services.ILogger.LogInformation(System.String)")]
        public string CreateXml(string xml, string cookie, int page, int count)
        {
            logger.LogInformation("CreateXml - start");
            StringReader stringReader = new StringReader(xml);
            XmlTextReader reader = new XmlTextReader(stringReader);

            // Load document
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);
            logger.LogInformation("CreateXml - end");
            return CreateXml(doc, cookie, page, count);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Tc.Crm.WebJob.AllocateResortTeam.Services.ILogger.LogInformation(System.String)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CreateXml")]
        public string CreateXml(XmlDocument doc, string cookie, int page, int count)
        {
            logger.LogInformation("CreateXml - start");
            if (doc == null) throw new ArgumentNullException("doc");
            XmlAttributeCollection attrs = doc.DocumentElement.Attributes;

            if (cookie != null)
            {
                XmlAttribute pagingAttr = doc.CreateAttribute("paging-cookie");
                pagingAttr.Value = cookie;
                attrs.Append(pagingAttr);
            }

            XmlAttribute pageAttr = doc.CreateAttribute("page");
            pageAttr.Value = System.Convert.ToString(page,CultureInfo.CurrentCulture);
            attrs.Append(pageAttr);

            XmlAttribute countAttr = doc.CreateAttribute("count");
            countAttr.Value = System.Convert.ToString(count, CultureInfo.CurrentCulture);
            attrs.Append(countAttr);

            StringBuilder sb = new StringBuilder(1024);
            StringWriter stringWriter = new StringWriter(sb,CultureInfo.CurrentCulture);

            XmlTextWriter writer = new XmlTextWriter(stringWriter);
            doc.WriteTo(writer);
            writer.Close();

            logger.LogInformation("CreateXml - end");
            return sb.ToString();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Tc.Crm.WebJob.AllocateResortTeam.Services.ILogger.LogInformation(System.String)")]
        public EntityCollection GetRecordsUsingQuery(QueryExpression queryExpr)
        {
            logger.LogInformation("GetRecordsUsingQuery - start");
            int pageNumber = 1;
            int recordCount = 1;
            queryExpr.PageInfo = new PagingInfo();
            queryExpr.PageInfo.PageNumber = pageNumber;
            queryExpr.PageInfo.Count = recordCount;
            queryExpr.PageInfo.PagingCookie = null;
            EntityCollection entityCollection = null;
            while (true)
            {
                entityCollection = organizationService.RetrieveMultiple(queryExpr);

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
            logger.LogInformation("GetRecordsUsingQuery - end");
            return entityCollection;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "BulkAssign")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Tc.Crm.WebJob.AllocateResortTeam.Services.ILogger.LogInformation(System.String)")]
        public void BulkAssign(Collection<AssignInformation> assignRequestCollection)
        {
            logger.LogInformation("BulkAssign - start");
            if (assignRequestCollection == null || assignRequestCollection.Count == 0)
                throw new ArgumentNullException("assignRequestCollection");

            var batch = configurationService.ExecuteMultipleBatchSize;
            ExecuteMultipleRequest request = new ExecuteMultipleRequest()
            {
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                },
                Requests = new OrganizationRequestCollection()
            };

            for (int i = 0; i < assignRequestCollection.Count; i++)
            {
                var targetEntityName = assignRequestCollection[i].EntityName.ToLower();
                var targetId = assignRequestCollection[i].RecordId;
                var ownerType = (assignRequestCollection[i].RecordOwner.OwnerType == OwnerType.Team) ? EntityName.Team : EntityName.User;
                var ownerId = assignRequestCollection[i].RecordOwner.Id;
                request.Requests.Add(new AssignRequest {
                                                            Target = new EntityReference(targetEntityName, targetId)
                                                            , Assignee = new EntityReference(ownerType, ownerId)
                                                        });
                if (request.Requests.Count == batch)
                {
                    var response = (ExecuteMultipleResponse)organizationService.Execute(request);
                    LogResponses(response.Responses, request.Requests);
                    request.Requests.Clear();
                }

                if (request.Requests.Count < batch && i== assignRequestCollection.Count-1)
                {
                    var response = (ExecuteMultipleResponse)organizationService.Execute(request);
                    LogResponses(response.Responses, request.Requests);
                    request.Requests.Clear();
                }
            }
            logger.LogInformation("BulkAssign - end");
        }

        public void LogResponses(ExecuteMultipleResponseItemCollection responses, OrganizationRequestCollection requests)
        {
            for (int i = 0; i < responses.Count; i++)
            {
                if(responses[i].Fault != null)
                {

                    logger.LogError("Id: " + ((AssignRequest)requests[i]).Target.Id + "::Error:" + responses[i].Fault.TraceText);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Tc.Crm.WebJob.AllocateResortTeam.Services.ILogger.LogInformation(System.String)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetOrganizationService")]
        public IOrganizationService GetOrganizationService()
        {
            logger.LogInformation("GetOrganizationService - start");
            if (organizationService != null) return organizationService;

            var connectionString = configurationService.ConnectionString;
            CrmServiceClient client = new CrmServiceClient(connectionString);
            logger.LogInformation("GetOrganizationService - end");
            return (IOrganizationService)client;
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
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

