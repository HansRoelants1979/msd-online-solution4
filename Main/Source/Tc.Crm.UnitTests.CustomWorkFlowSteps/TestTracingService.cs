using Microsoft.Xrm.Sdk;

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
