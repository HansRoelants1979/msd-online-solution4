using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.Common.Services;

namespace Tc.Crm.WebJob.DeallocateResortTeamTests
{
    public class TestLogger : ILogger
    {
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
