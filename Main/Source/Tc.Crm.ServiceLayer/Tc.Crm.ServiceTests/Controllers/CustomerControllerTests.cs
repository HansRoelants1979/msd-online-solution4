using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tc.Crm.Service.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.Service.Services;
using Tc.Crm.ServiceTests;
using FakeXrmEasy;
using System.Net;
using System.Web.Http.Hosting;
using System.Web.Http;
using Microsoft.Xrm.Sdk;

namespace Tc.Crm.Service.Controllers.Tests
{
    [TestClass()]
    public class CustomerControllerTests
    {
        XrmFakedContext context;
        ICustomerService customerService;
        CustomerController controller;
        ICrmService crmService;
        Entity testEntity1;
        [TestInitialize()]
        public void TestSetup()
        {
            context = new XrmFakedContext();
            context.Data = new Dictionary<string, Dictionary<Guid, Microsoft.Xrm.Sdk.Entity>>();
            context.Data.Add("contact", new Dictionary<Guid, Microsoft.Xrm.Sdk.Entity>());
            testEntity1 = new Entity("contact");
            testEntity1.Id = Guid.NewGuid();
            testEntity1["firstname"] = "Axe";
            testEntity1["lastname"] = "Him";
            testEntity1["emailaddress1"] = "Axe@tc.com";
            testEntity1["new_sourcekey"] = "CON002";
            context.Data["contact"].Add(testEntity1.Id, testEntity1);

            customerService = new CustomerService();
            crmService = new TestCrmService(context);
            controller = new CustomerController(customerService,crmService);
            controller.Request = new System.Net.Http.HttpRequestMessage();
            controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
        }

        [TestMethod()]
        public void UpdateTest_CustomerIsNull()
        {
            var response = controller.Update(null);
            Assert.AreEqual(response.StatusCode,HttpStatusCode.InternalServerError);
        }

        [TestMethod()]
        public void UpdateTest_CustomerIdIsNull()
        {
            var response = controller.Update(new Models.Customer { Id = null });
            Task<string> task = response.Content.ReadAsStringAsync();
            var content = task.Result;
            Assert.AreEqual("\"Source key is empty or null.\"", content);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
        [TestMethod()]
        public void UpdateTest_CustomerIdIsEmpty()
        {
            var response = controller.Update(new Models.Customer { Id=string.Empty});
            Task<string> task = response.Content.ReadAsStringAsync();
            var content = task.Result;
            Assert.AreEqual("\"Source key is empty or null.\"", content);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod()]
        public void UpdateTest_CustomerCreate()
        {
            Models.Customer customer = new Models.Customer
            {
                Email = "joliver@tc.com",
                FirstName = "John",
                LastName = "Oliver",
                Id = "CON001"
            };

            var response = controller.Update(customer);
            Task<string> task = response.Content.ReadAsStringAsync();
            var content = task.Result;
            content = content.Trim('"');
            Entity expectedValue = null;
            if (context.Data["contact"].TryGetValue(new Guid(content), out expectedValue))
            {
                Assert.IsTrue(true);
            }
            else
                Assert.IsTrue(false);
        }

        [TestMethod()]
        public void UpdateTest_CustomerUpdate()
        {
            Models.Customer customer = new Models.Customer
            {
                Email = "Axe.Her@tc.com",
                FirstName = "Axe",
                LastName = "Her",
                Id = "CON002"
            };

            var response = controller.Update(customer);

            var expected = context.Data["contact"][testEntity1.Id];

            Assert.AreEqual(customer.Email,expected["emailaddress1"].ToString());
            Assert.AreEqual(customer.LastName, expected["lastname"].ToString());
        }
    }
}