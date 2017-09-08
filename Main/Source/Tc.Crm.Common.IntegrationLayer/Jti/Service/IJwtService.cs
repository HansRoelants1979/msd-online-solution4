using Tc.Crm.Common.IntegrationLayer.Jti.Models;

namespace Tc.Crm.Common.IntegrationLayer.Jti.Service
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
