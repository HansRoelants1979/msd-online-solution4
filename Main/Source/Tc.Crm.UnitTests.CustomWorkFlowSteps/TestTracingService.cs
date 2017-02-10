using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.UnitTests.CustomWorkFlowSteps
{
    public class TestTracingService : ITracingService
    {
        public void Trace(string format, params object[] args)
        {
            return;
        }
    }
}
