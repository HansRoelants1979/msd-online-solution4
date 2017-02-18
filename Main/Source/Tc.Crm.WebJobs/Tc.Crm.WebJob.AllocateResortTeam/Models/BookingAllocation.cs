using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;


namespace Tc.Crm.WebJob.AllocateResortTeam.Models
{
   
    public class BookingAllocationRequest
    {
        public int DepartureDateinNextXDays { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public IList<Guid> Destination { get; set; }
    }

    public class BookingAllocationResponse
    {
        public Guid BookingId { get; set; }
        public DateTime AccommodationStartDate { get; set; }
        public DateTime AccommodationEndDate { get; set; }
        public Owner OwnerId { get; set; }        
        
    }

    public class BookingAllocationResortTeamRequest
    {
        public Guid Id { get; set; }

        public EntityType EntityType { get; set; }

        public CustomerType CustomerType { get; set; }
    }

    public class Owner
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public OwnerType OwnerType { get; set; }
    }

    public enum OwnerType
    {
        User,
        Team
    }

    public enum EntityType
    {
        Customer,
        Booking
    }

    public enum CustomerType
    {
        Contact,
        Account
    }

}
