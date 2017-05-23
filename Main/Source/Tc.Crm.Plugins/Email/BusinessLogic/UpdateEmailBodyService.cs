using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Tc.Crm.Plugins.Email.BusinessLogic
{
    public class UpdateEmailBodyService
    {

        public IPluginExecutionContext context = null;
        public IOrganizationService service = null;
        public ITracingService trace = null;

        public UpdateEmailBodyService(IPluginExecutionContext context, ITracingService trace, IOrganizationService service)
        {
            this.context = context;
            this.service = service;
            this.trace = trace;
        }

        private bool IsContextValid()
        {
            if (!context.MessageName.Equals("create", StringComparison.OrdinalIgnoreCase)) return false;
            if (context.Stage != (int)PluginStage.Preoperation) return false;
            if (!context.InputParameters.Contains(InputParameters.Target)
                || !(context.InputParameters[InputParameters.Target] is Entity))
                return false;
            return true;
        }


        public void UpdateEmailBodyWithHeadersandFooters()
        {
            trace.Trace("Begin - UpdateEmailBodyWithHeadersandFooters");
            if (!IsContextValid()) return;
            Guid emailId = Guid.Empty;
            string emailDescription = string.Empty;
            Entity targetEmail = null;           
            targetEmail = (Entity)context.InputParameters["Target"];
            if (targetEmail.Id != Guid.Empty)
                emailId = targetEmail.Id;            
            if ((bool)targetEmail["directioncode"] == false)
                return;
            targetEmail["description"] = GetEmailDescription(targetEmail["description"].ToString());
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
        public string GetEmailDescription(string body)
        {
            string emailDescription = string.Empty;
            emailDescription = body;
            List<string> footerHeaderNamesList = new List<string>();
            footerHeaderNamesList = getFooterHeaderName(emailDescription, "{!EmailHeaderFooter:", "}");
            footerHeaderNamesList = footerHeaderNamesList.Distinct().ToList();
            trace.Trace("footerHeaderNamesList Count : " + footerHeaderNamesList.Count.ToString());
            if (footerHeaderNamesList == null || footerHeaderNamesList.Count == 0)
                return body;

            foreach (var footerHeaderName in footerHeaderNamesList)
            {
                if (string.IsNullOrWhiteSpace(footerHeaderName))
                    continue;
                var query = GetQuery(footerHeaderName);

                EntityCollection emailTempalteHeaderFooterValue = service.RetrieveMultiple(query);
                if (emailTempalteHeaderFooterValue == null || emailTempalteHeaderFooterValue.Entities.Count == 0)
                    continue;
                if (string.IsNullOrWhiteSpace(emailTempalteHeaderFooterValue.Entities[0].Attributes["tc_longvalue"].ToString()))
                    continue;

                string emailTemplateFooterHeaderValue = emailTempalteHeaderFooterValue.Entities[0].Attributes["tc_longvalue"].ToString();
                trace.Trace(emailTemplateFooterHeaderValue);
                emailDescription = emailDescription.Replace("{!EmailHeaderFooter:" + footerHeaderName + "}", emailTemplateFooterHeaderValue);
            }
            if (string.IsNullOrWhiteSpace(emailDescription))
                return body;
            return emailDescription;
        }
        private QueryExpression GetQuery(string footerHeaderName)
        {
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
            return query;
        }
    }
}
