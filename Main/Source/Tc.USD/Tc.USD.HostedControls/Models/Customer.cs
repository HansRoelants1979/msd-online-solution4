using System.Runtime.Serialization;

namespace Tc.Usd.HostedControls.Models
{
    
   
    [DataContract(Name = "address")]
    public class Address
    {

        [DataMember(Name = "additionalAddressInfo")]
        public string AdditionalAddressInfo { get; set; }

        [DataMember(Name = "flatNumberUnit")]
        public string FlatNumberUnit { get; set; }

        [DataMember(Name = "houseNumberBuilding")]
        public string HouseNumberBuilding { get; set; }

        [DataMember(Name = "town")]
        public string Town { get; set; }

        [DataMember(Name = "country")]
        public string Country { get; set; }

        [DataMember(Name = "county")]
        public string County { get; set; }

        [DataMember(Name = "postalCode")]
        public string PostalCode { get; set; }

        [DataMember(Name = "street")]
        public string Street { get; set; }
    }
    
   
    [DataContract(Name = "customerIdentifier")]
    public class CustomerIdentifier
    {

        [DataMember(Name = "customerID")]
        public string CustomerId { get; set; }
    }

    [DataContract(Name = "customerIdentity")]
    public class CustomerIdentity
    {

        [DataMember(Name = "salutation")]
        public string Salutation { get; set; }

        [DataMember(Name = "firstName")]
        public string FirstName { get; set; }

        [DataMember(Name = "middleName")]
        public string MiddleName { get; set; }

        [DataMember(Name = "lastName")]
        public string LastName { get; set; }
        
        [DataMember(Name = "birthdate")]
        public string Birthdate { get; set; }
    }

   
    [DataContract(Name = "phone")]
    public class Phone
    {

        [DataMember(Name = "type")]
        public string PhoneType { get; set; }

        [DataMember(Name = "number")]
        public string Number { get; set; }
    }

    [DataContract(Name = "email")]
    public class Email
    {

        [DataMember(Name = "type")]
        public string EmailType { get; set; }

        [DataMember(Name = "address")]
        public string Address { get; set; }
    }

   
    [DataContract(Name = "customer")]
    public class Customer
    {
        [DataMember(Name = "customerIdentifier")]
        public CustomerIdentifier CustomerIdentifier { get; set; }

        [DataMember(Name = "customerIdentity")]
        public CustomerIdentity CustomerIdentity { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [DataMember(Name = "address")]
        public Address[] Address { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [DataMember(Name = "phone")]
        public Phone[] Phone { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [DataMember(Name = "email")]
        public Email[] Email { get; set; }
    }

   
}


