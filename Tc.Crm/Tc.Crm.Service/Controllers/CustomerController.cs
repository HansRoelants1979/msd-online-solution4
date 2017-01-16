using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tc.Crm.Service.Filters;
using Tc.Crm.Service.Models;
using Tc.Crm.Service.Services;

namespace Tc.Crm.Service.Controllers
{
    [RequireHttps]
    public class CustomerController : ApiController
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        [Route("api/v1/customer/update")]
        [Route("api/customer/update")]
        [HttpPut]
        [JsonWebTokenAuthorize]
        public HttpResponseMessage Update()
        {
            try
            {
                var token = JsonWebTokenHelper.GetToken(Request);
                if (token == null)
                    throw new ArgumentNullException(Constants.Messages.TokenIsNull);
                var payload = JsonWebTokenHelper.DecodePayloadToObject<JsonWebTokenPayload>(token);
                if (payload == null)
                    throw new ArgumentNullException(Constants.Messages.PayloadIsNull);
                var customer = CustomerService.GetCustomerFromPayload(payload.Data);
                if (customer == null)
                    throw new ArgumentNullException(Constants.Messages.CustomerObjectIsNull);
                try
                {
                    if (string.IsNullOrEmpty(customer.Id))
                        return Request.CreateResponse(HttpStatusCode.BadRequest, Constants.Messages.SourceKeyNotPresent);

                    var response = CustomerService.Update(customer);
                    if(response.Created)
                        return Request.CreateResponse(HttpStatusCode.Created, response.Id);
                    else
                        return Request.CreateResponse(HttpStatusCode.NoContent, response.Id);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Unexpected error in Customer.Update::Message:{0}||Trace:{1}", ex.Message, ex.StackTrace);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError,ex.Message);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Unexpected error in Cstomer.Update::Message:{0}||Trace:{1}", ex.Message, ex.StackTrace);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
