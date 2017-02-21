using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.Common.Services
{
    public class Logger : ILogger
    {
        public void LogError(string message)
        {
            Trace.TraceError(message);
        }

        public void LogInformation(string message)
        {
            Trace.TraceInformation(message);
        }

        public void LogWarning(string message)
        {
            Trace.TraceWarning(message);
        }
    }
}
