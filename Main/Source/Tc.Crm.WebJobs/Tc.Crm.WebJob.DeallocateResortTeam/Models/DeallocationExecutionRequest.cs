using System.Collections.Generic;
using Tc.Crm.Common.Models;

namespace Tc.Crm.WebJob.DeallocateResortTeam.Models
{
    public class DeallocationExecutionRequest
    {
        public HashSet<Booking> Bookings { get; set; }
        public HashSet<Customer> Customers { get; set; }
        public HashSet<Case> Cases { get; set; }

        public int TotalBookings
        {
            get
            {
                return Bookings != null ? Bookings.Count : 0;
            }
        }
        public int TotalCustomers
        {
            get
            {
                return Customers != null ? Customers.Count : 0;
            }
        }
        public int TotalCases
        {
            get
            {
                return Cases != null ? Cases.Count : 0;
            }
        }

        public int TotalItems
        {
            get { return TotalBookings + TotalCustomers + TotalCases; }
        }
    }
}
