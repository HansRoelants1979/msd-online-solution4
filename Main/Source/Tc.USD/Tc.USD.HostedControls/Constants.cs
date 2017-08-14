namespace Tc.Usd.HostedControls.Constants
{
    public class ActionName
    {
        public const string OpenOwr = "Tc.Usd.SingleSignOnController.Custom.OpenOwr";
    }

    public class EventName
    {
        public const string SsoCompleteEvent = "Tc.Event.OnSsoComplete";
    }

    public class DataKey
    {
        public const string OwrUrlConfigName = "Tc.Owr.SsoServiceUrl";
        public const string DiagnosticSource = "Tc.Usd.SessionCustomActions";
        public const string JwtPrivateKeyConfigName = "Tc.Owr.JwtPrivateKey";
        public const string SsoTokenExpired = "Tc.Owr.SsoTokenExpiredSeconds";
        public const string SsoTokenNotBefore = "Tc.Owr.SsoTokenNotBeforeTimeSeconds";
        public const string AudOneWebRetail = "onewebretail";
        public const string AudWebRio = "webrio";
    }
}