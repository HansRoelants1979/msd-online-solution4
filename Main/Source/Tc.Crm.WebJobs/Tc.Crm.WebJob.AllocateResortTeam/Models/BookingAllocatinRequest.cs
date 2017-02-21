using System;
using System.Collections.Generic;

namespace Tc.Crm.WebJob.AllocateResortTeam.Models
{
    public class BookingAllocationRequest
    {
        public int DepartureDateInNextXDays { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public IList<Guid> Destination { get; set; }
    }
}
