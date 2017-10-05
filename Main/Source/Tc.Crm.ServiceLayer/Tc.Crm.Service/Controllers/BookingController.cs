using Tc.Crm.Service.Models;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tc.Crm.Service.Filters;
using System;
using Tc.Crm.Service.Services;
using System.Diagnostics;
using Newtonsoft.Json;
using Microsoft.Xrm.Sdk;

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
                if (messages != null && messages.Count != 0)
                {
                    var message = bookingService.GetStringFrom(messages);
                    Trace.TraceWarning(message);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, message);
                }
                var booking = bookingInfo.Booking;
                bookingService.ResolveReferences(booking);
                if (string.IsNullOrWhiteSpace(booking.BookingIdentifier.SourceMarket))
                {
                    Trace.TraceWarning(Constants.Messages.SourceMarketMissing);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Constants.Messages.SourceMarketMissing);
                }
                if (string.IsNullOrWhiteSpace(booking.BookingGeneral.Currency))
                {
                    Trace.TraceWarning(Constants.Messages.CurrencyResolutionError);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Constants.Messages.CurrencyResolutionError);
                }
                if (booking.Customer != null && booking.Customer.CustomerIdentifier != null)
                {
                    if (string.IsNullOrWhiteSpace(booking.Customer.CustomerIdentifier.SourceMarket))
                    {
                        Trace.TraceWarning(Constants.Messages.CustomerSourceMarketMissing);
                        return Request.CreateResponse(HttpStatusCode.BadRequest, Constants.Messages.CustomerSourceMarketMissing);
                    }
                }
                
                return BookingUpdateService(booking);

            }
            catch (Exception ex)
            {
                Trace.TraceError("Unexpected error Booking.Update::Message:{0}||Trace:{1}", ex.Message, ex.StackTrace.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        private HttpResponseMessage BookingUpdateService(Booking booking)
        {
            var jsonData = JsonConvert.SerializeObject(booking);
            var response = bookingService.Update(jsonData, crmService);
            if (response.Created)
                return Request.CreateResponse(HttpStatusCode.Created, response.Id);
            else
                return Request.CreateResponse(HttpStatusCode.NoContent, response.Id);
        }

        private HttpResponseMessage ExecuteBookingBusinessRules(Booking booking)
        {
            BookingRulesService bookingRules = new BookingRulesService();
            Entity msdBooking = new Entity();
            if (!bookingRules.IsOnTourorTCVAtCoreBooking(booking.BookingIdentifier))
            {
                if (!bookingRules.IsBookingConsultationEmpty(booking.BookingIdentifier))
                {
                    msdBooking = bookingRules.MatchingBooking(booking);
                    if (!bookingRules.IsSameCustomer(booking, msdBooking))
                    {
                        if (bookingRules.MatchingCustomer(booking))
                            return BookingUpdateService(booking);
                        else
                            return Request.CreateResponse((HttpStatusCode)422);
                    }
                    else
                        return BookingUpdateService(booking);
                }
                else
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            else
            {
                //if (bookingRules.MatchingCustomer(booking))
                //OnTourBookingDelivered goes here
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            
        }
    }
}
