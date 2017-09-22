using System.Runtime.Serialization;

namespace Tc.Crm.Service.Models
{
    [DataContract(Name = "customerInfo")]
    public class CustomerInformation
    {
        private Customer customer = new Customer();
        [DataMember(Name = "customer")]
        public Customer Customer {
            get { return customer; }
            set { customer = value; }
        }
    }
}