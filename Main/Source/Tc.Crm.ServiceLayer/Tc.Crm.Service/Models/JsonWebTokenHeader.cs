using System.Runtime.Serialization;

namespace Tc.Crm.Service.Models
{
    [DataContract]
    public class JsonWebTokenHeader
    {
        [DataMember(Name ="typ")]
        public string TokenType { get; set; }

        [DataMember(Name = "alg")]
        public string Algorithm { get; set; }
    }
}