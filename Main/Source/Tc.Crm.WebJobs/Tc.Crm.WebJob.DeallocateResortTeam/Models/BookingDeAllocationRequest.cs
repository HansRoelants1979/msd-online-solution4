using System;
using System.Collections.Generic;

namespace Tc.Crm.WebJob.DeallocateResortTeam.Models
{
    public class BookingDeallocationRequest
    {
        public DateTime AccommodationEndDate { get; set; }
        public IList<Guid> Destination { get; set; }

    }
}
