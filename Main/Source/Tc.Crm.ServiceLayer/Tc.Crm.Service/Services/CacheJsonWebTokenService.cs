using JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.Services
{
    public class CacheJsonWebTokenService:JsonWebTokenServiceBase
    {
        public CacheJsonWebTokenService(IConfigurationService configurationService)
        {
            this.ConfigurationService = configurationService;
        }
        public override void ValidateHeader(JsonWebTokenRequest request)
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

        public override void ValidateSignature(JsonWebTokenRequest jsonWebTokenRequest)
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
                var key = ConfigurationService.GetSecretKey();
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

        
    }
}