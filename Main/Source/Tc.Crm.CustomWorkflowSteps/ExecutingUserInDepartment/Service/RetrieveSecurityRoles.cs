using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.CustomWorkflowSteps.ExecutingUserInDepartment.Service
{
     public static class RetrieveSecurityRoles
    {
        public static bool GetSecurityRoles(string securityRoleName, Guid userId, IOrganizationService service, ITracingService trace)
        {
            if (trace == null)
            {
                throw new InvalidPluginExecutionException("trace is null");
            }

            
            if (userId == Guid.Empty)
            {
                trace.Trace("userId is null");
                throw new InvalidPluginExecutionException("userId is null");
            }

            if (service == null)
            {
                trace.Trace("service is null");
                throw new InvalidPluginExecutionException("service is null");
            }

           

            trace.Trace("GetSecurityRoles - Start ");
            var response = false;

            QueryExpression query = new QueryExpression();

            query.EntityName = "role"; //role entity name

            ColumnSet cols = new ColumnSet();

            cols.AddColumn("name"); //We only need role name

            query.ColumnSet = cols;

            ConditionExpression Condition1 = new ConditionExpression();

            Condition1.AttributeName = "systemuserid";

            Condition1.Operator = ConditionOperator.Equal;

            Condition1.Values.Add(userId);

            ConditionExpression Condition2 = new ConditionExpression();

            Condition2.AttributeName = "name";

            Condition2.Operator = ConditionOperator.Equal;

            Condition2.Values.Add(securityRoleName);

            //system roles

            LinkEntity linkRole = new LinkEntity();

            linkRole.LinkFromAttributeName = "roleid";

            linkRole.LinkFromEntityName = "role"; //FROM

            linkRole.LinkToEntityName = "systemuserroles";

            linkRole.LinkToAttributeName = "roleid";

            //system users

            LinkEntity linkSystemusers = new LinkEntity();

            linkSystemusers.LinkFromEntityName = "systemuserroles";

            linkSystemusers.LinkFromAttributeName = "systemuserid";

            linkSystemusers.LinkToEntityName = "systemuser";

            linkSystemusers.LinkToAttributeName = "systemuserid";

            linkSystemusers.LinkCriteria = new FilterExpression();

            linkSystemusers.LinkCriteria.Conditions.Add(Condition1);

            linkRole.LinkEntities.Add(linkSystemusers);

            query.LinkEntities.Add(linkRole);

            query.Criteria.Conditions.Add(Condition2);

            EntityCollection collRoles = service.RetrieveMultiple(query);

            if (collRoles != null && collRoles.Entities.Count > 0)
            {

                foreach (Entity _entity in collRoles.Entities)
                {

                    if (_entity.Attributes["name"].ToString().ToLower() == securityRoleName)
                    {
                        return response = true;
                    }

                }

            }
            
            trace.Trace("GetSecurityRoles - End ");

            return response;
        }
    }
}
