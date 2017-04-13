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
using System.Collections.Generic;

namespace Tc.Crm.Common.Services
{
    public class CrmService : ICrmService, IDisposable
    {        
        private IConfigurationService configurationService;
        private ILogger logger;
        private IOrganizationService organizationService;

        public CrmService(IConfigurationService configurationService, ILogger logger)
        {
            this.logger = logger;
            this.configurationService = configurationService;
            this.organizationService = new CrmServiceClient(configurationService.ConnectionString);
        }

        public EntityCollection RetrieveMultipleRecordsFetchXml(string query)
        {
            EntityCollection entityCollection = new EntityCollection();

            int fetchCount = 10000;
            int pageNumber = 1;
            string pagingCookie = null;

            while (true)
            {
                string xml = CreateXml(query, pagingCookie, pageNumber, fetchCount);
                FetchExpression fetch = new FetchExpression(xml);
                EntityCollection returnCollection = organizationService.RetrieveMultiple(fetch);
                entityCollection.Entities.AddRange(returnCollection.Entities);
                if (!returnCollection.MoreRecords)
                    break;
                pageNumber++;
            }
            return entityCollection;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "BulkAssign")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Tc.Crm.WebJob.AllocateResortTeam.Services.ILogger.LogInformation(System.String)")]
        public void BulkAssign(Collection<AssignInformation> assignRequestCollection)
        {
            //logger.LogInformation("BulkAssign - start");
            if (assignRequestCollection == null || assignRequestCollection.Count == 0)
                throw new ArgumentNullException("assignRequestCollection");

            logger.LogInformation("\r\n\r\n***** Going to execute " + assignRequestCollection.Count + " assign requests ***** ");

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

                var target = new EntityReference(targetEntityName, targetId);
                if(!string.IsNullOrWhiteSpace(assignRequestCollection[i].RecordName))
                target.Name = assignRequestCollection[i].RecordName;

                var assignee = new EntityReference(ownerType, ownerId);
                if(!string.IsNullOrWhiteSpace(assignRequestCollection[i].RecordOwner.Name))
                assignee.Name = assignRequestCollection[i].RecordOwner.Name;

                request.Requests.Add(new AssignRequest
                {
                    Target = target,
                    Assignee = assignee
                });
                if (request.Requests.Count == batch)
                {
                    ExecuteMultipleRequests(request);
                }

                if (request.Requests.Count < batch && i == assignRequestCollection.Count - 1)
                {
                    ExecuteMultipleRequests(request);
                }
            }
            //logger.LogInformation("BulkAssign - end");
        }

        public void BulkUpdate(IEnumerable<Entity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");

            var batch = configurationService.ExecuteMultipleBatchSize;

            ExecuteMultipleRequest request = new ExecuteMultipleRequest
            {
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                },
                Requests = new OrganizationRequestCollection()
            };

