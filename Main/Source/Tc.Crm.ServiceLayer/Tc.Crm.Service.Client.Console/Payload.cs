using System.Runtime.Serialization;

namespace Tc.Crm.Service.Models
{
    [DataContract]
    public class Payload
    {
        [DataMember]
        public string Bucket { get; set; }
        [DataMember]
        public string JwtToken { get; set; }
    }
}