using System.Runtime.Serialization;

namespace Tc.Crm.Common.IntegrationLayer.Jti.Models
{
    [DataContract]
    public class JsonWebTokenPayloadBase
    {
        [DataMember(Name = "nbf")]
        public string NotBefore { get; set; }

        [DataMember(Name = "iat")]
        public string IssuedAtTime { get; set; }

        [DataMember(Name = "exp")]
        public string Expiry { get; set; }
    }
}
