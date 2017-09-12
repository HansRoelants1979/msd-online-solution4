using System.Collections.Generic;
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

        public virtual Dictionary<string, object> GeneratePayload()
        {
            var payload = new Dictionary<string, object>
            {
                {"iat", IssuedAtTime},
                {"nbf", NotBefore},
                {"exp", Expiry}
            };

            return payload;
        }
    }
}