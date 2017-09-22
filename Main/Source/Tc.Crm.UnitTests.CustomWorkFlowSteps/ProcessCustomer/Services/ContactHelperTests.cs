using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;
using Tc.Crm.UnitTests.CustomWorkFlowSteps;

namespace Tc.Crm.CustomWorkflowSteps.ProcessCustomer.Services.Tests
{
    [TestClass()]
    public class ContactHelperTests
    {
        TestTracingService trace;
        [TestInitialize()]
        public void Setp()
        {
            trace = new TestTracingService();
        }
        [TestMethod()]
        public void GetContactEntityForCustomerPayload_CustomerIsNull()
        {
            var customer = ContactHelper.GetContactEntityForCustomerPayload(null, trace, "POST");
            Assert.IsNull(customer);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Tracing service is null.")]
        public void GetContactEntityForCustomerPayload_TraceIsNull()
        {            
            ContactHelper.GetContactEntityForCustomerPayload(new Customer(), null, "POST");
        }

        [TestMethod()]
        public void GetContactEntityForCustomerPayload_CustomerIdentifierIsNull()
        {
            var contact = ContactHelper.GetContactEntityForCustomerPayload(new Customer(), trace, "POST");
            Assert.IsNotNull(contact);
            Assert.AreEqual("", contact[Attributes.Contact.SourceSystemId].ToString());
        }

        [TestMethod()]
        public void GetContactEntityForCustomerPayload_CustomerIdIsNull()
        {
            Customer c = new Customer
            {
                CustomerIdentifier = new CustomerIdentifier
                {
                    BusinessArea = "Hotel",
                    SourceMarket = Guid.NewGuid().ToString(),
                    SourceSystem = "On Tour"
                }
            };
            var contact = ContactHelper.GetContactEntityForCustomerPayload(c, trace, "POST");
            Assert.IsNotNull(contact);
            Assert.AreEqual("", contact[Attributes.Contact.SourceSystemId].ToString());
        }

        [TestMethod()]
        public void GetContactEntityForCustomerPayload_PopulateIdentity()
        {
            Customer c = new Customer
            {
                CustomerIdentifier = new CustomerIdentifier
                {
                    BusinessArea = "Hotel",
                    CustomerId = "CONT001",
                    SourceMarket = Guid.NewGuid().ToString(),
                    SourceSystem = "On Tour"
                },
                CustomerIdentity = new CustomerIdentity
                {
                    FirstName = "Joe",
                    LastName = "Blog",                    
                    Birthdate = "1982-10-08",
                    Gender = Gender.Female,
                    Language = "English",                   
                    Salutation = "Mr"
                }
            };
            var contact = ContactHelper.GetContactEntityForCustomerPayload(c, trace, "POST");            
            Assert.AreEqual(c.CustomerIdentity.FirstName, contact[Attributes.Contact.FirstName].ToString());
            Assert.AreEqual(c.CustomerIdentity.LastName, contact[Attributes.Contact.LastName].ToString());            
            Assert.AreEqual(950000006, ((OptionSetValue)(contact[Attributes.Contact.Language])).Value);
            Assert.AreEqual(950000000, ((OptionSetValue)(contact[Attributes.Contact.Salutation])).Value);           
        }
    }
}
