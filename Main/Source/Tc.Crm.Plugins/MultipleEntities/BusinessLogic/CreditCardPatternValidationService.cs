using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Tc.Crm.Plugins.MultipleEntities.BusinessLogic
{
    
    public abstract class CreditCardPatternValidationService
    {
        
        private IOrganizationService service = null;
        private ITracingService trace = null;
        private string creditCardPattern = null;

        protected CreditCardPatternValidationService()
        {
           
        }

        protected CreditCardPatternValidationService(ITracingService trace, IOrganizationService service)
        {
            this.service = service;
            this.trace = trace;
        }


        /// <summary>
        ///  To validate target entity
        /// </summary>
        /// <param name="context"></param>
        public void ValidateEntity(IPluginExecutionContext context)
        {
            trace.Trace("ValidateEntity - Start");            
            if (context.InputParameters.Contains(InputParameters.Target) && context.InputParameters[InputParameters.Target] is Entity)
            {
                trace.Trace("Contains Input Parameters 'Target' as Entity");
                var entity = context.InputParameters[InputParameters.Target] as Entity;
                creditCardPattern = GetCreditCardPattern();
                if (!string.IsNullOrWhiteSpace(creditCardPattern))
                    ValidateContent(entity);
            }
            trace.Trace("ValidateEntity - End");
        }

        /// <summary>
        /// To validate all string type of attributes in entity
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void ValidateContent(Entity entity)
        {
            trace.Trace("ValidateContent - Start");
            if (entity == null || entity.Attributes == null || entity.Attributes.Count == 0) return;
            IList<string> attributeValueList = new List<string>();
            foreach (KeyValuePair<string, object> attribute in entity.Attributes)
            {
                if (entity.Attributes[attribute.Key] == null || entity.Attributes[attribute.Key].GetType() == null) continue;
                var attributeType = entity.Attributes[attribute.Key].GetType();
                if (attributeType == typeof(string))
                    attributeValueList.Add(attribute.Value.ToString());
            }
            ValidateCreditCardPattern(attributeValueList);
            trace.Trace("ValidateContent - End");
        }

        /// <summary>
        /// To get values of attributes from entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="listOfAttributes"></param>
        /// <returns></returns>
        protected IList<string> GetAttributeValueList(Entity entity, IList<string> listOfAttributes)
        {
            trace.Trace("GetAttributeValueList - Start");
            IList<string> attributeValueList = new List<string>();
            if (entity == null || entity.Attributes.Count == 0) return attributeValueList;
            foreach (var attribute in listOfAttributes)
            {
                if (entity.Attributes.Contains(attribute) && entity.Attributes[attribute] != null)
                    attributeValueList.Add(entity.Attributes[attribute].ToString());
            }
            trace.Trace("GetAttributeValueList - End");
            return attributeValueList;
        }



        /// <summary>
        /// To validate content and to throw exception when it has credit card pattern 
        /// </summary>
        /// <param name="content"></param>
        protected void ValidateCreditCardPattern(IEnumerable<string> attributeValueList)
        {
            trace.Trace("ValidateCreditCardPattern - Start");
            var hasCreditCard = HasCreditCardNumber(attributeValueList);
            if (hasCreditCard)
                throw new InvalidPluginExecutionException(ValidationMessages.DataYouEnteredInNotesContainPotentionalCreditCardNumber);
            trace.Trace("ValidateCreditCardPattern - End");
        }

        /// <summary>
        /// To get credit card pattern
        /// </summary>
        /// <returns></returns>
        private string GetCreditCardPattern()
        {
            trace.Trace("GetCreditCardPattern - Start");
            var query = GetQueryToRetrieveCreditCardPattern();
            EntityCollection configurations = service.RetrieveMultiple(query);
            trace.Trace("GetCreditCardPattern - End");
            if (configurations.Entities.Count > 0)
                return (configurations.Entities[0].Attributes.Contains(Attributes.Configuration.Value) && configurations.Entities[0].Attributes[Attributes.Configuration.Value] != null) ? configurations.Entities[0].GetAttributeValue<string>(Attributes.Configuration.Value) : string.Empty;
            else
                return string.Empty;
        }

        /// <summary>
        /// To check whether the content contains credit card pattern number or not
        /// </summary>
        /// <param name="attributeValueList"></param>
        /// <returns></returns>
        private bool HasCreditCardNumber(IEnumerable<string> attributeValueList)
        {
            trace.Trace("HasCreditCardNumber - Start");
            if (!string.IsNullOrWhiteSpace(creditCardPattern))
            {
                var attributes = attributeValueList.GetEnumerator();
                while (attributes.MoveNext())
                {
                    if (string.IsNullOrWhiteSpace(attributes.Current)) continue;
                    MatchCollection matches = Regex.Matches(attributes.Current, creditCardPattern, RegexOptions.Multiline);
                    if (matches.Count > 0)
                        return true;
                }
            }
            trace.Trace("HasCreditCardNumber - End");
            return false;
        }

        /// <summary>
        /// To get queryexpression to retrieve credit card pattern
        /// </summary>
        /// <returns></returns>
        private QueryExpression GetQueryToRetrieveCreditCardPattern()
        {
            trace.Trace("GetQueryToRetrieveCreditCardPattern - Start");
            QueryExpression query = new QueryExpression
            {
                EntityName = Entities.Configuration,
                ColumnSet = new ColumnSet(Attributes.Configuration.Configurationid, Attributes.Configuration.Value),
                Criteria =
                        {
                        FilterOperator = LogicalOperator.And,
                        Conditions =
                            {
                                new ConditionExpression
                                    {
                                        AttributeName = Attributes.Configuration.Name,
                                        Operator = ConditionOperator.Equal,
                                        Values = { Configurationkeys.CreditCardPattern }
                                    }
                            }
                      }
            };
            trace.Trace("GetQueryToRetrieveCreditCardPattern - End");
            return query;
        }
    }
}
