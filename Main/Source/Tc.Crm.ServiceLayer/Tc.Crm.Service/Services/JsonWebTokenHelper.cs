using JWT;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using Tc.Crm.Service.Models;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Globalization;

namespace Tc.Crm.Service.Services
{
    public class JsonWebTokenHelper
    {
        private readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        private IConfigurationService configurationService;

        public JsonWebTokenHelper(IConfigurationService configurationService)
        {
            this.configurationService = configurationService;
        }
        /// <summary>
        /// Parses the request to an object
        /// Validates the Header pay load and signature
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Tc.Crm.Service.Models.JsonWebTokenRequestError.#ctor(System.String)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public JsonWebTokenRequest GetRequestObject(HttpRequestMessage request)
        {
            var jsonWebTokenRequest = new JsonWebTokenRequest();
            try
            {
                if (request == null)
                    throw new ArgumentNullException("request");

                Trace.TraceInformation("requst is not null");
                //parse json web token parts
                jsonWebTokenRequest.Token = GetToken(request);
                Trace.TraceInformation("token: {0}", jsonWebTokenRequest.Token);
                jsonWebTokenRequest.Header = DecodeHeaderToObject<JsonWebTokenHeader>(jsonWebTokenRequest.Token);
                Trace.TraceInformation("algo: {0},type:{1}", jsonWebTokenRequest.Header.Algorithm, jsonWebTokenRequest.Header.TokenType);
                jsonWebTokenRequest.Payload = DecodePayloadToObject<JsonWebTokenPayload>(jsonWebTokenRequest.Token);
                Trace.TraceInformation("iat: {0},nbf:{1},exp:{2}", jsonWebTokenRequest.Payload.IssuedAtTime, jsonWebTokenRequest.Payload.NotBefore, jsonWebTokenRequest.Payload.Expiry);


                //validate the parts
                ValidateHeader(jsonWebTokenRequest);
                ValidatePayload(jsonWebTokenRequest);
                ValidateSignature(jsonWebTokenRequest);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Unexpected error in GetRequestObject::Message:{0}||Trace:{1}", ex.Message, ex.StackTrace);
                jsonWebTokenRequest.Errors.Add(new JsonWebTokenRequestError(ex.Message, ex.StackTrace));
                jsonWebTokenRequest.Errors.Add(new JsonWebTokenRequestError(Constants.Messages.JsonWebTokenParserError));
            }

            return jsonWebTokenRequest;
        }


        public JsonWebTokenRequest GetRequestObject(Payload payload)
        {
            var jsonWebTokenRequest = new JsonWebTokenRequest();
            try
            {
                if (payload == null || string.IsNullOrWhiteSpace(payload.JWTToken))
                    throw new ArgumentNullException("request");

                Trace.TraceInformation("requst is not null");
                //parse json web token parts
                jsonWebTokenRequest.Token = payload.JWTToken;
                Trace.TraceInformation("token: {0}", jsonWebTokenRequest.Token);
                jsonWebTokenRequest.Header = DecodeHeaderToObject<JsonWebTokenHeader>(jsonWebTokenRequest.Token);
                Trace.TraceInformation("algo: {0},type:{1}", jsonWebTokenRequest.Header.Algorithm, jsonWebTokenRequest.Header.TokenType);
                jsonWebTokenRequest.Payload = DecodePayloadToObject<JsonWebTokenPayload>(jsonWebTokenRequest.Token);
                Trace.TraceInformation("iat: {0},nbf:{1},exp:{2}", jsonWebTokenRequest.Payload.IssuedAtTime, jsonWebTokenRequest.Payload.NotBefore, jsonWebTokenRequest.Payload.Expiry);


                //validate the parts
                ValidateHeaderForHMAC(jsonWebTokenRequest);
                ValidatePayload(jsonWebTokenRequest);
                ValidateSignatureUsingHMAC(jsonWebTokenRequest);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Unexpected error in GetRequestObject::Message:{0}||Trace:{1}", ex.Message, ex.StackTrace);
                jsonWebTokenRequest.Errors.Add(new JsonWebTokenRequestError(ex.Message, ex.StackTrace));
                jsonWebTokenRequest.Errors.Add(new JsonWebTokenRequestError(Constants.Messages.JsonWebTokenParserError));
            }

            return jsonWebTokenRequest;
        }

