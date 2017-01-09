using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.Services
{
    public class BookingService
    {
        public static Booking GetBookingFromPayload(string dataJson)
        {
            return JsonConvert.DeserializeObject<Booking>(dataJson);
        }


    }
}