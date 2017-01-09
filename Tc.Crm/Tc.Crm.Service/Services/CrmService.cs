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

namespace Tc.Crm.Service.Services
{

    public class CrmService
    {
        private static HttpClient httpClient;
        private static IOrganizationService orgService;

        private static void ConnectToCRM(string key)
        {
            Microsoft.Crm.Sdk.Samples.HelperCode.Configuration config = null;

            config = new FileConfiguration(key);

            //Create a helper object to authenticate the user with this connection info.
            Authentication auth = new Authentication(config);
            //Next use a HttpClient object to connect to specified CRM Web service.
            httpClient = new HttpClient(auth.ClientHandler, true);
            //Define the Web API base address, the max period of execute time, the 
            // default OData version, and the default response payload format.
            httpClient.BaseAddress = new Uri(config.ServiceUrl + "api/data/v8.1/");
            httpClient.Timeout = new TimeSpan(0, 2, 0);
            httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }


        public static async Task<HttpResponseMessage> Create(JObject e,string uri)
        {
            if (e == null) throw new ArgumentNullException("entity");
            if (string.IsNullOrWhiteSpace(uri)) throw new ArgumentNullException("uri");

            var client = GetHttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = new StringContent(e.ToString(),Encoding.UTF8, "application/json");
            var response = await httpClient.SendAsync(request);
            return response;
        }

        public static Guid Create(Entity e)
        {
            var service = GetOrgService();
            return service.Create(e);
        }

        public static void Update(Entity e)
        {
            var service = GetOrgService();
            service.Update(e);
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

        public static string GetEntityUri(HttpResponseMessage response)
        {
            var uri = response.Headers.GetValues("OData-EntityId").FirstOrDefault();
            return uri;
        }

        public static async Task<HttpResponseMessage> Update(JObject e,string uri)
        {
            if (e == null) throw new ArgumentNullException("entity");
            if (string.IsNullOrWhiteSpace(uri)) throw new ArgumentNullException("uri");

            var request = new HttpRequestMessage(new HttpMethod("PATCH"), uri);
            request.Content = new StringContent(e.ToString(),Encoding.UTF8, "application/json");
            var response = await httpClient.SendAsync(request);
            return response;
        }

        public static async Task<HttpResponseMessage> Retrieve(string queryOptions, string uri)
        {
            var response = await httpClient.GetAsync(uri+ queryOptions);
            return response;
        }

        private static HttpClient GetHttpClient()
        {
            if (httpClient == null)
                ConnectToCRM("Crm");
            return httpClient;
        }
    }
}