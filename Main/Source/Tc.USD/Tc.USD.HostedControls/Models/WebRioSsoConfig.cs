using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Usd.HostedControls.Models
{
    public class WebRioSsoConfig
    {
        public SsoLogin Login { get; set; }
        public string PrivateKey { get; set; }
        public string CallerId { get; set; }
        public string NotBeforeTime { get; set; }
        public string ExpirySeconds { get; set; }
        public string AdminApi { get; set; }
        public string ServiceUrl { get; set; }
        public PageType RequestType { get; set; }
        public string JSessionId { get; set; }
        public List<string> Errors { get; set; }
    }
}
