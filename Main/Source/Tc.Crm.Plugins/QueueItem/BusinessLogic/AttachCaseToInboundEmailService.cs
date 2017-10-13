using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Text.RegularExpressions;

namespace Tc.Crm.Plugins.QueueItem.BusinessLogic
{
    public class AttachCaseToInboundEmailService
    {

        public IPluginExecutionContext context = null;
        public IOrganizationService service = null;
        public ITracingService trace = null;

        public AttachCaseToInboundEmailService(IPluginExecutionContext context, ITracingService trace, IOrganizationService service)
        {
            this.context = context;
            this.service = service;
            this.trace = trace;
        }
        private bool IsContextValid()
        {
            if (!context.MessageName.Equals("create", StringComparison.OrdinalIgnoreCase)) return false;
            if (context.Stage != (int)PluginStage.Postoperation) return false;
            if (!context.InputParameters.Contains(InputParameters.Target)
                || !(context.InputParameters[InputParameters.Target] is Entity))
                return false;
            return true;
        }
        public void AttachCaseToInboundEmail()
        {
            trace.Trace("Begin - AttachCaseToInboundEmailService");

            if (!IsContextValid()) return;

            Entity queueItem = (Entity)context.InputParameters[InputParameters.Target];

            if (!queueItem.Contains(Attributes.QueueItem.ObjectTypeCode) ||
                !queueItem.Contains(Attributes.QueueItem.ObjectId)) return;


            OptionSetValue objectTypeCode = (OptionSetValue)queueItem[Attributes.QueueItem.ObjectTypeCode];
            if (!(objectTypeCode.Value == 4202)) return;

            EntityReference emailObject = (EntityReference)queueItem[Attributes.QueueItem.ObjectId];
            if (emailObject == null) return;

            string subject, description;
            GetSubjectAndDescription(emailObject, out subject, out description);
            if (string.IsNullOrWhiteSpace(subject) && string.IsNullOrWhiteSpace(description)) return;

            string caseNumber = string.Empty;
            if (!string.IsNullOrWhiteSpace(subject))
            {
                caseNumber = ParseEmailContent(subject);

            }
            if (string.IsNullOrWhiteSpace(caseNumber) && !string.IsNullOrWhiteSpace(description))
            {
                caseNumber = ParseEmailContent(description);
            }

            if (string.IsNullOrWhiteSpace(caseNumber)) return;
            AttachCaseToEmail(emailObject, caseNumber);

            trace.Trace("End - AttachCaseToInboundEmailService");
        }
        private void GetSubjectAndDescription(EntityReference emailObject,
            out string subject,
            out string description)
        {
            subject = string.Empty;
            description = string.Empty;

            Entity email = GetEmail(emailObject.Id);
            if (email == null) return;

            var directionCode = email.Contains(Attributes.Email.DirectionCode) ? (bool)email[Attributes.Email.DirectionCode] : true;
            if (directionCode == true) return;

            EntityReference regarding = email.Contains(Attributes.Email.Regarding) ? email[Attributes.Email.Regarding] as EntityReference : null;
            if (regarding != null && regarding.Id != Guid.Empty) return;

            subject = email.Contains(Attributes.Email.Subject) ? email[Attributes.Email.Subject].ToString() : string.Empty;
            description = email.Contains(Attributes.Email.Description) ? email[Attributes.Email.Description].ToString() : string.Empty;
        }
        private void AttachCaseToEmail(EntityReference emailObject, string caseNumber)
        {
            trace.Trace("Begin - AttachCaseToEmail");
            Guid caseId = GetCase(caseNumber);
            if (caseId == Guid.Empty) return;
            Entity updateEmail = new Entity(Entities.Email, emailObject.Id);
            updateEmail[Attributes.Email.Regarding] = new EntityReference(Entities.Case, caseId);
            service.Update(updateEmail);
            trace.Trace("End - AttachCaseToEmail");
        }
        private Guid GetCase(string caseNumber)
        {
            trace.Trace("Begin - GetCase");
            var caseFetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' no-lock='true'>
                              <entity name='incident'>
                                <attribute name='incidentid' />
                                <filter type='and'>
                                  <condition attribute='ticketnumber' operator='eq' value='{caseNumber}' />
                                  <condition attribute='statecode' operator='eq' value='0' />
                                </filter>
                              </entity>
                            </fetch>";

            EntityCollection incidents = service.RetrieveMultiple(new FetchExpression(caseFetch));
            if (incidents != null && incidents.Entities.Count > 0)
            {
                var incident = incidents.Entities[0];
                Guid incidentId = incident.Contains(Attributes.Case.CaseId) ? (Guid)incident[Attributes.Case.CaseId] : Guid.Empty;
                trace.Trace("End - GetCase");
                return incidentId;
            }
            trace.Trace("End - GetCase");
            return Guid.Empty;
        }
        private string ParseEmailContent(string content)
        {
            trace.Trace("Begin - ParseEmailContent");
            var configurationQuery = GetConfigurationQuery();
            EntityCollection configurations = service.RetrieveMultiple(configurationQuery);
            if (configurations != null && configurations.Entities.Count > 0)
            {
                string caseTitlePattern = configurations.Entities[0].Attributes.Contains(Attributes.Configuration.Value) ? configurations.Entities[0].GetAttributeValue<string>(Attributes.Configuration.Value) : null;
                if (string.IsNullOrWhiteSpace(caseTitlePattern)) return null;
                Match match = Regex.Match(content, caseTitlePattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                trace.Trace("After finding match and status:" + match.Success);
                if (match.Success)
                {
                    return match.Value;
                }
                return null;
            }
            trace.Trace("End - ParseEmailContent");
            return null;
        }
        private Entity GetEmail(Guid emailId)
        {
            trace.Trace("Begin - GetEmail");
            Entity email = service.Retrieve(Entities.Email, emailId,
                new ColumnSet(Attributes.Email.DirectionCode,
                Attributes.Email.Regarding,
                Attributes.Email.Subject,
                Attributes.Email.Description));

            if (email != null) return email;
            trace.Trace("End - GetEmail");
            return null;
        }
        private QueryExpression GetConfigurationQuery()
        {
            trace.Trace("Begin - GetConfigurationQuery");
            QueryExpression configurationQuery = new QueryExpression
            {
                EntityName = Entities.Configuration,
                ColumnSet = new ColumnSet(Attributes.Configuration.Value),
                Criteria =
                        {
                        FilterOperator = LogicalOperator.And,
                        Conditions =
                            {
                                new ConditionExpression
                                    {
                                        AttributeName = Attributes.Configuration.Name,
                                        Operator = ConditionOperator.Equal,
                                        Values = { Configurationkeys.CaseTitlePattern }
                                    }
                            }
                      }
            };
            trace.Trace("End - GetConfigurationQuery");
            return configurationQuery;
        }
    }
}
