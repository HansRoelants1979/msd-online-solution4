using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.Plugins.CacheRequest.BusinessLogic
{
    public class CachingApiService:ICachingApiService
    {
        public HttpResponseMessage SendRequest(string requestData
                                , Dictionary<string, string> cachingServiceParameters
                                , ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException(ValidationMessages.TraceIsNull);
            if (cachingServiceParameters == null) throw new InvalidPluginExecutionException(ValidationMessages.CachingParameterIsNull);

            if (string.IsNullOrWhiteSpace(requestData)) throw new InvalidPluginExecutionException(ValidationMessages.RequestDataIsEmpty);

            if (!cachingServiceParameters.ContainsKey(CachingParameter.ServiceUrl) || cachingServiceParameters[CachingParameter.ServiceUrl] == null)
                throw new InvalidPluginExecutionException(ValidationMessages.CachingServiceUrlIsNullOrEmpty);

            if (!cachingServiceParameters.ContainsKey(CachingParameter.Api) || cachingServiceParameters[CachingParameter.Api] == null)
                throw new InvalidPluginExecutionException(ValidationMessages.CachingApiIsNullOrEmpty);

            trace.Trace("Start - CachingApiService.SendRequest");

            var url = cachingServiceParameters[CachingParameter.ServiceUrl];
            var api = cachingServiceParameters[CachingParameter.Api];
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            Task<HttpResponseMessage> t = client.PostAsync(api, new StringContent(requestData, Encoding.UTF8, "application/json"));

            var response = t.Result;
            trace.Trace("End - CachingApiService.SendRequest");

            return response;

        }
    }
}
