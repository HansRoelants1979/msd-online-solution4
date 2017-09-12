using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Jose;
using Tc.Crm.Common.IntegrationLayer.Jti.Models;
using Tc.Crm.Common.IntegrationLayer.Model;
using Tc.Crm.Common.Services;

namespace Tc.Crm.Common.IntegrationLayer.Jti.Service
{
    public class JwtService: IJwtService
    {
        private const string SendHttpRequestApplicationTerminated = "SendHttpRequest application terminated with an error::";

        private readonly ILogger _logger;
        public JwtService(ILogger logger)
        {
            _logger = logger;
        }
        public readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public string CreateJwtToken<T>(string privateKey, T payloadObj) where T : JsonWebTokenPayloadBase
        {
            var payload = payloadObj.GeneratePayload();

            var header = new Dictionary<string, object>
            {
                {"alg", "RS256"},
                {"typ", "JWT"}
            };

            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);
            return JWT.Encode(payload, rsa, JwsAlgorithm.RS256, header);
        }

        public ResponseEntity SendHttpRequest(HttpMethod method, string serviceUrl, string token, string data)
        {
            var request = new HttpClient();
            request.DefaultRequestHeaders.Accept.Clear();
            request.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var t = SendHttpRequestAsync(method, serviceUrl, data, request);
                var response = t.Result;
                var task = response.Content.ReadAsStringAsync();
                var content = task.Result;

                var result = new ResponseEntity
                {
                    Content = content,
                    StatusCode = response.StatusCode
                };
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{SendHttpRequestApplicationTerminated}{ex.ToString()}");
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

        private static Task<HttpResponseMessage> SendHttpRequestAsync(HttpMethod method, string serviceUrl, string data, HttpClient request)
        {
            switch (method)
            {
                case HttpMethod.Post:
                    return request.PostAsync(serviceUrl, new StringContent(data, Encoding.UTF8, "application/json"));
                case HttpMethod.Get:
                    return request.GetAsync(serviceUrl);
                case HttpMethod.Put:
                    return request.PutAsync(serviceUrl, new StringContent(data, Encoding.UTF8, "application/json"));
                case HttpMethod.Delete:
                    return request.DeleteAsync(serviceUrl);
                default:
                    throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }
        }
    }
}