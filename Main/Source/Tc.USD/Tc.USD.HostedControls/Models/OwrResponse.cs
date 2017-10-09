using System.Runtime.Serialization;

namespace Tc.Usd.HostedControls.Models
{
        [DataContract]
        public class OwrResponseRoot
        {
            [DataMember(Name = "title")]
            public string Title { get; set; }
            [DataMember(Name = "description")]
            public string Description { get; set; }
            [DataMember(Name = "definitions")]
            public Definitions Definitions { get; set; }
        }
        
        [DataContract(Name= "definitions")]
        public class Definitions
        {
            [DataMember(Name = "owrResponse")]
            public OwrResponse OwrResponse { get; set; }
        }

        [DataContract(Name="owrResponse")]
        public class OwrResponse
        {
            [DataMember(Name = "requestId")]
            public string RequestId { get; set; }

            [DataMember(Name = "responseCode")]
            public int ResponseCode { get; set; }

            [DataMember(Name = "responseMessage")]
            public string ResponseMessage { get; set; }

            [DataMember(Name = "launchUri")]
            public string LaunchUri { get; set; }
        }
    }

