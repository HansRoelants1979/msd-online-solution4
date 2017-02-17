using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace Tc.Crm.WebJob.DeallocateResortTeam.Models
{
    public class BookingDeallocation
    {
        public Guid HotelId { get; set; }



        public ResortTeam Team { get; set; }

        public List<ResortTeam> ResortTeam { get; set; }

        public List<Accommodation> Accommodation { get; set; }

        public List<Booking> Booking { get; set; }

        public int Days { get; set; }
    }

    public class ResortTeam
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
    }

    public class Accommodation
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
    }

    public class Booking
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
    }

    public class Hotel
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
}
