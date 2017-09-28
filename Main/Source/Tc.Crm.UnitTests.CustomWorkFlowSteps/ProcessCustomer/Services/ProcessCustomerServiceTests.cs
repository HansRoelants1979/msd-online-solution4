using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Tc.Crm.CustomWorkflowSteps.ProcessCustomer.Models;
using Tc.Crm.CustomWorkflowSteps.ProcessCustomer.Services;
using Tc.Crm.UnitTests.CustomWorkFlowSteps;

namespace Tc.Crm.CustomWorkFlowSteps.ProcessCustomer.Services.Tests
{
    [TestClass()]
    public class ProcessCustomerServiceTests
    {

        TestTracingService trace; 
        [TestInitialize()]
        public void Setup()
        {
            trace = new TestTracingService();             
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Cannot create instance of Service - customer payload instance is null.")]
        public void ProcessCustomerService_PayloadCustomerNullCheck()
        {
            PayloadCustomer customer = null;
            ProcessCustomerService cs = new ProcessCustomerService(customer);
        }       
    }
}
