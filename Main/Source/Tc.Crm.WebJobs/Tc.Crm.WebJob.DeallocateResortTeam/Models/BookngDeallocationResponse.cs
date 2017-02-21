using System;
using Tc.Crm.Common.Models;

namespace Tc.Crm.WebJob.DeallocateResortTeam.Models
{
    public class BookingDeallocationResponse
    {
        public Guid BookingId { get; set; }
        public DateTime AccommodationEndDate { get; set; }
        public Guid HotelId { get; set; }
        public Customer Customer { get; set; }
    }
}
