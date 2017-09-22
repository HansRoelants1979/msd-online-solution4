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
        public void Setp()
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

        [TestMethod()]
        public void GetAccountEntityForCustomerPayload_CustomerIdIsNull()
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
            AccountHelper.GetAccountEntityForCustomerPayload(c, trace);
        }

        [TestMethod()]
        public void GetAccountEntityForCustomerPayload_CompanyNameIsNull()
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
                Company = new Company
                {

                }
            };
            AccountHelper.GetAccountEntityForCustomerPayload(c, trace);
        }

        [TestMethod()]
        public void GetAccountEntityForCustomerPayload_CompanyIsNull()
        {
            Customer c = new Customer
            {
                CustomerIdentifier = new CustomerIdentifier
                {
                    BusinessArea = "Hotel",
                    CustomerId = "CONT001",
                    SourceMarket = Guid.NewGuid().ToString(),
                    SourceSystem = "On Tour"
                }
            };
            AccountHelper.GetAccountEntityForCustomerPayload(c, trace);
        }
    }
}
