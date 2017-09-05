using Tc.Crm.Common.Services;

namespace Tc.Crm.OutboundSynchronisation.CustomerTests
{
    public class TestLogger : ILogger
    {
        public string FormatMessage(string message)
        {
            return string.Empty;
        }

        public void LogError(string message)
        {
        }

        public void LogInformation(string message)
        {
        }

        public void LogWarning(string message)
        {
        }
    }
}