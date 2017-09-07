using System.Diagnostics;
using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using Tc.Crm.Common.Services;

namespace Tc.Usd.HostedControls.Models
{
    public class UsdLogger:ILogger
    {
        private readonly TraceLogger _logWriter;

        public UsdLogger(TraceLogger logWriter)
        {
            _logWriter = logWriter;
        }

        public void LogError(string message)
        {
            _logWriter.Log(message, TraceEventType.Error);
        }

        public void LogWarning(string message)
        {
            _logWriter.Log(message, TraceEventType.Warning);
        }

        public void LogInformation(string message)
        {
            _logWriter.Log(message,TraceEventType.Information);
        }

        public string FormatMessage(string message)
        {
            return message;
        }
    }
}
