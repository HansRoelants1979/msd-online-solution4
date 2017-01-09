using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Tc.Crm.Service.Models
{
    [DataContract]
    public class JwtPayload
    {
        [DataMember(Name ="exp")]
        public string Nbf { get; set; }

        [DataMember(Name = "iat")]
        public string Iat { get; set; }

        [DataMember(Name = "data")]
        public string Data { get; set; }
    }
}