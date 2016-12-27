using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Microsoft.Crm.Sdk.Samples.HelperCode;
using System.Net;
using System.Text;

namespace Tc.Crm.Service.BusinessServices.CRM
{
    public class CrmDataService : IDataService
    {
        public  async Task<Guid> Create(string uri, JObject obj)
        {
            //TODO:instantiate CRM http client
            HttpClient httpClient = null;
            JObject jsonBooking = obj;
            HttpRequestMessage createRequest =
                new HttpRequestMessage(HttpMethod.Post, uri);
            createRequest.Content = new StringContent(jsonBooking.ToString(),
                Encoding.UTF8, "application/json");
            HttpResponseMessage createResponse =
                await httpClient.SendAsync(createRequest);
            if (createResponse.StatusCode == HttpStatusCode.NoContent)  //204
            {
                return Guid.NewGuid();//TODO;
            }
            else
            {
                throw new CrmHttpResponseException(createResponse.Content);
            }
        }

        public  async Task Fetch(string uri, string queryOptions)
        {
            throw new NotImplementedException();
        }

        public  async Task<bool> Update(string uri, JObject obj)
        {
            //TODO:instantiate CRM http client
            HttpClient httpClient = null;
            JObject jsonBooking = obj;
            HttpRequestMessage updateRequest = new HttpRequestMessage(
                new HttpMethod("PATCH"), uri); 

            updateRequest.Content = new StringContent(obj.ToString(),
                Encoding.UTF8, "application/json");
            HttpResponseMessage createResponse =
                await httpClient.SendAsync(updateRequest);
            if (createResponse.StatusCode == HttpStatusCode.NoContent)  //204
            {
                return true;
            }
            else
            {
                throw new CrmHttpResponseException(updateRequest.Content);
            }
        }

}
}