        private void ValidateSignatureUsingHMAC(JsonWebTokenRequest jsonWebTokenRequest)
        {
            try
            {
                //guard clause
                if (jsonWebTokenRequest == null) throw new ArgumentNullException(Constants.Parameters.JsonWebTokenRequest);
                if (jsonWebTokenRequest.Token == null) throw new ArgumentNullException(Constants.Parameters.JsonWebTokenRequestToken);

                var tokenParts = jsonWebTokenRequest.Token.Split(Constants.Delimiters.Dot);

                //get the decoded signature from the request
                var crypto = JsonWebToken.Base64UrlDecode(tokenParts[2]);
                var decodedCrypto = Convert.ToBase64String(crypto);

                //Recreating the signature from the JWT request header and payload
                var header = tokenParts[0];
                var payload = tokenParts[1];
                var bytesToSign = Encoding.UTF8.GetBytes(string.Concat(header, Constants.Delimiters.Dot, payload));
                byte[] signatureData;
                var key = configurationService.GetSecretKey();
                using (var sha = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
                {
                    signatureData = sha.ComputeHash(bytesToSign);
                }
                var decodedSignature = Convert.ToBase64String(signatureData);

                //compare signatures
                if (decodedCrypto == decodedSignature)
                    jsonWebTokenRequest.SignatureValid = true;
                else
                    jsonWebTokenRequest.SignatureValid = false;
            }
            catch (Exception ex)
            {
                jsonWebTokenRequest.Errors.Add(new JsonWebTokenRequestError(ex.Message, ex.StackTrace));
                jsonWebTokenRequest.Errors.Add(new JsonWebTokenRequestError(Constants.Messages.SignatureValidationUnhandledError));
            }
        }

        private void ValidateHeaderForHMAC(JsonWebTokenRequest request)
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
                if (!request.Header.Algorithm.Equals(Constants.JsonWebTokenContent.AlgorithmHS256
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

        /// <summary>
        /// Validates the signature
        /// </summary>
        /// <param name="jsonWebTokenRequest"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Tc.Crm.Service.Models.JsonWebTokenRequestError.#ctor(System.String)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void ValidateSignature(JsonWebTokenRequest jsonWebTokenRequest)
        {
            try
            {
                //guard clause
                if (jsonWebTokenRequest == null) throw new ArgumentNullException(Constants.Parameters.JsonWebTokenRequest);
                if (jsonWebTokenRequest.Token == null) throw new ArgumentNullException(Constants.Parameters.JsonWebTokenRequestToken);

                var tokenParts = jsonWebTokenRequest.Token.Split(Constants.Delimiters.Dot);

                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    var publicKeyXml = configurationService.GetPublicKey();
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

        /// <summary>
        /// Gets the token from the request header
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        public string GetToken(HttpRequestMessage request)
        {
            if (request == null) throw new ArgumentNullException(Constants.Parameters.Request);
            if (request.Headers == null) throw new ArgumentNullException(Constants.Parameters.RequestHeaders);
            if (request.Headers.Authorization == null) throw new ArgumentNullException(Constants.Parameters.RequestHeadersAuthorization);

            return request.Headers.Authorization.Parameter;
        }

        /// <summary>
        /// Validates the Json Web token header
        /// </summary>
        /// <param name="request"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Tc.Crm.Service.Models.JsonWebTokenRequestError.#ctor(System.String)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void ValidateHeader(JsonWebTokenRequest request)
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

        /// <summary>
        /// Validates the payload
        /// </summary>
        /// <param name="request"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Tc.Crm.Service.Models.JsonWebTokenRequestError.#ctor(System.String)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void ValidatePayload(JsonWebTokenRequest request)
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
                        expiry = Int32.Parse(configurationService.GetIssuedAtTimeExpiryInSeconds()
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
        /// <summary>
        /// converts string to integer
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private int ConvertToInt(string v)
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