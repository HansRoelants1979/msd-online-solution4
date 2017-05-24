using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.CTIIntegration.Client.Console.Models;

namespace Tc.Crm.CTIIntegration.Client.Console.Service
{
    public static class CTIIntegrationHelper
    {
        public static Guid checkSecurityRole(string securityRoleName, Guid userId,IOrganizationService crmService)
        {
            if (securityRoleName == null || securityRoleName == "")
            {
                System.Console.WriteLine("SecurityRoleName is null");
            }

            if (userId == Guid.Empty)
            {
                System.Console.WriteLine("userId is null");
            }

            Guid Roleid = Guid.Empty;
            var query = CrmServiceHelper.GetQuery(securityRoleName, userId);
            EntityCollection collRoles = crmService.RetrieveMultiple(query);
            if (collRoles != null && collRoles.Entities.Count > 0)
            {
                System.Console.WriteLine("The Non-Intaractive User Has Role");
                foreach (Entity _entity in collRoles.Entities)
                {
                    System.Console.WriteLine(_entity.Attributes["name"].ToString());
                    if (_entity.Attributes["name"].ToString().ToLower() == securityRoleName)
                    {

                        Roleid = new Guid(_entity.Attributes["roleid"].ToString());
                    }

                }
                System.Console.WriteLine("*****************************************************");

            }

            return Roleid;
        }

        public static Previlage GetPrevilageType(string entity, string accessType)
        {
            string entityName = string.Empty;
            string previlageType = string.Empty;

            switch (entity)
            {
                case "1":

                    switch (accessType)
                    {
                        case "1":
                            entityName = "Contact";
                            previlageType = "prvReadContact";
                            break;
                        case "2":
                            entityName = "Contact";
                            previlageType = "prvWriteContact";
                            break;
                        case "3":
                            entityName = "Contact";
                            previlageType = "prvCreateContact";
                            break;
                    }
                    break;
                case "2":

                    switch (accessType)
                    {
                        case "1":
                            entityName = "Account";
                            previlageType = "prvReadAccount";
                            break;
                        case "2":
                            entityName = "Account";
                            previlageType = "prvWriteAccount";
                            break;
                        case "3":
                            entityName = "Account";
                            previlageType = "prvCreateAccount";
                            break;

                    }
                    break;

                case "3":

                    switch (accessType)
                    {
                        case "1":
                            entityName = "Incident";
                            previlageType = "prvReadIncident";
                            break;
                        case "2":
                            entityName = "Incident";
                            previlageType = "prvWriteIncident";
                            break;
                        case "3":
                            entityName = "Incident";
                            previlageType = "prvCreateIncident";
                            break;

                    }
                    break;
            }

            return new Previlage
            {
                EntityName = entityName,
                PrevilageType = previlageType
            };
    }
    public static bool checkthePrevilage(Guid _userId, string previlageType,IOrganizationService crmService)
        {
            bool userHasPrivilege = false;

            ConditionExpression privilegeCondition =
                new ConditionExpression("name", ConditionOperator.Equal, previlageType); // name of the privilege
            FilterExpression privilegeFilter = new FilterExpression(LogicalOperator.And);
            privilegeFilter.Conditions.Add(privilegeCondition);

            QueryExpression privilegeQuery = new QueryExpression
            {
                EntityName = "privilege",
                ColumnSet = new ColumnSet(true),
                Criteria = privilegeFilter
            };

            EntityCollection retrievedPrivileges = crmService.RetrieveMultiple(privilegeQuery);
            if (retrievedPrivileges.Entities.Count == 1)
            {
                RetrieveUserPrivilegesRequest request = new RetrieveUserPrivilegesRequest();
                request.UserId = _userId; // Id of the User
                RetrieveUserPrivilegesResponse response = (RetrieveUserPrivilegesResponse)crmService.Execute(request);
                foreach (RolePrivilege rolePrivilege in response.RolePrivileges)
                {
                    if (rolePrivilege.PrivilegeId == retrievedPrivileges.Entities[0].Id)
                    {
                        userHasPrivilege = true;
                        break;
                    }
                }
            }
            return userHasPrivilege;
        }

    }
}
