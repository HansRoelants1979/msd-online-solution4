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
    public class JsonWebTokenHelper
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Parses the request to an object
        /// Validates the Header pay load and signature
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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
                ValidatePayLoad(jsonWebTokenRequest);
                ValidateSignature(jsonWebTokenRequest);
            }
            catch (Exception ex)
            {
                jsonWebTokenRequest.Errors.Add(new JsonWebTokenRequestError(ex.Message, ex.StackTrace));
                jsonWebTokenRequest.Errors.Add(new JsonWebTokenRequestError(Constants.Messages.JSON_WEB_TOKEN_PARSES_ERROR));
            }

            return jsonWebTokenRequest;
        }

        /// <summary>
        /// Validates the signature
        /// </summary>
        /// <param name="jsonWebTokenRequest"></param>
        private static void ValidateSignature(JsonWebTokenRequest jsonWebTokenRequest)
        {
            try
            {
                //guard clause
                if (jsonWebTokenRequest == null) throw new ArgumentNullException(Constants.Parameters.JSON_WEB_TOKEN_REQUEST);
                if (jsonWebTokenRequest.Token == null) throw new ArgumentNullException(Constants.Parameters.JSON_WEB_TOKEN_REQUEST_TOKEN);

                var tokenParts = jsonWebTokenRequest.Token.Split(Constants.Delimiters.DOT);

                //get the decoded signature from the request
                var crypto = JsonWebToken.Base64UrlDecode(tokenParts[2]);
                var decodedCrypto = Convert.ToBase64String(crypto);

                //Recreating the signature from the JWT request header and payload
                var header = tokenParts[0];
                var payload = tokenParts[1];
                var bytesToSign = Encoding.UTF8.GetBytes(string.Concat(header, Constants.Delimiters.DOT, payload));
                byte[] signatureData;
                var key = ConfigurationManager.AppSettings[Constants.Configuration.AppSettings.JSON_WEB_TOKEN_SECRET];
                using (var sha = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
                {
                    signatureData = sha.ComputeHash(bytesToSign);
                }
                var decodedSignature = Convert.ToBase64String(signatureData);

                //compare signatures
                if (decodedCrypto == decodedSignature)
                    jsonWebTokenRequest.SignaturValid = true;
                else
                    jsonWebTokenRequest.SignaturValid = false;
            }
            catch (Exception ex)
            {
                jsonWebTokenRequest.Errors.Add(new JsonWebTokenRequestError(ex.Message, ex.StackTrace));
                jsonWebTokenRequest.Errors.Add(new JsonWebTokenRequestError(Constants.Messages.SIGNATURE_VALIDATION_UNHANDLED_ERROR));
            }
        }

        /// <summary>
        /// Gets the token from the request header
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetToken(HttpRequestMessage request)
        {
            try
            {
                //guard clause
                if (request == null) throw new ArgumentNullException(Constants.Parameters.REQUEST);
                if (request.Headers == null) throw new ArgumentNullException(Constants.Parameters.REQUEST_HEADERS);
                if (request.Headers.Authorization == null) throw new ArgumentNullException(Constants.Parameters.REQUEST_HEADERS_AUTHORIZATION);

                return request.Headers.Authorization.Parameter;
            }
            catch (Exception)
            {
                //TODO: logging
                throw;
            }
        }

        /// <summary>
        /// Validates the Json Web token header
        /// </summary>
        /// <param name="request"></param>
        public static void ValidateHeader(JsonWebTokenRequest request)
        {
            try
            {
                //guard clause
                if (request == null) throw new ArgumentNullException(Constants.Parameters.REQUEST);
                if (request.Header == null) throw new ArgumentNullException(Constants.Parameters.REQUEST_HEADER);
                if (string.IsNullOrWhiteSpace(request.Header.Algorithm))
                    throw new ArgumentNullException(Constants.Parameters.REQUEST_HEADER_ALGORITHM);
                if (string.IsNullOrWhiteSpace(request.Header.Type))
                    throw new ArgumentNullException(Constants.Parameters.REQUEST_HEADER_TYPE);

                if (!request.Header.Algorithm.Equals(Constants.JsonWebTokenContent.ALGORITHM_HS256
                                                        , StringComparison.OrdinalIgnoreCase))
                {
                    request.HeaderAlgorithmValid = false;
                }
                else
                {
                    request.HeaderAlgorithmValid = true;
                }

                if (!request.Header.Type.Equals(Constants.JsonWebTokenContent.TYPE_JWT
                                                , StringComparison.OrdinalIgnoreCase))
                {
                    request.HeaderTypeValid = false;
                }
                else
                {
                    request.HeaderTypeValid = true;
                }
            }
            catch (Exception ex)
            {
                request.Errors.Add(new JsonWebTokenRequestError(ex.Message, ex.StackTrace));
                request.Errors.Add(new JsonWebTokenRequestError(Constants.Messages.HEADER_VALIDATION_UNHANDLED_ERROR));
            }
        }

        /// <summary>
        /// Validates the payload
        /// </summary>
        /// <param name="request"></param>
        public static void ValidatePayLoad(JsonWebTokenRequest request)
        {
            try
            {
                //guard clause
                if (request == null) throw new ArgumentNullException(Constants.Parameters.REQUEST);
                if (request.Payload == null) throw new ArgumentNullException(Constants.Parameters.REQUEST_PAYLOAD);

                if (string.IsNullOrEmpty(request.Payload.NotBefore))
                {
                    request.NotBeforeTimeValid = true;
                }
                else
                {
                    int expInt = ConvertToInt(request.Payload.NotBefore);
                    var secondsSinceEpoch = Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds);
                    if (secondsSinceEpoch >= expInt)
                        request.NotBeforeTimeValid = false;
                    else
                        request.NotBeforeTimeValid = true;
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
                        expiry = Int32.Parse(ConfigurationManager.AppSettings[Constants.Configuration.AppSettings.ISSUED_AT_TIME_EXPIRY_IN_SECONDS]);
                    }
                    catch (Exception)
                    {
                        throw new FormatException(Constants.Messages.EXPIRY_NOT_INT);
                    }
                     
                    if (expInt >= secondsSinceEpoch + expiry)
                        request.IssuedAtTimeValid = false;
                    else
                        request.IssuedAtTimeValid = true;
                }
            }
            catch (Exception ex)
            {
                request.Errors.Add(new JsonWebTokenRequestError(ex.Message, ex.StackTrace));
                request.Errors.Add(new JsonWebTokenRequestError(Constants.Messages.PAYLOAD_VALIDATION_UNHANDLED_ERROR));
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
            throw new SignatureVerificationException(Constants.Messages.CLAIM_NOT_INT);
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
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException(Constants.Parameters.TOKEN);

            var parts = token.Split(Constants.Delimiters.DOT);
            if (parts.Length != 3)
            {
                throw new FormatException(Constants.Messages.TOKEN_FORMAT_ERROR);
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
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException(Constants.Parameters.TOKEN);

            var parts = token.Split(Constants.Delimiters.DOT);
            if (parts.Length != 3)
            {
                throw new FormatException(Constants.Messages.TOKEN_FORMAT_ERROR);
            }

            var payLoad = parts[1];
            var payLoadJson = Encoding.UTF8.GetString(JsonWebToken.Base64UrlDecode(payLoad));

            return JsonConvert.DeserializeObject<T>(payLoadJson);
        }


    }
}