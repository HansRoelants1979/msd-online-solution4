using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Tc.Crm.Service.Models
{
    [DataContract]
    public class Customer
    {
        [DataMember(Name = "sourcekey")]
        public string Id { get; set; }
        [DataMember(Name = "firstname")]
        public string FirstName { get; set; }
        [DataMember(Name = "lastname")]
        public string LastName { get; set; }
        [DataMember(Name = "email")]
        public string Email { get; set; }
    }
}