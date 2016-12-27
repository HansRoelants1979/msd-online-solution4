using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tc.Crm.Service.Common;
using Tc.Crm.Service.Models;



namespace Tc.Crm.Service.BusinessServices.CRM
{
    public class BookingService : IBookingService
    {
        IDataService service = null;
        public BookingService()
        {
            service = new CrmDataService();  
        }
        public async Task<Guid> Create(Booking booking)
        {
            var t = await service.Create("bookings", GetBookingJsonObject(booking));
            return t;
        }
        public JObject GetBookingJsonObject(Booking booking)
        {
            JObject jsonBooking = new JObject();
            jsonBooking[Constants.CrmFields.Booking.FirstName] = booking.FirstName;
            jsonBooking[Constants.CrmFields.Booking.LastName] = booking.LastName;

            return jsonBooking;
        }

        public async Task<bool> Update(Booking booking)
        {
            var t = await service.Update("bookings", GetBookingJsonObject(booking));
            return t;
        }

        public async Task<Booking> Upsert(Booking booking)
        {
            if (string.IsNullOrWhiteSpace(booking.Id))
            {
                var t = await service.Create("bookings", GetBookingJsonObject(booking));
                booking.Id = t.ToString();
                return booking;
            }
            else
            {
                var t = await service.Update("bookings", GetBookingJsonObject(booking));
                if (t)
                    return booking;
                return null;
            }
        }
    }
}