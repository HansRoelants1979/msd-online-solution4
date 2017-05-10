using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.CustomWorkflowSteps.RetrieveConfigurationEntity.Service
{
    public static class RetrieveConfigurationEntityValue
    {
        public static string RetrieveValue(string Name, ITracingService trace, IOrganizationService service)
        {
            string Value = string.Empty;
            if (trace == null)
            {
                throw new InvalidPluginExecutionException("trace is null");
            }
            if (service == null)
            {
                throw new InvalidPluginExecutionException("service is null");
            }
            trace.Trace("RetrieveValue - Start");
            var query = GetQuery(Name);
            EntityCollection ConfigurationEntityValue = service.RetrieveMultiple(query);
            if (ConfigurationEntityValue == null || ConfigurationEntityValue.Entities.Count == 0)
                return Value;
            if (string.IsNullOrWhiteSpace(ConfigurationEntityValue.Entities[0].Attributes["tc_value"].ToString()))
                return Value;
            var ConfigValue = ConfigurationEntityValue.Entities[0].Attributes["tc_value"].ToString();
            if (string.IsNullOrWhiteSpace(ConfigValue))
                return Value;
            trace.Trace("RetrieveValue - End");
            return Value = ConfigValue;
        }
        public static QueryExpression GetQuery(string Name)
        {
            QueryExpression query = new QueryExpression
            {
                EntityName = "tc_configuration",
                ColumnSet = new ColumnSet("tc_value"),
                Criteria =
                        {
                        FilterOperator = LogicalOperator.And,
                        Conditions =
                            {
                                new ConditionExpression
                                    {
                                        AttributeName = "tc_name",
                                        Operator = ConditionOperator.Equal,
                                        Values = { Name }
                                }
                                }

                            }
            };
            return query;
        }

    }
}
