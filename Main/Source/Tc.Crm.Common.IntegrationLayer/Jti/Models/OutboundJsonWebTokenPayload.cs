using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tc.Crm.Common.IntegrationLayer.Jti.Models
{
    [DataContract]
    public class OutboundJsonWebTokenPayload : JsonWebTokenPayloadBase
    {
		[DataMember(Name = "iss")]
		public string Issuer { get; set; }

		public override Dictionary<string, object> GeneratePayload()
		{
			var payload = base.GeneratePayload();

			payload.Add("iss", Issuer);

			return payload;
		}
	}
}