using Microsoft.Crm.Sdk.Samples.HelperCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
//using Microsoft.Xrm.Tooling.Connector;
using System.Configuration;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk.Messages;

namespace Tc.Crm.Service.Services
{

    public class CrmService
    {
        private static IOrganizationService orgService;
        
        public static UpsertResponse Upsert(Entity entity)
        {
            var service = GetOrgService();
            UpsertRequest request = new UpsertRequest
            {
                Target = entity
            };
            return service.Execute<UpsertResponse>(request);
        }

        public static Guid? RetrieveBy(string sourceKey,string sourceKeyValue,string name,string primaryKey)
        {
            var service = GetOrgService();
            QueryExpression q = new QueryExpression(name)
            {
                ColumnSet = new ColumnSet(primaryKey),
                Criteria = new FilterExpression(LogicalOperator.And)
            };
            q.Criteria.AddCondition(sourceKey,ConditionOperator.Equal,sourceKeyValue);
            var response = service.RetrieveMultiple(q);

            if (response == null || response.Entities.Count == 0) return null;
            if (response.Entities.Count > 1) throw new InvalidOperationException("Multiple records exist for the source key.");

            return response.Entities[0].Id;
        }

        public static IOrganizationService GetOrgService()
        {
            if (orgService == null)
            {
                var c = ConfigurationManager.ConnectionStrings["Crm"];
                CrmConnection crmConnection = CrmConnection.Parse(c.ConnectionString);


                orgService  = new OrganizationService(crmConnection);
            }

            return orgService;
        }

                
    }
}