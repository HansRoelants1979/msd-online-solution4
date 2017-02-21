using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Tc.Crm.WebJob.DeallocateResortTeam.Services
{
    public class Logger : ILogger
    {
        public void LogError(string message)
        {
            Trace.TraceError(message);
        }

        public void LogInformation(string message)
        {
            Trace.TraceError(message);
        }

        public void LogWarning(string message)
        {
            Trace.TraceError(message);
        }
    }
}
