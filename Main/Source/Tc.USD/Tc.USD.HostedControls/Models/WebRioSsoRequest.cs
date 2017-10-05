using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Usd.HostedControls.Models
{
    [DataContract]
    public class WebRioSsoRequest
    {
        [DataMember(Name ="consultation")]
        public string Consultation { get; set; }

        [DataMember(Name = "customer")]
        public string Customer { get; set; }
    }
}
