using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeXrmEasy;
using Tc.Crm.ServiceTests;
using System.Net;

namespace Tc.Crm.Service.Controllers.Tests
{
    [TestClass()]
    public class PingControllerTests
    {
        XrmFakedContext context;
        TestCrmService service;
        PingController controller;

        [TestInitialize()]
        public void TestSetup()
        {
            context = new XrmFakedContext();
            service = new TestCrmService(context);
            controller = new PingController(service);
            controller.Request = new System.Net.Http.HttpRequestMessage();
        }

        [TestMethod()]
        public void PingServiceAvailable()
        {
            service.Switch = DataSwitch.Updated;
            var response = controller.Ping();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod()]
        public void PingServiceUnavailable()
        {
            service.Switch = DataSwitch.Response_Failed;
            var response = controller.Ping();
            Assert.AreEqual(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        }
    }
}