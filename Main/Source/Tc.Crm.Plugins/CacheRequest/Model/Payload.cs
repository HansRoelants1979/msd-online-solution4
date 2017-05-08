using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.Plugins.CacheRequest.Model
{
    [DataContract]
    public class Payload
    {
        [DataMember]
        public string Bucket { get; set; }
        [DataMember]
        public string JWTToken { get; set; }
    }
}
