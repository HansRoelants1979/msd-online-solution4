using JWT;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using Tc.Crm.Service.Models;
using System.Security.Cryptography;

namespace Tc.Crm.Service.Services
{
    public static class JsonWebTokenHelper
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Parses the request to an object
        /// Validates the Header pay load and signature
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static JsonWebTokenRequest GetRequestObject(HttpRequestMessage request)
        {
            var jsonWebTokenRequest = new JsonWebTokenRequest();
            try
            {
                if (request == null)
                    throw new ArgumentNullException("request");

                //parse json web token parts
                jsonWebTokenRequest.Token = GetToken(request);
                jsonWebTokenRequest.Header = DecodeHeaderToObject<JsonWebTokenHeader>(jsonWebTokenRequest.Token);
                jsonWebTokenRequest.Payload = DecodePayloadToObject<JsonWebTokenPayload>(jsonWebTokenRequest.Token);

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

        /// <summary>
        /// Validates the signature
        /// </summary>
        /// <param name="jsonWebTokenRequest"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static void ValidateSignature(JsonWebTokenRequest jsonWebTokenRequest)
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
                var key = ConfigurationManager.AppSettings[Constants.Configuration.AppSettings.JsonWebTokenSecret];
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

        /// <summary>
        /// Gets the token from the request header
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        public static string GetToken(HttpRequestMessage request)
        {
            //guard clause
            if (request == null) throw new ArgumentNullException(Constants.Parameters.Request);
            if (request.Headers == null) throw new ArgumentNullException(Constants.Parameters.RequestHeaders);
            if (request.Headers.Authorization == null) throw new ArgumentNullException(Constants.Parameters.RequestHeadersAuthorization);

            return request.Headers.Authorization.Parameter;
        }

        /// <summary>
        /// Validates the Json Web token header
        /// </summary>
        /// <param name="request"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static void ValidateHeader(JsonWebTokenRequest request)
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
        /// Validates the payload
        /// </summary>
        /// <param name="request"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static void ValidatePayload(JsonWebTokenRequest request)
        {
            //guard clause
            if (request == null) throw new ArgumentNullException(Constants.Parameters.Request);
            if (request.Payload == null) throw new ArgumentNullException(Constants.Parameters.RequestPayload);
            try
            {
                if (string.IsNullOrEmpty(request.Payload.NotBefore))
                {
                    request.NotBeforetimeValid = true;
                }
                else
                {
                    int expInt = ConvertToInt(request.Payload.NotBefore);
                    var secondsSinceEpoch = Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds);
                    if (secondsSinceEpoch >= expInt)
                        request.NotBeforetimeValid = false;
                    else
                        request.NotBeforetimeValid = true;
                }

                //check iat
                if (string.IsNullOrEmpty(request.Payload.IssuedAtTime))
                {
                    request.IssuedAtTimeValid = true;
                }
                else
                {
                    //convert  iat to integer
                    int expInt = ConvertToInt(request.Payload.IssuedAtTime);
                    var secondsSinceEpoch = Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds);
                    int expiry = -1;
                    try
                    {
                        expiry = Int32.Parse(ConfigurationManager.AppSettings[Constants.Configuration.AppSettings.IssuedAtTimeExpiryInSeconds]);
                    }
                    catch (FormatException)
                    {
                        throw new FormatException(Constants.Messages.ExpiryNotInteger);
                    }
                     
                    if (expInt >= secondsSinceEpoch + expiry)
                        request.IssuedAtTimeValid = false;
                    else
                        request.IssuedAtTimeValid = true;
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
        private static int ConvertToInt(string v)
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
        public static T DecodeHeaderToObject<T>(string token)
        {
            //guard clause
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException(Constants.Parameters.Token);

            var parts = token.Split(Constants.Delimiters.Dot);
            if (parts.Length != 3)
            {
                throw new FormatException(Constants.Messages.TokenFormatError);
            }

            var header = parts[0];
            var headerJson = Encoding.UTF8.GetString(JsonWebToken.Base64UrlDecode(header));

            return JsonConvert.DeserializeObject<T>(headerJson);
        }

        /// <summary>
        /// decode the payload of the JWT Token
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token"></param>
        /// <returns></returns>
        public static T DecodePayloadToObject<T>(string token)
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