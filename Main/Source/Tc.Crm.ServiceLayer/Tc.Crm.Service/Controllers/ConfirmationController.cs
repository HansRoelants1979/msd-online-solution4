using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tc.Crm.Service.Filters;
using Tc.Crm.Service.Services;
using System.Diagnostics;
using Tc.Crm.Service.Models;
using Tc.Crm.Service.Constants;

namespace Tc.Crm.Service.Controllers
{
    [RequireHttps]
    public class ConfirmationController : ApiController
    {
        IConfirmationService confirmationService;        
        public ConfirmationController(IConfirmationService confirmationService)
        {
            this.confirmationService = confirmationService;            
        }


        [Route("api/v1/confirmations/{msDCorrelationId}")]
        [Route("api/confirmations/{msDCorrelationId}")]
        [HttpPut]
        [JsonWebTokenAuthorize]
        public HttpResponseMessage Confirmations(string msDCorrelationId, IntegrationLayerResponse ilResponse)
        {
            try
            {
				Guid entityCacheMessageId;
				if (string.IsNullOrWhiteSpace(msDCorrelationId))
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.MissingMsdCorrelationId);
				if (!Guid.TryParse(msDCorrelationId, out entityCacheMessageId))
					return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.IncorrectMsdCorrelationId);
				if (ilResponse == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.ConfirmationDataPassedIsNullOrCouldNotBeParsed);
                if(string.IsNullOrWhiteSpace(ilResponse.CorrelationId))
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Messages.MissingCorrelationId);

                var confirmationResponse = confirmationService.ProcessResponse(entityCacheMessageId, ilResponse);
                return Request.CreateResponse(confirmationResponse.StatusCode, confirmationResponse.Message);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Unexpected error Survey.Create::Message:{0}||Trace:{1}", ex.Message, ex.StackTrace.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, Messages.UnexpectedError);
            }

        }
       
    }
    
}