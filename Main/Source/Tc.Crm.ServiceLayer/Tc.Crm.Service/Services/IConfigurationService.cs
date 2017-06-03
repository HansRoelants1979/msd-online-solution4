using System.Collections.ObjectModel;

namespace Tc.Crm.Service.Services
{
    public interface IConfigurationService
    {
        string GetPublicKey(string fileName);
        Collection<string> GetPublicKeyFileNames(Api contextApi);
        string GetSecretKey();
        string GetIssuedAtTimeExpiryInSeconds();
    }
}