            foreach(var entity in entities)
            {
                request.Requests.Add(new UpdateRequest { Target = entity });
                if (request.Requests.Count == batch)
                {
                    ExecuteMultipleRequests(request);
                }
            }
            if (request.Requests.Count > 0)
            {
                ExecuteMultipleRequests(request);
            }
        }

        #region Private helpers

        private string CreateXml(string xml, string cookie, int page, int count)
        {
            using (var stringReader = new StringReader(xml))
            using (var reader = new XmlTextReader(stringReader))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);
                return CreateXml(doc, cookie, page, count);
            }
        }

        private string CreateXml(XmlDocument doc, string cookie, int page, int count)
        {
            if (doc == null) throw new ArgumentNullException("doc");
            XmlAttributeCollection attrs = doc.DocumentElement.Attributes;

            if (cookie != null)
            {
                XmlAttribute pagingAttr = doc.CreateAttribute("paging-cookie");
                pagingAttr.Value = cookie;
                attrs.Append(pagingAttr);
            }

            XmlAttribute pageAttr = doc.CreateAttribute("page");
            pageAttr.Value = Convert.ToString(page, CultureInfo.CurrentCulture);
            attrs.Append(pageAttr);

            XmlAttribute countAttr = doc.CreateAttribute("count");
            countAttr.Value = Convert.ToString(count, CultureInfo.CurrentCulture);
            attrs.Append(countAttr);

            StringBuilder sb = new StringBuilder(1024);
            using (var writer = new XmlTextWriter(new StringWriter(sb, CultureInfo.CurrentCulture)))
            {
                doc.WriteTo(writer);
            }
            return sb.ToString();
        }

        private void ExecuteMultipleRequests(ExecuteMultipleRequest request)
        {
            logger.LogInformation("\r\n***Executing " + request.Requests.Count + " update requests as a batch***");
            var response = (ExecuteMultipleResponse)organizationService.Execute(request);
            LogResponses(response.Responses, request.Requests);
            request.Requests.Clear();
        }

        private void LogResponses(ExecuteMultipleResponseItemCollection responses, OrganizationRequestCollection requests)
        {
            foreach (var response in responses)
            {
                if (response.Fault == null)
                    logger.LogInformation(requests[response.RequestIndex].FormatSuccessMessage());
                else
                    logger.LogError(FormatFaultException(requests[response.RequestIndex], response.Fault));
            }
        }

        private static string FormatFaultException(OrganizationRequest request, OrganizationServiceFault fault)
        {
            var message = new StringBuilder();
            string requestMessage = request.FormatFaultException();

            message.AppendLine(requestMessage);
            if (fault.Message != null)
                message.AppendLine("Message: " + fault.Message);
            if (fault.TraceText != null)
                message.AppendLine("TraceText: " + fault.TraceText);
            if (fault.InnerFault != null)
            {
                if (fault.InnerFault.Message != null)
                    message.AppendLine("InnerFault Message: " + fault.InnerFault.Message);
                if (fault.InnerFault.TraceText != null)
                    message.AppendLine("InnerFault TraceText: " + fault.InnerFault.TraceText);
            }
            
            return message.ToString();
        }

        #endregion

        #region IDisposable

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

        #endregion
    }

    public static class CrmRequestExtensions
    {
        public static string FormatSuccessMessage(this AssignRequest request)
        {
            return request != null ? string.Format("'{0}' of Name: '{1}', Id: '{2}' was successfully assigned to '{3}' of Name: '{4}', Id: '{5}' ",
                request.Target.LogicalName, 
                request.Target.Name, 
                request.Target.Id, 
                request.Assignee.LogicalName, 
                request.Assignee.Name, 
                request.Assignee.Id) : null;
        }

        public static string FormatSuccessMessage(this UpdateRequest request)
        {
            return request != null ? string.Format("{0} of ID {1} was successfully updated", request.Target.LogicalName, request.Target.Id) : null;
        }

        public static string FormatSuccessMessage(this OrganizationRequest request)
        {
            if (request is AssignRequest)
            {
                return ((AssignRequest)request).FormatSuccessMessage();
            }
            if (request is UpdateRequest)
            {
                return ((UpdateRequest)request).FormatSuccessMessage();
            }
            return null;
        }

        public static string FormatFaultException(this AssignRequest request)
        {
            return request != null ? string.Format("Request failed while assigning '{0}' of Name: '{1}', Id: '{2}' to '{3}' of Name: '{4}', Id: '{5}' due to below exception",
                request.Target.LogicalName,
                request.Target.Name,
                request.Target.Id,
                request.Assignee.LogicalName,
                request.Assignee.Name,
                request.Assignee.Id) : null;
        }

        public static string FormatFaultException(this UpdateRequest request)
        {
            return request != null ? string.Format("Request failed while updating '{0}' of Id: '{1}' due to below exception", request.Target.LogicalName, request.Target.Id) : null;
        }

        public static string FormatFaultException(this OrganizationRequest request)
        {
            if (request is AssignRequest)
            {
                return ((AssignRequest)request).FormatFaultException();
            }
            if (request is UpdateRequest)
            {
                return ((UpdateRequest)request).FormatFaultException();
            }
            return null;
        }
    }
    
}

