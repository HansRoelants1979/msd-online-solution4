using Tc.Crm.Service.Models;
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
        public HttpResponseMessage Update(BookingInformation bookingInfo)
        {
            try
            {
                var messages = bookingService.Validate(bookingInfo);
                if (messages != null && messages.Count !=0)
                {
                    var message = bookingService.GetStringFrom(messages);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, message);
                }
                var booking = bookingInfo.Booking;
                bookingService.ResolveReferences(booking);
                if(string.IsNullOrWhiteSpace(booking.BookingIdentifier.SourceMarket))
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Constants.Messages.SourceMarketMissing);
                var jsonData = JsonConvert.SerializeObject(booking);
                var response = bookingService.Update(jsonData, crmService);
                if (response.Created)
                    return Request.CreateResponse(HttpStatusCode.Created, response.Id);
                else
                    return Request.CreateResponse(HttpStatusCode.NoContent, response.Id);

            }
            catch (Exception ex)
            {
                Trace.TraceError("Unexpected error Booking.Update::Message:{0}||Trace:{1}", ex.Message, ex.StackTrace.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
    }
}
