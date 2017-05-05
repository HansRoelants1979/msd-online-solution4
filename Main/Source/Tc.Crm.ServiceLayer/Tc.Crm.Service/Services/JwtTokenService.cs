using System.Collections.Generic;
using System.Security.Cryptography;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.Services
{
    public class JwtTokenService:IJwtTokenService
    {
        public string CreateJWTToken(Token token)
        {
            var payload = new Dictionary<string, object>()
            {
                {"iat", token.IssuedAtTime},
                {"nbf", token.NotBeforeTime},
                {"exp", token.Expiry},
            };

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(token.PrivateKey);

            return Jose.JWT.Encode(payload, rsa, Jose.JwsAlgorithm.RS256);
        }
    }
}