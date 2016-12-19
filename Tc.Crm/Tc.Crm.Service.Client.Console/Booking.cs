using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.Service.Client.Console
{
    public class Booking
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Postcode { get; set; }

        public string Country { get; set; }
        public decimal TotalAmount { get; set; }

        public int BookingId { get; set; }
    }
}
