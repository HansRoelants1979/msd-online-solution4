using System;
using System.Collections.Generic;
using System.Net;
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
    public class JwtService : IJwtService
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

        public ResponseEntity SendHttpRequestWithCookie(HttpMethod method, string serviceUrl, string token, string data, string correlationId, Dictionary<string, string> cookies)
        {
            var request = new HttpClient();
            request.DefaultRequestHeaders.Accept.Clear();
            request.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            if (!string.IsNullOrEmpty(correlationId))
                request.DefaultRequestHeaders.Add("tc-correlation-id", correlationId);
            if (cookies != null && cookies.Count > 0)
            {
                var cookieString = new StringBuilder();
                foreach (var item in cookies)
                {
                    cookieString.Append($"{item.Key}={item.Value},");
                }
                request.DefaultRequestHeaders.Add("Cookie", cookieString.ToString().TrimEnd(','));
            }

            ResponseEntity result;

            try
            {
                var t = SendHttpRequestAsync(method, serviceUrl, data, request);
                var response = t.Result;
                var task = response.Content.ReadAsStringAsync();
                var content = task.Result;
                result = new ResponseEntity
                {
                    Content = content,
                    StatusCode = response.StatusCode
                };

                Dictionary<string, string> responseCookies = null;
                IEnumerable<string> cookieList;
                if (response.Headers.TryGetValues("Set-Cookie", out cookieList))
                {
                    foreach (string c in cookieList)
                    {
                        responseCookies = new Dictionary<string, string>();
                        var cookieArray = c.Split(',');
                        foreach (var ci in cookieArray)
                        {
                            var cookieItemArray = ci.Split('=');
                            responseCookies.Add(cookieItemArray[0], cookieItemArray[1]);
                        }
                        break;
                    }

                    result.Cookies = responseCookies;
                }


                
                return result;
            }
            catch (AggregateException ex)
            {
                result = new ResponseEntity
                {
                    Content = ex.InnerException?.Message ?? ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                };
                _logger.LogError($"{SendHttpRequestApplicationTerminated}{ex.ToString()}");
            }
            catch (Exception ex)
            {
                result = new ResponseEntity
                {
                    Content = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                };
                _logger.LogError($"{SendHttpRequestApplicationTerminated}{ex.ToString()}");
            }
            return result;
        }

        public ResponseEntity SendHttpRequest(HttpMethod method, string serviceUrl, string token, string data, string correlationId)
        {
            var request = new HttpClient();
            request.DefaultRequestHeaders.Accept.Clear();
            request.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            if (!string.IsNullOrEmpty(correlationId))
                request.DefaultRequestHeaders.Add("tc-correlation-id", correlationId);

            ResponseEntity result;

            try
            {
                var t = SendHttpRequestAsync(method, serviceUrl, data, request);
                var response = t.Result;
                var task = response.Content.ReadAsStringAsync();
                var content = task.Result;

                result = new ResponseEntity
                {
                    Content = content,
                    StatusCode = response.StatusCode
                };
                return result;
            }
            catch (AggregateException ex)
            {
                result = new ResponseEntity
                {
                    Content = ex.InnerException?.Message ?? ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                };
                _logger.LogError($"{SendHttpRequestApplicationTerminated}{ex.ToString()}");
            }
            catch (Exception ex)
            {
                result = new ResponseEntity
                {
                    Content = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                };
                _logger.LogError($"{SendHttpRequestApplicationTerminated}{ex.ToString()}");
            }
            return result;
        }

        public ResponseEntity SendHttpRequest(HttpMethod method, string serviceUrl, string token, string data)
        {
            return SendHttpRequest(method, serviceUrl, token, data, null);
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

        private Task<HttpResponseMessage> SendHttpRequestAsync(HttpMethod method, string serviceUrl, string data, HttpClient request)
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
                case HttpMethod.Patch:
                    return PatchAsync(request, serviceUrl, new StringContent(data, Encoding.UTF8, "application/json"));
                default:
                    throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }
        }

        public async Task<HttpResponseMessage> PatchAsync(HttpClient client, string serviceUrl, HttpContent content)
        {
            var method = new System.Net.Http.HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, serviceUrl)
            {
                Content = content
            };

            var response = new HttpResponseMessage();

            try
            {
                response = await client.SendAsync(request);
            }
            catch (TaskCanceledException exception)
            {
                _logger.LogError(exception.ToString());
                UpdateResponse(response, exception.InnerException?.Message ?? exception.Message);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.ToString());
                UpdateResponse(response, exception.InnerException?.Message ?? exception.Message);
            }

            return response;
        }

        private static void UpdateResponse(HttpResponseMessage response, string data)
        {
            response.StatusCode = HttpStatusCode.InternalServerError;
            if (response.Content == null)
            {
                response.Content = new StringContent(data, Encoding.UTF8, "application/json");
            }
        }
    }
}