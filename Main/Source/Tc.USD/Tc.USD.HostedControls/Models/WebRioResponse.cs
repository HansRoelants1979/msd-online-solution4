using System.Runtime.Serialization;

namespace Tc.Usd.HostedControls.Models
{
    [DataContract]
    public class WebRioResponse
    {
        [DataMember(Name ="responseCode")]
        public string ResponseCode { get; set; }
        [DataMember(Name = "responseMessage")]
        public string ResponseMessage { get; set; }
        [DataMember(Name = "webRioUrl")]
        public string WebRioUrl { get; set; }
    }
}
