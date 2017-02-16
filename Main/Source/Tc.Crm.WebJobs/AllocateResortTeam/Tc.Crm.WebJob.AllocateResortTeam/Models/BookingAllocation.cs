using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;


namespace Tc.Crm.WebJob.AllocateResortTeam.Models
{
    public class BookingAllocation
    {
               
        public EntityReference HotelId { get; set; }

        public ResortTeam Team { get; set; }

        public EntityCollection Accommodation { get; set; }

        public EntityCollection Booking { get; set; }

        public int Days { get; set; }
    }

    public class ResortTeam
    {
        public EntityReference Team { get; set; }
    }
}
