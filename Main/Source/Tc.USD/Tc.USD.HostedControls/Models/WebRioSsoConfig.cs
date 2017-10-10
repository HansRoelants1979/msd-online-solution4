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
        public RequestType RequestType { get; set; }
        public string JSessionId { get; set; }
        public List<string> Errors { get; set; }
        public string OpenConsultationApi { get; set; }
        public string BookingSummaryId { get; set; }
        public string TravelPlannerSummaryId { get; set; }
        public string ObjectTypeCode { get; set; }
        public string ConsultationReference { get; set; }
        public string Data { get; set; }
        public string CustomerId { get; set; }
        public string NewConsultationApi { get; set; }
    }
}
