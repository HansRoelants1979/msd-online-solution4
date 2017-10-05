using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tc.Crm.Common.IntegrationLayer.Jti.Models;

namespace Tc.Crm.UnitTests.Common.IL.Jti.Models
{
	[TestClass]
	public class TestOutboundJsonWebTokenPayload
	{
		[TestMethod]
		public void GeneratePayloadTest()
		{
			// Given
			var token = new OutboundJsonWebTokenPayload()
			{
				Issuer = "issuer"		
			};
			
			// When
			var payload = token.GeneratePayload();

			// Then
			Assert.AreEqual(token.Expiry, payload["exp"]);
			Assert.AreEqual(token.IssuedAtTime, payload["iat"]);
			Assert.AreEqual(token.NotBefore, payload["nbf"]);
			Assert.AreEqual(token.Issuer, payload["iss"]);
		}
	}
}
