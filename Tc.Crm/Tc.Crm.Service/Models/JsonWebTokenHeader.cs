using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Tc.Crm.Service.Models
{
    [DataContract]
    public class JsonWebTokenHeader
    {
        [DataMember(Name ="typ")]
        public string Type { get; set; }

        [DataMember(Name = "alg")]
        public string Algorithm { get; set; }
    }
}