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
        [Route("api/v1/customer/update")]
        [Route("api/customer/update")]
        [HttpPut]
        [JwtAuthorize]
        public HttpResponseMessage Update()
        {
            try
            {
                var token = JwtHelper.GetToken(Request);
                var payload = JwtHelper.DecodePayloadToObject<JwtPayload>(token);
                var customer = CustomerService.GetCustomerFromPayload(payload.Data);
                try
                {
                    if (string.IsNullOrEmpty(customer.Id)) return Request.CreateResponse(HttpStatusCode.BadRequest, Constants.Messages.SOURCE_KEY_NOT_PRESENT);

                    var response = CustomerService.Update(customer);
                    if(response.Created) return Request.CreateResponse(HttpStatusCode.Created, "<GUID>");
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError,ex.Message);
                }
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
