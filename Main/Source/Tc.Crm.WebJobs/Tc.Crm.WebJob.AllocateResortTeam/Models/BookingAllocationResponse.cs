using System;
using Tc.Crm.Common.Models;

namespace Tc.Crm.WebJob.AllocateResortTeam.Models
{
    public class BookingAllocationResponse
    {
        public Guid BookingId { get; set; }
        public DateTime AccommodationStartDate { get; set; }
        public DateTime AccommodationEndDate { get; set; }
        public Owner HotelOwner { get; set; }
        public Customer Customer { get; set; }
        public Owner BookingOwner { get; set; }
    }
}
