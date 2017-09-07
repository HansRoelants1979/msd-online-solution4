using Tc.Crm.Common.Jti.Models;

namespace Tc.Crm.Common.Jti.Service
{
    public interface IJwtService
    {
        string CreateJwtToken(string privateKey, OwrJsonWebTokenPayload payloadObj);
        string SendHttpRequest(string serviceUrl, string token, string data);
        double GetExpiry(string expiredSeconds);
        double GetIssuedAtTime();
        double GetNotBeforeTime(string notBeforeSeconds);
    }
}
