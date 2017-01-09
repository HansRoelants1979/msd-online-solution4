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
    public class JwtHelper
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Parses the request to an object
        /// Validates the Header pay load and signature
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static JwtRequest GetRequestObject(HttpRequestMessage request)
        {
            var jwtReq = new JwtRequest();
            try
            {
                if (request == null)
                    throw new ArgumentNullException("request");

                //get the token
                jwtReq.Token = GetToken(request);
                //get the header
                jwtReq.Header = DecodeHeaderToObject<JwtHeader>(jwtReq.Token);
                //get the payload
                jwtReq.Payload = DecodePayloadToObject<JwtPayload>(jwtReq.Token);

                //validate the header
                ValidateHeader(jwtReq);
                //validate the payload
                ValidatePayLoad(jwtReq);
                //validate the signatue
                ValidateSignature(jwtReq);
            }
            catch (Exception ex)
            {
                jwtReq.Errors.Add(new JwtRequestError(ex.Message, ex.StackTrace));
                jwtReq.Errors.Add(new JwtRequestError("Error at entry point."));
            }

            return jwtReq;
        }

        /// <summary>
        /// Validates the signature
        /// </summary>
        /// <param name="jwtReq"></param>
        private static void ValidateSignature(JwtRequest jwtReq)
        {
            try
            {
                //guard clause
                if (jwtReq == null) throw new ArgumentNullException("jwtReq");
                if (jwtReq.Token == null) throw new ArgumentNullException("jwtReq.Token");

                var tokenParts = jwtReq.Token.Split('.');

                //get the decoded signature from the request
                var crypto = JsonWebToken.Base64UrlDecode(tokenParts[2]);
                var decodedCrypto = Convert.ToBase64String(crypto);

                //Recreating the signature from the JWT request header and payload
                //get the header
                var header = tokenParts[0];
                //get the payload
                var payload = tokenParts[1];
                //get the bytes after combining header and payload
                var bytesToSign = Encoding.UTF8.GetBytes(string.Concat(header, ".", payload));

                byte[] signatureData;
                //get the secret from web.config
                var key = ConfigurationManager.AppSettings["jwtkey"];
                //Hash the bytes
                using (var sha = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
                {
                    signatureData = sha.ComputeHash(bytesToSign);
                }
                //create the signature
                var decodedSignature = Convert.ToBase64String(signatureData);

                //compare signatures
                if (decodedCrypto == decodedSignature)
                    jwtReq.SignatureOk = true;
                else
                    jwtReq.SignatureOk = false;
            }
            catch (Exception ex)
            {
                jwtReq.Errors.Add(new JwtRequestError(ex.Message, ex.StackTrace));
                jwtReq.Errors.Add(new JwtRequestError("Error while validating the signature."));
            }
        }

        /// <summary>
        /// Gets the token from the request header
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetToken(HttpRequestMessage request)
        {
            //guard clause
            if (request == null) throw new ArgumentNullException("request");
            var re = request;
            var headers = re.Headers;
            return headers.Authorization.Parameter;
        }

        /// <summary>
        /// Validates the JWT token header
        /// </summary>
        /// <param name="request"></param>
        public static void ValidateHeader(JwtRequest request)
        {
            try
            {
                //guard clause
                if (request == null) throw new ArgumentNullException("request");
                if (request.Header == null) throw new ArgumentNullException("request.Header");

                //check the algo
                if (!request.Header.Algorithm.Equals("HS256", StringComparison.OrdinalIgnoreCase))
                    request.AlgOk = false;
                else
                    request.AlgOk = true;
                //check the type
                if (!request.Header.Type.Equals("JWT", StringComparison.OrdinalIgnoreCase))
                    request.TypeOk = false;
                else
                    request.TypeOk = true;
            }
            catch (Exception ex)
            {
                request.Errors.Add(new JwtRequestError(ex.Message, ex.StackTrace));
                request.Errors.Add(new JwtRequestError("Error while validating the header."));
            }
        }

        /// <summary>
        /// Validates the payload
        /// </summary>
        /// <param name="request"></param>
        public static void ValidatePayLoad(JwtRequest request)
        {
            try
            {
                //guard clause
                if (request == null) throw new ArgumentNullException("request");
                if (request.Payload == null) throw new ArgumentNullException("request.Payload");

                //check nbf
                if (string.IsNullOrEmpty(request.Payload.Nbf))
                {
                    request.NbfOk = true;
                }
                else
                {
                    //convert the nbf to integer
                    int expInt = ConvertToInt(request.Payload.Nbf);
                    
                    var secondsSinceEpoch = Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds);

                    if (secondsSinceEpoch >= expInt)
                        request.NbfOk = false;
                    else
                        request.NbfOk = true;
                }

                //check iat
                if (string.IsNullOrEmpty(request.Payload.Iat))
                {
                    request.IatOk = true;
                }
                else
                {
                    //convert  iat to integer
                    int expInt = ConvertToInt(request.Payload.Iat);
                    var secondsSinceEpoch = Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds);

                    if (expInt >= secondsSinceEpoch + 1800)
                        request.IatOk = false;
                    else
                        request.IatOk = true;
                }
            }
            catch (Exception ex)
            {
                request.Errors.Add(new JwtRequestError(ex.Message, ex.StackTrace));
                request.Errors.Add(new JwtRequestError("Error while validating the payload."));
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
            throw new SignatureVerificationException("Claim value must be an integer.");
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
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException("token");

            var parts = token.Split('.');
            if (parts.Length != 3)
            {
                throw new ArgumentException("Token must consist from 3 delimited by dot parts");
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
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException("token");

            var parts = token.Split('.');
            if (parts.Length != 3)
            {
                throw new ArgumentException("Token must consist from 3 delimited by dot parts");
            }

            var payLoad = parts[1];
            var payLoadJson = Encoding.UTF8.GetString(JsonWebToken.Base64UrlDecode(payLoad));

            return JsonConvert.DeserializeObject<T>(payLoadJson);
        }


    }
}