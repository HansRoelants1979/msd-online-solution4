using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.Plugins.CacheRequest.Model
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
