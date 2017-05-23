namespace Tc.Crm.Service.Services
{
    public interface IConfigurationService
    {
        string GetPublicKey();
        string GetSecretKey();
        string GetIssuedAtTimeExpiryInSeconds();
    }
}
