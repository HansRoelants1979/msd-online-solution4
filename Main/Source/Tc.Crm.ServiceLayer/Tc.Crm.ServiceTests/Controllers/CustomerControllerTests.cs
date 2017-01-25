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
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException),"customer")]
        public void UpdateTest_CustomerIsNull()
        {
            CustomerService cs = new CustomerService();
            CustomerController c = new CustomerController(cs,new TestCrmService(new XrmFakedContext()));
            c.Update(null);
            Assert.Fail();
        }

        [TestMethod()]
        public void UpdateTest_CustomerIdIsNull()
        {
            CustomerService cs = new CustomerService();
            CustomerController c = new CustomerController(cs, new TestCrmService(new XrmFakedContext()));
            c.Request = new System.Net.Http.HttpRequestMessage();
            c.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            var response = c.Update(new Models.Customer { Id = null });
            Task<string> task = response.Content.ReadAsStringAsync();
            var content = task.Result;
            Assert.AreEqual("\"Source key is empty or null.\"", content);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
        [TestMethod()]
        public void UpdateTest_CustomerIdIsEmpty()
        {
            CustomerService cs = new CustomerService();
            CustomerController c = new CustomerController(cs, new TestCrmService(new XrmFakedContext()));
            c.Request = new System.Net.Http.HttpRequestMessage();
            c.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            var response = c.Update(new Models.Customer { Id=string.Empty});
            Task<string> task = response.Content.ReadAsStringAsync();
            var content = task.Result;
            Assert.AreEqual("\"Source key is empty or null.\"", content);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod()]
        public void UpdateTest_CustomerCreate()
        {
            var context = new XrmFakedContext();
            context.Data = new Dictionary<string, Dictionary<Guid, Microsoft.Xrm.Sdk.Entity>>();
            context.Data.Add("contact", new Dictionary<Guid, Microsoft.Xrm.Sdk.Entity>());
            CustomerService cs = new CustomerService();
            CustomerController c = new CustomerController(cs, new TestCrmService(context));
            c.Request = new System.Net.Http.HttpRequestMessage();
            c.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            
            Models.Customer customer = new Models.Customer
            {
                Email = "joliver@tc.com",
                FirstName = "John",
                LastName = "Oliver",
                Id = "CON001"
            };

            var response = c.Update(customer);
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
            var context = new XrmFakedContext();
            context.Data = new Dictionary<string, Dictionary<Guid, Microsoft.Xrm.Sdk.Entity>>();
            context.Data.Add("contact", new Dictionary<Guid, Microsoft.Xrm.Sdk.Entity>());
            Entity e = new Entity("contact");
            e.Id = Guid.NewGuid();
            e["firstname"] = "John";
            e["lastname"] = "John";
            e["emailaddress1"] = "John";
            e["new_sourcekey"] = "CON001";
            context.Data["contact"].Add(e.Id, e);

            CustomerService cs = new CustomerService();
            CustomerController c = new CustomerController(cs, new TestCrmService(context));
            c.Request = new System.Net.Http.HttpRequestMessage();
            c.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            Models.Customer customer = new Models.Customer
            {
                Email = "joliver@tc.com",
                FirstName = "John",
                LastName = "Oliver",
                Id = "CON001"
            };

            var response = c.Update(customer);

            var expected = context.Data["contact"][e.Id];

            Assert.AreEqual(customer.Email,expected["emailaddress1"].ToString());
            Assert.AreEqual(customer.LastName, expected["lastname"].ToString());
        }
    }
}