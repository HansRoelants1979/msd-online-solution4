using Tc.Crm.Service.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tc.Crm.Service.Filters;
using System;
using Tc.Crm.Service.Services;

namespace Tc.Crm.Service.Controllers
{
    [RequireHttps]
    public class BookingController : ApiController
    {
        [Route("api/v1/booking/update")]
        [Route("api/booking/update")]
        [HttpPut]
        [JwtAuthorize]

        public IHttpActionResult Update()
        {
            try
            {
                var t = JwtHelper.GetToken(Request);
                var p = JwtHelper.DecodePayloadToObject<JwtPayload>(t);
                var b = BookingService.GetBookingFromPayload(p.Data);
                try
                {
                    if(string.IsNullOrEmpty(b.Id)) return StatusCode(HttpStatusCode.BadRequest);
                    var response = BookingService.Update(b);
                    if (response.Created) return StatusCode(HttpStatusCode.Created);
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
    }
}
