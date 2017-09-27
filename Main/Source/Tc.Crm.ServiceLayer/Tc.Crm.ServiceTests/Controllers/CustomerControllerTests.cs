using FakeXrmEasy;
using JsonPatch;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using System.Net;
using System.Web.Http;
using System.Web.Http.Hosting;
using Tc.Crm.Service.CacheBuckets;
using Tc.Crm.Service.Constants;
using Tc.Crm.Service.Controllers;
using Tc.Crm.Service.Models;
using Tc.Crm.Service.Services;

namespace Tc.Crm.ServiceTests.Controllers.Tests
{
    [TestClass()]
    public class CustomerControllerTests
    {
        XrmFakedContext context;
        ICustomerService customerService;
        ICrmService crmService;
        CustomerController controller;
        Customer customer;

        [TestInitialize()]
        public void TestSetup()
        {
            context = new XrmFakedContext();
            crmService = new TestCrmService(context);
            customerService =new CustomerService(new CountryBucket(crmService), new SourceMarketBucket(crmService));
            controller = new CustomerController(customerService, crmService);
            controller.Request = new System.Net.Http.HttpRequestMessage();
            controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());


            customer = new Customer();
            customer.CustomerGeneral = new CustomerGeneral();
            customer.CustomerGeneral.CustomerType = CustomerType.Person;
            customer.CustomerIdentifier = new CustomerIdentifier();
            customer.CustomerIdentifier.CustomerId = "123";
            customer.CustomerIdentity = new CustomerIdentity();
            customer.CustomerIdentity.FirstName = "Mock";
            customer.CustomerIdentity.LastName = "Test";
            customer.CustomerIdentity.Salutation = "Mr";
            customer.CustomerIdentity.Language = "EN";
            customer.CustomerIdentity.Birthdate = "01-01-1990";
        }

        [TestMethod()]
        public void CreateCustomerIsNull()
        {
            var response = controller.Create(null);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
            Assert.AreEqual(((System.Net.Http.ObjectContent)response.Content).Value, Messages.CustomerDataPassedIsNullOrCouldNotBeParsed);
        }

        [TestMethod()]
        public void UpdateCustomerIsNull()
        {
            JsonPatchDocument<CustomerInformation> customerInfo = null;
            var response = controller.Update(string.Empty, customerInfo);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
            Assert.AreEqual(((System.Net.Http.ObjectContent)response.Content).Value, Messages.CustomerIdIsNull);
        }

        [TestMethod()]
        public void CreateCustomerGeneralIsNull()
        {
            CustomerInformation customerInfo = new CustomerInformation();
            Customer customer = new Customer();
            customerInfo.Customer = customer;
            var response = controller.Create(customerInfo);
            Collection<string> messages = new Collection<string>();
            messages.Add(Messages.CustomerIdIsNull);
            messages.Add(Messages.CustomerGeneralNotPresent);
            messages.Add(Messages.CustomerIdentityNotPresent);
            var expectedMessage = customerService.GetStringFrom(messages);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual(expectedMessage, ((System.Net.Http.ObjectContent)response.Content).Value);
        }

        [TestMethod()]
        public void UpdateCustomerGeneralIsNull()
        {
            JsonPatchDocument<CustomerInformation> customerInformation = new JsonPatchDocument<CustomerInformation>();
            
            var response = controller.Update("1234", customerInformation);
            Collection<string> messages = new Collection<string>();
            messages.Add(Messages.CustomerTypeNotPresent);
            var expectedMessage = customerService.GetStringFrom(messages);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual(expectedMessage, ((System.Net.Http.ObjectContent)response.Content).Value);
        }

        [TestMethod()]
        public void CreateCustomerIdentifierIsNull()
        {
            CustomerInformation customerInfo = new CustomerInformation();
            Customer customer = new Customer();
            customerInfo.Customer = customer;
            customerInfo.Customer.CustomerGeneral = new CustomerGeneral();
            customerInfo.Customer.CustomerIdentifier = new CustomerIdentifier();
            customerInfo.Customer.CustomerIdentity = new CustomerIdentity();
            var response = controller.Create(customerInfo);
            Collection<string> messages = new Collection<string>();
            messages.Add(Messages.CustomerIdIsNull);
            messages.Add(Messages.CustomerTypeNotPresent);
            var expectedMessage = customerService.GetStringFrom(messages);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual(expectedMessage, ((System.Net.Http.ObjectContent)response.Content).Value);
        }

        [TestMethod()]
        public void CreateActionResponseIsNull()
        {
            CustomerInformation customerInfo = new CustomerInformation();
            customerInfo.Customer = customer;
            TestCrmService service = new TestCrmService(context);
            service.Switch = DataSwitch.Response_NULL;
            controller = new CustomerController(customerService, service);
            controller.Request = new System.Net.Http.HttpRequestMessage();
            controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            var response = controller.Create(customerInfo);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError);
        }

        [TestMethod()]
        public void CustomerCreated()
        {
            CustomerInformation customerInfo = new CustomerInformation();
            customerInfo.Customer = customer;
            TestCrmService service = new TestCrmService(context);
            service.Switch = DataSwitch.Created;
            controller = new CustomerController(customerService, service);
            controller.Request = new System.Net.Http.HttpRequestMessage();
            controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            var response = controller.Create(customerInfo);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
        }

        [TestMethod()]
        public void CustomerUpdated()
        {
            JsonPatchDocument<CustomerInformation> customerInfo = new JsonPatchDocument<CustomerInformation>();
            customerInfo.Add("Customer/CustomerIdentity/FirstName", "John");
            customerInfo.Add("Customer/CustomerGeneral/CustomerType", "Person");
            TestCrmService service = new TestCrmService(context);
            service.Switch = DataSwitch.Updated;
            controller = new CustomerController(customerService, service);
            controller.Request = new System.Net.Http.HttpRequestMessage();
            controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            var response = controller.Update("123",customerInfo);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NoContent);
        }
    }
}
