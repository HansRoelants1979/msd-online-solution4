using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tc.Crm.Service.Filters;
using Tc.Crm.Service.Models;
using Tc.Crm.Service.Services;

namespace Tc.Crm.Service.Controllers
{
    public class CacheController : ApiController
    {
        ICachingService cachingService;
        IConfigurationService configurationService;

        public CacheController(ICachingService cachingService,IConfigurationService configurationService)
        {
            this.cachingService = cachingService;
            this.configurationService = configurationService;
        }

        [Route("api/v1/cache/refresh")]
        [Route("api/cache/refresh")]
        [RequireHttps]
        [HttpPost]
        public HttpResponseMessage Refresh(Payload payload)
        {
            if(payload == null)
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            Trace.TraceInformation($"{payload.Bucket},{payload.JWTToken}");
            HttpResponseMessage response = ValidateMessage(payload);
            if (response != null) return response;

            try
            {
                if(string.IsNullOrWhiteSpace(payload.Bucket))
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                cachingService.Cache(payload.Bucket);

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Unexpected error Booking.Update::Message:{0}||Trace:{1}", ex.Message, ex.StackTrace.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        private HttpResponseMessage ValidateMessage(Payload payload)
        {
            JsonWebTokenHelper helper = new JsonWebTokenHelper(this.configurationService);
            var request = helper.GetRequestObject(payload);
            if (request.Errors != null && request.Errors.Count > 0)
            {
                Trace.TraceWarning("Bad Request Header: Error while parsing the request object");
                return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
                {
                    ReasonPhrase = Constants.Messages.JsonWebTokenParserError
                };
            }
            //check token validation flags
            if (!request.HeaderAlgorithmValid
                || !request.HeaderTypeValid
                || !request.IssuedAtTimeValid
                || !request.NotBeforetimeValid
                || !request.SignatureValid
                || !request.ExpiryValid)
            {
                Trace.TraceWarning("Bad Request: One or more information is missing in the token or signature didn't match.");
                Trace.TraceWarning("Type:{0},Algorithm:{1},IatValid:{2},NbfValid:{3},SignValid:{4},Expiry:{5}"
                                     , request.HeaderTypeValid
                                     , request.HeaderAlgorithmValid
                                     , request.IssuedAtTimeValid
                                     , request.NotBeforetimeValid
                                     , request.SignatureValid
                                     , request.ExpiryValid);
                return new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                {
                    ReasonPhrase = Constants.Messages.JsonWebTokenExpiredOrNoMatch
                };

            }
            return null;
        }
    }
}
