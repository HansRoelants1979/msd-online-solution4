namespace Tc.Crm.Common.Services
{
    public interface ILogger
    {
        void LogError(string message);
        void LogWarning(string message);
        void LogInformation(string message);
        string FormatMessage(string message);
    }
}
