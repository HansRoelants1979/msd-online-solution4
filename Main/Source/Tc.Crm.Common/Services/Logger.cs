using System;
using System.Diagnostics;

namespace Tc.Crm.Common.Services
{
    public class Logger : ILogger
    {
        public void LogError(string message)
        {
            Trace.TraceError(FormatMessage(message));
        }

        public void LogInformation(string message)
        {
            Trace.TraceInformation(FormatMessage(message));
        }

        public void LogWarning(string message)
        {
            Trace.TraceWarning(FormatMessage(message));
        }

        /// <summary>
        /// Appends timestamp at the end of the message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public string FormatMessage(string message)
        {
            return string.Format(message + " on ({0})", DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
        }

    }
}
