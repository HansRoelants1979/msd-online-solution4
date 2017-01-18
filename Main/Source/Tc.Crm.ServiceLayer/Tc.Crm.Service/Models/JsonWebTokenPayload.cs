using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Tc.Crm.Service.Models
{
    [DataContract]
    public class JsonWebTokenPayload
    {
        [DataMember(Name ="exp")]
        public string NotBefore { get; set; }

        [DataMember(Name = "iat")]
        public string IssuedAtTime { get; set; }
    }
}