using JWT;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.Services
{
    public abstract class JsonWebTokenServiceBase
    {
        public readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        public IConfigurationService ConfigurationService { get; set; }
        public abstract void ValidateSignature(JsonWebTokenRequest jsonWebTokenRequest);
        
        public virtual void ValidateSignatureFor(JsonWebTokenRequest jsonWebTokenRequest, string fileName)
        {
            try
            {
                //guard clause
                if (jsonWebTokenRequest == null) throw new ArgumentNullException(Constants.Parameters.JsonWebTokenRequest);
                if (jsonWebTokenRequest.Token == null) throw new ArgumentNullException(Constants.Parameters.JsonWebTokenRequestToken);
                if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException("Public key file name is not provided.");
                var tokenParts = jsonWebTokenRequest.Token.Split(Constants.Delimiters.Dot);

                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    var publicKeyXml = ConfigurationService.GetPublicKey(fileName);
                    rsa.FromXmlString(publicKeyXml);

                    using (SHA256 sha256 = SHA256.Create())
                    {
                        byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(tokenParts[0] + '.' + tokenParts[1]));

                        RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
                        rsaDeformatter.SetHashAlgorithm("SHA256");
                        if (!rsaDeformatter.VerifySignature(hash, JsonWebToken.Base64UrlDecode(tokenParts[2])))
                        {
                            Trace.TraceWarning("public key:{0}, token signature:{1}", publicKeyXml, tokenParts[2]);
                            jsonWebTokenRequest.SignatureValid = false;
                        }
                        else
                        {
                            jsonWebTokenRequest.SignatureValid = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                jsonWebTokenRequest.Errors.Add(new JsonWebTokenRequestError(ex.Message, ex.StackTrace));
                jsonWebTokenRequest.Errors.Add(new JsonWebTokenRequestError(Constants.Messages.SignatureValidationUnhandledError));
            }
        }
        public virtual void ValidateHeader(JsonWebTokenRequest request)
        {
            //guard clause
            if (request == null) throw new ArgumentNullException(Constants.Parameters.Request);
            if (request.Header == null) throw new ArgumentNullException(Constants.Parameters.RequestHeader);
            if (string.IsNullOrWhiteSpace(request.Header.Algorithm))
                throw new ArgumentNullException(Constants.Parameters.RequestHeaderAlgorithm);
            if (string.IsNullOrWhiteSpace(request.Header.TokenType))
                throw new ArgumentNullException(Constants.Parameters.RequestHeaderType);

            try
            {
                if (!request.Header.Algorithm.Equals(Constants.JsonWebTokenContent.AlgorithmRS256
                                                        , StringComparison.OrdinalIgnoreCase))
                {
                    request.HeaderAlgorithmValid = false;
                }
                else
                {
                    request.HeaderAlgorithmValid = true;
                }

                if (!request.Header.TokenType.Equals(Constants.JsonWebTokenContent.TypeJwt
                                                , StringComparison.OrdinalIgnoreCase))
                {
                    request.HeaderTypeValid = false;
                }
                else
                {
                    request.HeaderTypeValid = true;
                }
            }
            catch (NullReferenceException ex)
            {
                request.Errors.Add(new JsonWebTokenRequestError(ex.Message, ex.StackTrace));
                request.Errors.Add(new JsonWebTokenRequestError(Constants.Messages.HeaderValidationUnhandledError));
            }
            catch (Exception ex)
            {
                request.Errors.Add(new JsonWebTokenRequestError(ex.Message, ex.StackTrace));
                request.Errors.Add(new JsonWebTokenRequestError(Constants.Messages.HeaderValidationUnhandledError));
            }
        }

        public virtual void ValidatePayload(JsonWebTokenRequest request)
        {
            //guard clause
            if (request == null) throw new ArgumentNullException(Constants.Parameters.Request);
            if (request.Payload == null) throw new ArgumentNullException(Constants.Parameters.RequestPayload);
            try
            {
                //check nbf
                if (string.IsNullOrEmpty(request.Payload.NotBefore))
                {
                    request.NotBeforetimeValid = true;
                }
                else
                {
                    int nbf = ConvertToInt(request.Payload.NotBefore);
                    var secondsSinceEpoch = Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds);
                    if (secondsSinceEpoch >= nbf)
                    {
                        Trace.TraceWarning("nbf:{0}, current:{1}", nbf, secondsSinceEpoch);
                        request.NotBeforetimeValid = true;
                    }
                    else
                    {
                        Trace.TraceWarning("nbf:{0}, current:{1}", nbf, secondsSinceEpoch);
                        request.NotBeforetimeValid = false;
                    }
                }

                //check exp
                if (string.IsNullOrEmpty(request.Payload.Expiry))
                {
                    request.ExpiryValid = true;
                }
                else
                {
                    int expiry = ConvertToInt(request.Payload.Expiry);
                    var secondsSinceEpoch = Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds);
                    if (secondsSinceEpoch <= expiry)
                    {
                        Trace.TraceWarning("exp:{0}, current:{1}", expiry, secondsSinceEpoch);
                        request.ExpiryValid = true;
                    }
                    else
                    {
                        Trace.TraceWarning("exp:{0}, current:{1}", expiry, secondsSinceEpoch);
                        request.ExpiryValid = false;
                    }
                }

                //check iat
                if (string.IsNullOrEmpty(request.Payload.IssuedAtTime))
                {
                    request.IssuedAtTimeValid = true;
                }
                else
                {
                    //convert  iat to integer
                    int issuedAtTime = ConvertToInt(request.Payload.IssuedAtTime);
                    var secondsSinceEpoch = Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds);
                    int expiry = -1;
                    try
                    {
                        expiry = Int32.Parse(ConfigurationService.GetIssuedAtTimeExpiryInSeconds()
                            , CultureInfo.CurrentCulture);
                    }
                    catch (FormatException)
                    {
                        throw new FormatException(Constants.Messages.ExpiryNotInteger);
                    }

                    if (issuedAtTime > secondsSinceEpoch + expiry)
                    {
                        Trace.TraceWarning("iat:{0}, current:{1}", issuedAtTime, secondsSinceEpoch);
                        request.IssuedAtTimeValid = false;
                    }
                    else
                    {
                        Trace.TraceWarning("iat:{0}, current:{1}", issuedAtTime, secondsSinceEpoch);
                        request.IssuedAtTimeValid = true;
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                request.Errors.Add(new JsonWebTokenRequestError(ex.Message, ex.StackTrace));
                request.Errors.Add(new JsonWebTokenRequestError(Constants.Messages.PayloadValidationUnhandledError));
            }
            catch (Exception ex)
            {
                request.Errors.Add(new JsonWebTokenRequestError(ex.Message, ex.StackTrace));
                request.Errors.Add(new JsonWebTokenRequestError(Constants.Messages.PayloadValidationUnhandledError));
            }
        }

        public string GetToken(HttpRequestMessage request)
        {
            if (request == null) throw new ArgumentNullException(Constants.Parameters.Request);
            if (request.Headers == null) throw new ArgumentNullException(Constants.Parameters.RequestHeaders);
            if (request.Headers.Authorization == null) throw new ArgumentNullException(Constants.Parameters.RequestHeadersAuthorization);

            return request.Headers.Authorization.Parameter;
        }
        public int ConvertToInt(string v)
        {
            int i;
            if (Int32.TryParse(v, out i))
                return i;
            throw new SignatureVerificationException(Constants.Messages.ClaimNotInteger);
        }
        /// <summary>
        /// decode the header of the JWT Token
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token"></param>
        /// <returns></returns>
        public T DecodeHeaderToObject<T>(string token)
        {
            //guard clause
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException(Constants.Parameters.Token);

            var parts = token.Split(Constants.Delimiters.Dot);
            if (parts.Length != 3)
            {
                throw new FormatException(Constants.Messages.TokenFormatError);
            }

            var header = parts[0];
            Trace.TraceInformation($"header:{header}");
            var headerJson = Encoding.UTF8.GetString(JsonWebToken.Base64UrlDecode(header));

            return JsonConvert.DeserializeObject<T>(headerJson);
        }

        /// <summary>
        /// decode the payload of the JWT Token
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token"></param>
        /// <returns></returns>
        public T DecodePayloadToObject<T>(string token)
        {
            //guard clause
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException(Constants.Parameters.Token);

            var parts = token.Split(Constants.Delimiters.Dot);
            if (parts.Length != 3)
            {
                throw new FormatException(Constants.Messages.TokenFormatError);
            }

            var payLoad = parts[1];
            var payLoadJson = Encoding.UTF8.GetString(JsonWebToken.Base64UrlDecode(payLoad));

            return JsonConvert.DeserializeObject<T>(payLoadJson);
        }
    }
}