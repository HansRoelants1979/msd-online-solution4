using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tc.Crm.Plugins.Email.BusinessLogic
{
    public class SendEmailService
    {

        public IPluginExecutionContext context = null;
        public IOrganizationService service = null;
        public ITracingService trace = null;

        public SendEmailService(IPluginExecutionContext context, ITracingService trace, IOrganizationService service)
        {
            this.context = context;
            this.service = service;
            this.trace = trace;
        }

        private bool IsContextValid()
        {
            if (!context.MessageName.Equals("send", StringComparison.OrdinalIgnoreCase)) return false;
            if (context.Stage != (int)PluginStage.Preoperation) return false;
            //if (!context.InputParameters.Contains(InputParameters.Target)
            //    || !(context.InputParameters[InputParameters.Target] is Entity))
            //    return false;
            return true;
        }


        public void UpdateEmailBodyWithHeadersandFooters()
        {
            trace.Trace("Begin - UpdateEmailBodyWithHeadersandFooters");
            if (!IsContextValid()) return;
                   
            Guid emailId =(Guid) context.InputParameters["EmailId"] ;
            Entity targetEmail = service.Retrieve("email", emailId, new ColumnSet("description"));
            trace.Trace("Entity : " +targetEmail.LogicalName.ToString());
            string emailDescription = targetEmail.GetAttributeValue<string>("description");
            trace.Trace(emailDescription.ToString());
            if (string.IsNullOrEmpty(emailDescription))
                throw new InvalidPluginExecutionException("the Email description is Empty");

            //{!EmailHeaderFooter: mark-up name}
            List<string> footerHeaderNamesList = new List<string>();
            footerHeaderNamesList = getFooterHeaderName(emailDescription, "{!EmailHeaderFooter:", "}");
             footerHeaderNamesList = footerHeaderNamesList.Distinct().ToList();
            trace.Trace("footerHeaderNamesList Count : "+footerHeaderNamesList.Count.ToString());
            if (footerHeaderNamesList != null && footerHeaderNamesList.Count > 0)
            {
                foreach (var footerHeaderName in footerHeaderNamesList)
                {
                    if (!string.IsNullOrWhiteSpace(footerHeaderName))
                    {
                        //QueryExpression to get the EmailHeaderFooter value

                        QueryExpression query = new QueryExpression
                        {
                            EntityName = "tc_configuration",
                            ColumnSet = new ColumnSet("tc_longvalue"),
                            Criteria =
                        {
                        FilterOperator = LogicalOperator.And,
                        Conditions =
                            {
                                new ConditionExpression
                                    {
                                        AttributeName = "tc_name",
                                        Operator = ConditionOperator.Equal,

                                        Values = { footerHeaderName }
                                     }
                            }

                            }
                        };

                        EntityCollection emailTempalteHeaderFooterValue = service.RetrieveMultiple(query);
                        if (emailTempalteHeaderFooterValue != null && emailTempalteHeaderFooterValue.Entities.Count > 0)
                        {
                            if (!string.IsNullOrWhiteSpace(emailTempalteHeaderFooterValue.Entities[0].Attributes["tc_longvalue"].ToString()))
                            {
                                string emailTemplateFooterHeaderValue = emailTempalteHeaderFooterValue.Entities[0].Attributes["tc_longvalue"].ToString();
                                trace.Trace(emailTemplateFooterHeaderValue);
                                emailDescription = emailDescription.Replace("{!EmailHeaderFooter:" + footerHeaderName + "}", emailTemplateFooterHeaderValue);
                            }
                        }

                    }
                }

                if ((!string.IsNullOrWhiteSpace(emailDescription)))
                {                  
                    targetEmail.Attributes["description"] = emailDescription;
                    targetEmail.Id = emailId;
                    trace.Trace("updated email body :" + emailDescription);
                    service.Update(targetEmail);
                }
            }

            trace.Trace("End - UpdateEmailBodyWithHeadersandFooters");
        }

        public static List<string> getFooterHeaderName(
            string text, string startString, string endString)
        {
            List<string> matched = new List<string>();
            int indexStart = 0, indexEnd = 0;
            bool exit = false;
            while (!exit)
            {
                indexStart = text.IndexOf(startString);
                indexEnd = text.IndexOf(endString);
                if (indexStart != -1 && indexEnd != -1)
                {
                    matched.Add(text.Substring(indexStart + startString.Length,
                        indexEnd - indexStart - startString.Length));
                    text = text.Substring(indexEnd + endString.Length);
                }
                else
                    exit = true;
            }
            return matched;
        }
    }
}
