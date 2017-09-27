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
        [ExpectedException(typeof(InvalidPluginExecutionException), "Customer is null.")]
        public void GetContactEntityForCustomerPayload_CustomerIsNull()
        {            
            ContactHelper.GetContactEntityForCustomerPayload(null, trace, "POST");            
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Tracing service is null.")]
        public void GetContactEntityForCustomerPayload_TraceIsNull()
        {            
            ContactHelper.GetContactEntityForCustomerPayload(new Customer(), null, "POST");
        }

        Customer c = new Customer
        {
            CustomerIdentifier = new CustomerIdentifier
            {
                CustomerId ="",
                BusinessArea = "Hotel",
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
            },
            Address = new Address[]
                {
                    new Address { Street = "dummy", Town = "dummy", Country = "12345678-1234-1234-1234-123456789012" },
                    new Address { Street = "dummy", Town = "dummy", Country = "12345678-1234-1234-1234-123456789012" },
                },
            Permission = new Permission
            {              
                DoNotContactInd = "true",
                EmailAllowedInd = "true",
                MailAllowedInd = "true",
                PhoneAllowedInd = "true",
                SmsAllowedInd = "true",
                PreferredContactMethod ="Email"               
            },
            Phone = new Phone[]
                {
                    new Phone { Number = "dummy"},
                    new Phone { Number ="dummy"},
                    new Phone { Number ="dummy"}
                },
            Email = new Email[]
                {
                    new Email { Address = "dummy" },
                    new Email { Address ="dummy"},
                    new Email { Address ="dummy"}
                }
        };

        [TestMethod()]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Customer Identifier is null.")]
        public void GetContactEntityForCustomerPayload_CustomerIdentifierIsNull()
        {
            c.CustomerIdentifier = null;
            ContactHelper.GetContactEntityForCustomerPayload(c, trace, "POST");                      
        }        

        [TestMethod()]
        public void GetContactEntityForCustomerPayload_PopulateIdentity()
        {            
            var contact = ContactHelper.GetContactEntityForCustomerPayload(c, trace, "POST");            
            Assert.AreEqual(c.CustomerIdentity.FirstName, contact[Attributes.Contact.FirstName].ToString());
            Assert.AreEqual(c.CustomerIdentity.LastName, contact[Attributes.Contact.LastName].ToString());            
            Assert.AreEqual(950000006, ((OptionSetValue)(contact[Attributes.Contact.Language])).Value);
            Assert.AreEqual(950000000, ((OptionSetValue)(contact[Attributes.Contact.Salutation])).Value);           
        }

        [TestMethod()]
        public void GetContactEntityForCustomerPayload_PopulateAddress()
        {                       
            var contact = ContactHelper.GetContactEntityForCustomerPayload(c, trace, "POST");
            Assert.AreEqual(c.Address[0].Street, contact[Attributes.Contact.Address1Street].ToString());
            Assert.AreEqual(c.Address[0].Street, contact[Attributes.Contact.Address1Town].ToString());
            Assert.AreEqual(c.Address[1].Street, contact[Attributes.Contact.Address2Street].ToString());
            Assert.AreEqual(c.Address[1].Street, contact[Attributes.Contact.Address2Street].ToString());            
        }

        [TestMethod()]
        public void GetContactEntityForCustomerPayload_PopulatePermission()
        {          
            var contact = ContactHelper.GetContactEntityForCustomerPayload(c, trace, "POST");
            Assert.AreEqual(new OptionSetValue(950000000), contact[Attributes.Contact.MarketingByPhone]);
            Assert.AreEqual(new OptionSetValue(950000000), contact[Attributes.Contact.SendMarketingByEmail]);
            Assert.AreEqual(new OptionSetValue(950000000), contact[Attributes.Contact.SendMarketingByPost]);
            Assert.AreEqual(new OptionSetValue(950000000), contact[Attributes.Contact.ThomasCookMarketingConsent]);
            Assert.AreEqual(new OptionSetValue(950000000), contact[Attributes.Contact.SendMarketingBySms]);
            Assert.AreEqual(new OptionSetValue(2), contact[Attributes.Contact.PreferredContactMethodCode]);
        }

        [TestMethod()]
        public void GetContactEntityForCustomerPayload_PopulatePhone()
        {
            var contact = ContactHelper.GetContactEntityForCustomerPayload(c, trace, "POST");
            Assert.AreEqual(c.Phone[0].Number, contact[Attributes.Contact.Telephone1].ToString());
            Assert.AreEqual(c.Phone[1].Number, contact[Attributes.Contact.Telephone2].ToString());
            Assert.AreEqual(c.Phone[2].Number, contact[Attributes.Contact.Telephone3].ToString());
        }

        [TestMethod()]
        public void GetContactEntityForCustomerPayload_PopulateEmail()
        {            
            var contact = ContactHelper.GetContactEntityForCustomerPayload(c, trace, "POST");
            Assert.AreEqual(c.Email[0].Address, contact[Attributes.Contact.EmailAddress1].ToString());
            Assert.AreEqual(c.Email[1].Address, contact[Attributes.Contact.EmailAddress2].ToString());
            Assert.AreEqual(c.Email[2].Address, contact[Attributes.Contact.EmailAddress3].ToString());
        }

    }
}
