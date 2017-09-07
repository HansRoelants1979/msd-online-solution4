using System.Runtime.Serialization;

namespace Tc.Crm.Common.Jti.Models
{
    [DataContract]
    public class JsonWebTokenPayloadBase
    {
        [DataMember(Name = "nbf")]
        public virtual string NotBefore { get; set; }

        [DataMember(Name = "iat")]
        public virtual string IssuedAtTime { get; set; }

        [DataMember(Name = "exp")]
        public virtual string Expiry { get; set; }
    }
}
