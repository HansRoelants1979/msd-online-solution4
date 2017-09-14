namespace Tc.Crm.Common.IntegrationLayer
{
    internal class DataKey
    {
        public const string OutboundSynchronisationUrlConfigName = "Tc.OutboundSynchronisation.SsoServiceUrl";
        public const string JwtPrivateKeyConfigName = "Tc.OutboundSynchronisation.JwtPrivateKey";
        public const string SsoTokenExpired = "Tc.OutboundSynchronisation.SsoTokenExpiredSeconds";
        public const string SsoTokenNotBefore = "Tc.OutboundSynchronisation.SsoTokenNotBeforeTimeSeconds";
    }
}