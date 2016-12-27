using Tc.Crm.Service.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tc.Crm.Service.Filters;


namespace Tc.Crm.Service.Controllers
{
    [ApiAuthenticationFilter]
    public class BookingController : ApiController
    {
        Booking[] bookings = new Booking[]
           {
                new Booking { FirstName = "John", LastName = "Doe",Country="Phillippines",TotalAmount=200,Id="101"},
                new Booking { FirstName = "Joe", LastName = "Blog",Country="China",TotalAmount=400,Id="100"},
                new Booking { FirstName = "John", LastName = "Jones",Country="Italy",TotalAmount=400,Id="102"},
           };
        [Route("api/bookings")]
        [Route("api/v1/bookings")]
        public IEnumerable<Booking> GetAllBookings()
        {
            return bookings;
        }
        [Route("api/bookings/{id}/booking")]
        [Route("api/v1/bookings/{id}/booking")]
        public IHttpActionResult GetBooking(string id)
        {
            var booking = bookings.FirstOrDefault((p) => p.Id == id);
            if (booking == null)
            {
                return NotFound();
            }
            return Ok(booking);
        }

        [Route("api/bookings/create")]
        [Route("api/v1/bookings/create")]
        public HttpResponseMessage CreateBooking(Booking booking)
        {
            var b  = bookings.FirstOrDefault((p) => p.Id == booking.Id);
            if (b == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            else
            {
                var response = new HttpResponseMessage();
                response.Headers.Add("Message", "Hello World!!!");
                return response;
            }

        }
    }
}
