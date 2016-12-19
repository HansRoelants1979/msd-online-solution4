using Tc.Crm.Service.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Tc.Crm.Service.Controllers
{

    public class BookingController : ApiController
    {
        Booking[] bookings = new Booking[]
           {
                new Booking { FirstName = "John", LastName = "Doe",Country="Phillippines",TotalAmount=200,BookingId=101},
                new Booking { FirstName = "Joe", LastName = "Blog",Country="China",TotalAmount=400,BookingId=100},
                new Booking { FirstName = "John", LastName = "Jones",Country="Italy",TotalAmount=400,BookingId=102},
           };
        public IEnumerable<Booking> GetAllBookings()
        {
            return bookings;
        }

        public IHttpActionResult GetBooking(int id)
        {
            var booking = bookings.FirstOrDefault((p) => p.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }
            return Ok(booking);
        }


        public HttpResponseMessage Put(Booking booking)
        {
            var b  = bookings.FirstOrDefault((p) => p.BookingId == booking.BookingId);
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
