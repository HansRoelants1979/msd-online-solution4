using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Configuration;

namespace Tc.Crm.CTIIntegration.Client.Console.Service
{
    public static class CrmServiceHelper
    {
        public static IOrganizationService InitializeOrganizationService()
        {
            IOrganizationService crmService;
            string connectionString = ConfigurationManager.ConnectionStrings["MyCRMServer"].ConnectionString.ToString();
            crmService = new CrmServiceClient(connectionString);             
            return crmService;
        }
        public static void checkUserLogin(IOrganizationService crmService)
        {
            try
            {
                crmService.Execute(new WhoAmIRequest());
                System.Console.WriteLine("Non-interative User Successfully login to CRM");
                System.Console.WriteLine("*****************************************************");

            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Non-interative User failed to login");
                System.Console.WriteLine("*****************************************************");
                throw ex;
            }

        }
        public static QueryExpression GetQuery(string securityRoleName, Guid userId)
        {
            QueryExpression query = new QueryExpression();
            query.EntityName = "role";
            ColumnSet cols = new ColumnSet();
            cols.AddColumn("name");
            cols.AddColumn("roleid");
            query.ColumnSet = cols;
            ConditionExpression Condition1 = new ConditionExpression();
            Condition1.AttributeName = "systemuserid";
            Condition1.Operator = ConditionOperator.Equal;
            Condition1.Values.Add(userId);
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
            return query;

        }
    }
}
