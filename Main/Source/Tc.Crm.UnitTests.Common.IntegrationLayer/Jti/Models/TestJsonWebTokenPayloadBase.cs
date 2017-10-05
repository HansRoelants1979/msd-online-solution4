using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tc.Crm.Common.IntegrationLayer.Jti.Models;

namespace Tc.Crm.UnitTests.Common.IL.Jti.Models
{
	[TestClass]
	public class TestJsonWebTokenPayloadBase
	{
		[TestMethod]
		public void GeneratePayloadTest()
		{
			// Given
			var token = new JsonWebTokenPayloadBase()
			{
				Expiry = "expiry",
				IssuedAtTime = "issuedAtTime",
				NotBefore = "notBefore"
			};

			// When
			var payload = token.GeneratePayload();

			// Then
			Assert.AreEqual(token.Expiry, payload["exp"]);
			Assert.AreEqual(token.IssuedAtTime, payload["iat"]);
			Assert.AreEqual(token.NotBefore, payload["nbf"]);
		}
	}
}
