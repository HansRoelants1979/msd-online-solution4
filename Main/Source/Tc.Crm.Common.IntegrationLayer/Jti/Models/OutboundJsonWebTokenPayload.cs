using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tc.Crm.Common.IntegrationLayer.Jti.Models
{
    [DataContract]
    public class OutboundJsonWebTokenPayload : JsonWebTokenPayloadBase
    {
        public override Dictionary<string, object> GeneratePayload()
        {
            var payload = base.GeneratePayload();
            payload.Remove("iat");

            return payload;
        }
    }
}