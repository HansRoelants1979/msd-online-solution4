using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Tc.Crm.Service.Models
{
    [DataContract]
    public class Token
    {
        [DataMember]
        public string PrivateKey { get; set; }
        [DataMember]
        public string IssuedAtTime { get; set; }
        [DataMember]
        public string NotBeforeTime { get; set; }
        [DataMember]
        public string Expiry { get; set; }
    }
}