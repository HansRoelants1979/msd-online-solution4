using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Net.Http;

namespace Tc.Crm.Plugins.CacheRequest.BusinessLogic
{
    public interface ICachingApiService
    {
        HttpResponseMessage SendRequest(string requestData, string url, string api, ITracingService trace);
    }
}
