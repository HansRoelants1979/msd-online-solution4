using System.Runtime.Serialization;

namespace Tc.Crm.Service.Models
{

    [DataContract]
    public class Booking
    {
        [DataMember(Name = "sourcekey")]
        public string Id { get; set; }
        [DataMember(Name = "total")]
        public decimal TotalAmount { get; set; }
        [DataMember(Name = "customerkey")]
        public string CustomerId { get; set; }
    }
}