using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.ServiceModel;
using System.Text.RegularExpressions;

namespace Tc.Crm.Plugins.Note
{
    public class PostNoteUpdateCreditCardPatternValidation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            try
            {
                if (context.PrimaryEntityName != Entities.Annotation) return;
                if ((!context.MessageName.Equals(Messages.Create, StringComparison.OrdinalIgnoreCase)) && (!context.MessageName.Equals(Messages.Update, StringComparison.OrdinalIgnoreCase))) return;

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity Notes = context.InputParameters["Target"] as Entity;
                    if (Notes != null)
                    {
                        var ccNumber = Notes.Attributes.Contains(Attributes.Annotation.NoteText) ? Notes.GetAttributeValue<string>(Attributes.Annotation.NoteText) : null;
                        if (ccNumber != null)
                        {
                            var hasCreditCard = HasCreditCardNumber(ccNumber, service);
                            if (hasCreditCard)
                            {
                                throw new InvalidPluginExecutionException(ValidationMessages.DataYouEnteredInNotesContainPotentionalCreditCardNumber);
                            }
                        }
                    }
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new InvalidPluginExecutionException(ex.ToString());
            }
            catch (TimeoutException ex)
            {
                throw new InvalidPluginExecutionException(ex.ToString());
            }
        }
        
        public static bool HasCreditCardNumber(string ccNumber, IOrganizationService service)
        {
            var query = GetQuery();

            EntityCollection configurations = service.RetrieveMultiple(query);

            if (configurations.Entities.Count > 0)
            {
                string CreditCardPattern = configurations.Entities[0].Attributes.Contains(Attributes.Configuration.Value) ? configurations.Entities[0].GetAttributeValue<string>(Attributes.Configuration.Value) : "";
                if (CreditCardPattern != null && CreditCardPattern != "")
                {
                    MatchCollection matches = Regex.Matches(ccNumber, CreditCardPattern, RegexOptions.Multiline);
                    if (matches.Count > 0)
                        return true;
                }
            }
            return false;
        }

        private static QueryExpression GetQuery()
        {
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
            return query;
        }

    }
}
