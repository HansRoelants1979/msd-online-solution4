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
            var t = JwtHelper.GetToken(Request);
            var p = JwtHelper.DecodePayloadToObject<JwtPayload>(t);
            var c = BookingService.GetBookingFromPayload(p.Data);

            return StatusCode(HttpStatusCode.NoContent);


        }

        [Route("api/v1/booking/create")]
        [Route("api/booking/create")]
        [HttpPut]
        [JwtAuthorize]
        public IHttpActionResult Create()
        {
            var t = JwtHelper.GetToken(Request);
            var p = JwtHelper.DecodePayloadToObject<JwtPayload>(t);
            var c = BookingService.GetBookingFromPayload(p.Data);

            return Ok(c);
        }
    }
}
