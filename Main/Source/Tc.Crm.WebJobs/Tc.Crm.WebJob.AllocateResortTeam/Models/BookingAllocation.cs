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
        public int DepartureDateInNextXDays { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public IList<Guid> Destination { get; set; }
    }

    public class BookingAllocationResponse
    {
        public Guid BookingId { get; set; }
        public DateTime AccommodationStartDate { get; set; }
        public DateTime AccommodationEndDate { get; set; }
        public Owner HotelOwner { get; set; }
        public Customer Customer { get; set; }
        public Owner BookingOwner { get; set; }
    }

    public class BookingAllocationResortTeamRequest
    {
       public BookingResortTeamRequest BookingResortTeamRequest { get; set; }
       public CustomerResortTeamRequest CustomerResortTeamRequest { get; set; }

    }

    public class BookingResortTeamRequest
    {
        public Guid Id { get; set; }
        public Owner Owner { get; set; }
    }

    public class CustomerResortTeamRequest
    {
        public Customer Customer { get; set; }
        public Owner Owner { get; set; }
    }

    public class Owner
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public OwnerType OwnerType { get; set; }
    }

    public class Customer
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public CustomerType CustomerType { get; set; }
    }

    public enum OwnerType
    {
        User,
        Team
    }

   
    public enum CustomerType
    {
        Contact,
        Account
    }

}
