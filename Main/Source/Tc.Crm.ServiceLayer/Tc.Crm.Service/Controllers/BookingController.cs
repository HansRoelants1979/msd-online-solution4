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
using Newtonsoft.Json;

namespace Tc.Crm.Service.Controllers
{
    [RequireHttps]
    public class BookingController : ApiController
    {
        IBookingService bookingService;
        ICrmService crmService;
        public BookingController(IBookingService bookingService, ICrmService crmService)
        {
            this.bookingService = bookingService;
            this.crmService = crmService;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        [Route("api/v1/booking/update")]
        [Route("api/booking/update")]
        [HttpPut]
        [JsonWebTokenAuthorize]
        public HttpResponseMessage Update(Booking booking)
        {
            try
            {
                if (booking == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Constants.Messages.BookingDataPassedIsNullOrCouldNotBeParsed);
                }
                try
                {
                    var jsonData = JsonConvert.SerializeObject(booking);
                    if(!bookingService.DataValid(jsonData))
                        return Request.CreateResponse(HttpStatusCode.BadRequest, Constants.Messages.BookingDataDoesNotComplyToSchema);
                    var response = bookingService.Update(jsonData, crmService);
                    if (response.Created)
                        return Request.CreateResponse(HttpStatusCode.Created, response.Id);
                    else
                        return Request.CreateResponse(HttpStatusCode.NoContent, response.Id);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Unexpected error in Booking.Update::Message:{0}||Trace:{1}", ex.Message, ex.StackTrace.ToString());
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Unexpected error Booking.Update::Message:{0}||Trace:{1}", ex.Message, ex.StackTrace.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
    }
}
