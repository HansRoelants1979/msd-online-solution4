using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Net.Http;

namespace Tc.Crm.Plugins.CacheRequest.BusinessLogic
{
    public interface ICachingApiService
    {
        HttpResponseMessage SendRequest(string requestData, Dictionary<string, string> cachingServiceParameters, ITracingService trace);
    }
}
