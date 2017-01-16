using System;
using System.Collections.Generic;
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
                var payload = JsonWebTokenHelper.DecodePayloadToObject<JsonWebTokenPayload>(token);
                var customer = CustomerService.GetCustomerFromPayload(payload.Data);
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
                    return Request.CreateResponse(HttpStatusCode.InternalServerError,ex.Message);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
