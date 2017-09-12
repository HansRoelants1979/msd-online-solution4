using System;
using System.Net;
using Tc.Crm.Common;
using Tc.Crm.Common.IntegrationLayer.Jti.Models;
using Tc.Crm.Common.IntegrationLayer.Jti.Service;
using Tc.Crm.Common.IntegrationLayer.Model;

namespace Tc.Crm.OutboundSynchronisation.CustomerTests
{
    public class TestJwtService : IJwtService
    {
        public string CreateJwtToken<T>(string privateKey, T payloadObj) where T : JsonWebTokenPayloadBase
        {
            return Guid.NewGuid().ToString();
        }

        public ResponseEntity SendHttpRequest(HttpMethod method, string serviceUrl, string token, string data)
        {
            return new ResponseEntity
            {
                Content = string.Empty,
                StatusCode = HttpStatusCode.OK
            };
        }

        public double GetExpiry(string expiredSeconds)
        {
            return 0;
        }

        public double GetIssuedAtTime()
        {
            return 0;
        }

        public double GetNotBeforeTime(string notBeforeSeconds)
        {
            return 0;
        }
    }
}