using System;
using System.Collections.Generic;

namespace Tc.Crm.WebJob.DeallocateResortTeam.Models
{
    public class DeallocationRequest
    {
        public DateTime Date { get; set; }
        public IList<Guid> Destination { get; set; }
        public string UserRolesToAssignCase { get; set; }
        public string TeamRolesToAssignCase { get; set; }
    }
    
}
