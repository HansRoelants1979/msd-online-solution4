using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Diagnostics;
using Tc.Crm.Service.Services;

namespace Tc.Crm.Service.Controllers
{
    public class PingController : ApiController
    {
        ICrmService crmService;

        public PingController(ICrmService crmService)
        {
            this.crmService = crmService;
        }
        
        [Route("api/v1/healthcheck")]
        [Route("api/healthcheck")]
        [HttpPost]
        public HttpResponseMessage Ping()
        {
            try
            {
                bool isCrmAvailable = crmService.PingCRM();
                return Request.CreateResponse(isCrmAvailable ? HttpStatusCode.OK : HttpStatusCode.ServiceUnavailable);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Unexpected error Ping::Message:{0}||Trace:{1}", ex.Message, ex.StackTrace.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
    }
}
