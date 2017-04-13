using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.CustomWorkflowSteps.ExecutingUserInDepartment.Service
{
     class RetrieveSecurityRoles
    {
        public static bool GetSecurityRoles(string securityRoleName,Guid userId,IOrganizationService service)
        {
            var response = false;

            QueryExpression query = new QueryExpression();

            query.EntityName = "role"; //role entity name

            ColumnSet cols = new ColumnSet();

            cols.AddColumn("name"); //We only need role name

            query.ColumnSet = cols;

            ConditionExpression ce = new ConditionExpression();

            ce.AttributeName = "systemuserid";

            ce.Operator = ConditionOperator.Equal;

            ce.Values.Add(userId);

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

            linkSystemusers.LinkCriteria.Conditions.Add(ce);

            linkRole.LinkEntities.Add(linkSystemusers);

            query.LinkEntities.Add(linkRole);

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


            return response;
        }
    }
}
