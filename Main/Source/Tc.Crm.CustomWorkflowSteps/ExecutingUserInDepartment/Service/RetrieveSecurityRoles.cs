using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

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

            if (securityRoleName == null || securityRoleName == "")
            {
                trace.Trace("SecurityRoleName is null");
                throw new InvalidPluginExecutionException("SecurityRoleName is null");
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

            query.EntityName = EntityName.Role; //role entity name
            ColumnSet cols = new ColumnSet();
            cols.AddColumn(Attributes.Role.Name); //We only need role name
            query.ColumnSet = cols;

            ConditionExpression systemUserIdCondition = new ConditionExpression();
            systemUserIdCondition.AttributeName = Attributes.Role.SystemUserId;
            systemUserIdCondition.Operator = ConditionOperator.Equal;
            systemUserIdCondition.Values.Add(userId);

            ConditionExpression securityRoleNameCondition = new ConditionExpression();
            securityRoleNameCondition.AttributeName = Attributes.Role.Name;
            securityRoleNameCondition.Operator = ConditionOperator.Equal;
            securityRoleNameCondition.Values.Add(securityRoleName);

            //system roles

            LinkEntity linkRole = new LinkEntity();
            linkRole.LinkFromAttributeName = Attributes.Role.RoleId;
            linkRole.LinkFromEntityName = EntityName.Role; //FROM
            linkRole.LinkToEntityName = EntityName.SystemUserRoles;
            linkRole.LinkToAttributeName = Attributes.SystemUserRoles.RoleId;

            //system users

            LinkEntity linkSystemusers = new LinkEntity();
            linkSystemusers.LinkFromEntityName = EntityName.SystemUserRoles;
            linkSystemusers.LinkFromAttributeName = Attributes.SystemUserRoles.SystemUserId;
            linkSystemusers.LinkToEntityName = EntityName.SystemUser;
            linkSystemusers.LinkToAttributeName = Attributes.SystemUser.SystemUserId;

            linkSystemusers.LinkCriteria = new FilterExpression();
            linkSystemusers.LinkCriteria.Conditions.Add(systemUserIdCondition);

            linkRole.LinkEntities.Add(linkSystemusers);
            query.LinkEntities.Add(linkRole);
            query.Criteria.Conditions.Add(securityRoleNameCondition);

            EntityCollection collRoles = service.RetrieveMultiple(query);

            if (collRoles != null && collRoles.Entities.Count > 0)
            {

                foreach (Entity entity in collRoles.Entities)
                {

                    if (entity.Contains(Attributes.Role.Name)&& entity.Attributes[Attributes.Role.Name].ToString().ToLower() == securityRoleName)
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
