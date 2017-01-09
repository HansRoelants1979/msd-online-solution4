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
        public IHttpActionResult Update()
        {
            try
            {
                var t = JwtHelper.GetToken(Request);
                var p = JwtHelper.DecodePayloadToObject<JwtPayload>(t);
                var c = CustomerService.GetCustomerFromPayload(p.Data);
                try
                {
                    var response = CustomerService.Update(c);
                    if(response.Created) return StatusCode(HttpStatusCode.Created);
                }
                catch (Exception)
                {
                    return StatusCode(HttpStatusCode.InternalServerError);
                }
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }

        [Route("api/v1/customer/create")]
        [Route("api/customer/create")]
        [HttpPut]
        [JwtAuthorize]
        public IHttpActionResult Create()
        {
            try
            {
                var t = JwtHelper.GetToken(Request);
                var p = JwtHelper.DecodePayloadToObject<JwtPayload>(t);
                var c = CustomerService.GetCustomerFromPayload(p.Data);
                try
                {
                    CustomerService.Create(c);
                }
                catch (Exception)
                {
                    return StatusCode(HttpStatusCode.InternalServerError);
                }
                return Ok(c);
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }


    }
}
