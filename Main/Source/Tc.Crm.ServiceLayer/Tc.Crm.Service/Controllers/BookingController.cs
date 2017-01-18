using Tc.Crm.Service.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tc.Crm.Service.Filters;
using System;
using Tc.Crm.Service.Services;
using System.Diagnostics;

namespace Tc.Crm.Service.Controllers
{
    [RequireHttps]
    public class BookingController : ApiController
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        [Route("api/v1/booking/update")]
        [Route("api/booking/update")]
        [HttpPut]
        [JsonWebTokenAuthorize]
        public HttpResponseMessage Update(Booking booking)
        {
            try
            {
                if(booking == null)
                    throw new ArgumentNullException(Constants.Parameters.Booking);
                try
                {
                    if(string.IsNullOrEmpty(booking.Id)) return Request.CreateResponse(HttpStatusCode.BadRequest,Constants.Messages.SourceKeyNotPresent);
                    var response = BookingService.Update(booking);
                    if (response.Created)
                        return Request.CreateResponse(HttpStatusCode.Created,response.Id);
                    else
                        return Request.CreateResponse(HttpStatusCode.NoContent, response.Id);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Unexpected error in Booking.Update::Message:{0}||Trace:{1}", ex.Message, ex.StackTrace);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError,ex.Message);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Unexpected error Booking.Update::Message:{0}||Trace:{1}",ex.Message,ex.StackTrace);
                return Request.CreateResponse(HttpStatusCode.InternalServerError,ex.Message);
            }

        }
    }
}
