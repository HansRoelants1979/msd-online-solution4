using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;
using Tc.Crm.UnitTests.CustomWorkFlowSteps;

namespace Tc.Crm.CustomWorkflowSteps.ProcessCustomer.Services.Tests
{
    [TestClass()]
    public class AccountHelperTests
    {
        TestTracingService trace;
        [TestInitialize()]
        public void Setup()
        {
            trace = new TestTracingService();
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Customer payload is null.")]
        public void GetAccountEntityForCustomerPayload_CustomerIsNull()
        {
            AccountHelper.GetAccountEntityForCustomerPayload(null, trace);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Tracing service is null.")]
        public void GetAccountEntityForCustomerPayload_TraceIsNull()
        {
            AccountHelper.GetAccountEntityForCustomerPayload(new Customer(), null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Customer Idenifier is null.")]
        public void GetAccountEntityForCustomerPayload_CustomerIdentifierIsNull()
        {
            AccountHelper.GetAccountEntityForCustomerPayload(new Customer(), trace);
        }

        Customer c = new Customer
        {
            CustomerIdentifier = new CustomerIdentifier
            {
                BusinessArea = "Hotel",
                SourceMarket = Guid.NewGuid().ToString(),
                SourceSystem = "On Tour"
            },
            Company = new Company
            {

            },
            Address = new Address[]
                {
                    new Address { Street = "dummy", Town = "dummy", Country = "12345678-1234-1234-1234-123456789012" },                    
                },           
            Phone = new Phone[]
                {
                    new Phone { Number = "dummy"},
                    new Phone { Number = "dummy"},
                    new Phone { Number = "dummy"}
                },
            Email = new Email[]
                {
                    new Email { Address = "dummy" },
                    new Email { Address = "dummy" },
                    new Email { Address = "dummy" }
                }
        };

        [TestMethod()]
        public void GetAccountEntityForCustomerPayload_CustomerIdIsNull()
        {           
            AccountHelper.GetAccountEntityForCustomerPayload(c, trace);
        }

        [TestMethod()]
        public void GetAccountEntityForCustomerPayload_CompanyNameIsNull()
        {           
            AccountHelper.GetAccountEntityForCustomerPayload(c, trace);
        }

        [TestMethod()]
        public void GetAccountEntityForCustomerPayload_CompanyIsNull()
        {            
            AccountHelper.GetAccountEntityForCustomerPayload(c, trace);
        }

        [TestMethod()]
        public void GetAccountEntityForCustomerPayload_PopulatePhone()
        {
            var account = AccountHelper.GetAccountEntityForCustomerPayload(c, trace);
            Assert.AreEqual(c.Phone[0].Number, account[Attributes.Account.Telephone1].ToString());
            Assert.AreEqual(c.Phone[1].Number, account[Attributes.Account.Telephone2].ToString());
            Assert.AreEqual(c.Phone[2].Number, account[Attributes.Account.Telephone3].ToString());
        }

        [TestMethod()]
        public void GetAccountEntityForCustomerPayload_PopulateEmail()
        {
            var account = AccountHelper.GetAccountEntityForCustomerPayload(c, trace);
            Assert.AreEqual(c.Email[0].Address, account[Attributes.Account.EmailAddress1].ToString());
            Assert.AreEqual(c.Email[1].Address, account[Attributes.Account.EmailAddress2].ToString());
            Assert.AreEqual(c.Email[2].Address, account[Attributes.Account.EmailAddress3].ToString());
        }

        [TestMethod()]
        public void GetAccountEntityForCustomerPayload_PopulateAddress()
        {
            var account = AccountHelper.GetAccountEntityForCustomerPayload(c, trace);
            Assert.AreEqual(c.Address[0].Street, account[Attributes.Account.Address1Street].ToString());
            Assert.AreEqual(c.Address[0].Street, account[Attributes.Account.Address1Town].ToString());            
        }
    }
}
