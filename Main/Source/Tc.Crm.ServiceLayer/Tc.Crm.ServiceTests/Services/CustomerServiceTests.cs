using FakeXrmEasy;
using JsonPatch;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.ObjectModel;
using Tc.Crm.Service.Constants;
using Tc.Crm.Service.Models;
using Tc.Crm.Service.Services;

namespace Tc.Crm.ServiceTests.Services.Tests
{
    [TestClass()]
    public class CustomerServiceTests
    {
        XrmFakedContext context;
        ICustomerService customerService;
        TestCrmService crmService;

        [TestInitialize()]
        public void TestSetup()
        {
            context = new XrmFakedContext();
            crmService = new TestCrmService(context);
            customerService = new CustomerService();
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException), Parameters.BookingData)]
        public void CustomerIsNull()
        {
            customerService.Create(null, crmService);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException), Parameters.BookingData)]
        public void CustomerIsEmpty()
        {
            customerService.Create(string.Empty, crmService);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException), Parameters.CrmService)]
        public void CrmCustomerServiceIsNull()
        {
            customerService.Create("some data", null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException), Messages.ResponseNull)]
        public void CustomerResponseIsNull()
        {
            crmService.Switch = DataSwitch.Return_NULL;
            customerService.Create("some data", crmService);
        }

        [TestMethod()]
        public void CustomerTypeNotPresentCheck()
        {
            CustomerInformation customerInformation = new CustomerInformation();
            customerInformation.Customer.CustomerGeneral = new CustomerGeneral();
            var validateMessage = customerService.ValidateCustomerPatchRequest(customerInformation);
            var message = customerService.GetStringFrom(validateMessage);
            Assert.AreEqual(Messages.CustomerTypeNotPresent, message);
        }



        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException), Parameters.CrmService)]
        public void PatchCustomerServiceIsNull()
        {
            CustomerInformation customerInformation = new CustomerInformation();
            customerService.Update("Test", customerInformation, null);
        }
    }
}
