using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Jose;
using Tc.Crm.Common.IntegrationLayer.Jti.Models;
using Tc.Crm.Common.Services;

namespace Tc.Crm.Common.IntegrationLayer.Jti.Service
{
    public class JwtService: IJwtService
    {
        private readonly ILogger _logger;
        public JwtService(ILogger logger)
        {
            _logger = logger;
        }
        public readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        public virtual string CreateJwtToken(string privateKey, OwrJsonWebTokenPayload payloadObj)
        {
            var payload = new Dictionary<string, object>
            {
                {"iat", payloadObj.IssuedAtTime},
                {"nbf", payloadObj.NotBefore},
                {"exp", payloadObj.Expiry},
                {"jti", payloadObj.Jti},
                {"aud", payloadObj.Aud},
                {"bra", payloadObj.BranchCode},
                {"abt", payloadObj.AbtaNumber},
                {"emp", payloadObj.EmployeeId},
                {"ini", payloadObj.Initials},
                {"crt", payloadObj.CreatedBy}
            };

            var header = new Dictionary<string, object>
            {
                {"alg", "RS256"},
                {"typ", "JWT"}
            };

            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);
            return Jose.JWT.Encode(payload, rsa, JwsAlgorithm.RS256, header);
        }
        public string SendHttpRequest(string serviceUrl, string token, string data)
        {
            var request = new HttpClient();
            request.DefaultRequestHeaders.Accept.Clear();
            request.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                var t = request.PostAsync(serviceUrl, new StringContent(data, Encoding.UTF8, "application/json"));
                var response = t.Result;
                var task = response.Content.ReadAsStringAsync();
                var content = task.Result;
                return content;
            }
            catch (Exception ex)
            {
                _logger.LogError($"SendHttpRequest application terminated with an error::{ex.ToString()}");
            }
            return null;
        }

        public double GetExpiry(string expiredSeconds)
        {
            int sec;
            int.TryParse(expiredSeconds, out sec);
            return Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds + sec);
        }

        public double GetIssuedAtTime()
        {
            return Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds);
        }

        public double GetNotBeforeTime(string notBeforeSeconds)
        {
            int sec;
            int.TryParse(notBeforeSeconds, out sec);
            return Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds + sec);
        }
    }
}
