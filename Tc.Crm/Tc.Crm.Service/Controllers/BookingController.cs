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
        public HttpResponseMessage Update()
        {
            try
            {
                var token = JwtHelper.GetToken(Request);
                var payload = JwtHelper.DecodePayloadToObject<JwtPayload>(token);
                var booking = BookingService.GetBookingFromPayload(payload.Data);
                try
                {
                    if(string.IsNullOrEmpty(booking.Id)) return Request.CreateResponse(HttpStatusCode.BadRequest,Constants.Messages.SOURCE_KEY_NOT_PRESENT);
                    var response = BookingService.Update(booking);
                    if (response.Created) return Request.CreateResponse(HttpStatusCode.Created,"<Guid>");
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError,ex.Message);
                }
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,ex.Message);
            }

        }
    }
}
