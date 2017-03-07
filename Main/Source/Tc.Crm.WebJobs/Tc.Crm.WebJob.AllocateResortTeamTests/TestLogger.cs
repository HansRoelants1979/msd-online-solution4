using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.Common.Services;

namespace Tc.Crm.WebJob.AllocateResortTeamTests
{
    public class TestLogger : ILogger
    {
        public string FormatMessage(string message)
        {
            return string.Empty;
        }

        public void LogError(string message)
        {
            return;
        }

        public void LogInformation(string message)
        {
            return;
        }

        public void LogWarning(string message)
        {
            return;
        }
    }
}
