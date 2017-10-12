using System;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;
using System.Collections.Generic;

namespace Tc.Crm.CustomWorkflowSteps.ProcessCustomer.Services.Tests
{
    [TestClass]
    public class AccountPatchHelperTests
    {
        private ITracingService tracingService;
        private Customer Testcustomer = new Customer
        {
            CustomerIdentifier = new CustomerIdentifier
            {
                CustomerId = "Test customer"
            },
            Address = new[]{
                    new Address() {
                        FlatNumberUnit ="1234",
                        AddressType =AddressType.Main,
                        AdditionalAddressInfo =null,
                        Country =null,
                        County="",
                    }
                },
            Email = new[]{
                    new Email() {Address="1234",EmailType=EmailType.NotSpecified },
                    new Email() {Address="45678",EmailType=EmailType.Primary },
                    new Email() {Address="67890",EmailType=EmailType.Promo }
                },
            Phone = new[]{
                    new Phone() {Number="1234",PhoneType=PhoneType.Business },
                    new Phone() {Number="45678",PhoneType=PhoneType.Home },
                    new Phone() {Number="67890",PhoneType=PhoneType.NotSpecified }
                },
            PatchParameters = new List<string>() { "tc_address1", "tc_phone1", "tc_email1" },
            Company = new Company() { CompanyName = "Test" }
        };
        [TestInitialize]
        public void Setup()
        {
            tracingService = A.Fake<ITracingService>();

            A.CallTo(() => tracingService.Trace(A<string>._)).DoesNothing();
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Customer payload is null.")]
        public void GetAccountEntityForCustomerPayload_CustomerIsNull()
        {
            AccountPatchHelper.GetAccountEntityForCustomerPayload(null, tracingService);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Tracing service is null.")]
        public void GetAccountEntityForCustomerPayload_TraceIsNull()
        {
            AccountPatchHelper.GetAccountEntityForCustomerPayload(new Customer(), null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Customer Idenifier is null.")]
        public void GetAccountEntityForCustomerPayload_CustomerIdentifierIsNull()
        {
            AccountPatchHelper.GetAccountEntityForCustomerPayload(new Customer(), tracingService);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Customer Identifier could not be retrieved from payload.")]
        public void GetAccountEntityForCustomerPayload_CustomerIdIsNull()
        {
            AccountPatchHelper.GetAccountEntityForCustomerPayload(new Customer(), tracingService);
        }
        [TestMethod]
        public void GetAccountEntityForCustomerPayload_CompanyNameIsNull()
        {
            // Given
            var customer = Testcustomer;
            customer.Company = null;
            // When        
            var result = AccountPatchHelper.GetAccountEntityForCustomerPayload(customer, tracingService);

            // Then
            Assert.IsFalse(result.Contains(Attributes.Account.Name));
        }

        [TestMethod]
        public void GetAccountEntityForCustomerPayload_CompanyIsNotProvided()
        {
            // Given
            var customer = Testcustomer;
            customer.Company.CompanyName = "";
            // When        
            var result = AccountPatchHelper.GetAccountEntityForCustomerPayload(customer, tracingService);

            // Then
            Assert.IsFalse(result.Contains(Attributes.Account.Name));
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Customer Patch parameter is null.")]
        public void GetAccountEntityForCustomerPayload_PatchParametersIsNotProvided()
        {
            // Given
            var customer = new Customer
            {
                PatchParameters = null 
            };

            AccountPatchHelper.GetAccountEntityForCustomerPayload(customer, tracingService);
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Customer Patch parameter is null.")]
        public void GetAccountEntityForCustomerPayload_PatchParametersCount()
        {
            // Given
            var customer = new Customer
            {
                PatchParameters = new List<string>()
            };

            AccountPatchHelper.GetAccountEntityForCustomerPayload(customer, tracingService);
        }

        [TestMethod]
        public void GetAccountEntityForCustomerPayload_SourceMarketIsNull()
        {
            // Given
            var customer = Testcustomer;
            customer.CustomerIdentifier.SourceMarket = null;
            // When        
            var result = AccountPatchHelper.GetAccountEntityForCustomerPayload(customer, tracingService);

            // Then
            Assert.IsFalse(result.Contains(Attributes.Account.Name));
        }
        [TestMethod]
        public void GetAccountEntityForCustomerPayload_EmailIsNull()
        {
            // Given
            var customer = Testcustomer;
            customer.Email = null;
            // When        
            var result = AccountPatchHelper.GetAccountEntityForCustomerPayload(customer, tracingService);
            // Then
            Assert.IsFalse(result.Contains(Attributes.Account.Name));
        }
        [TestMethod]
        public void GetAccountEntityForCustomerPayload_PhoneIsNull()
        {
            // Given
            var customer =   Testcustomer;
            customer.Phone = null;
            // When        
            var result = AccountPatchHelper.GetAccountEntityForCustomerPayload(customer, tracingService);
            // Then
            Assert.IsFalse(result.Contains(Attributes.Account.Name));
        }
        [TestMethod]
        public void GetAccountEntityForCustomerPayload_AddressIsNull()
        {
            // Given
            var customer =  Testcustomer;
            customer.Address = null;
            // When        
            var result = AccountPatchHelper.GetAccountEntityForCustomerPayload(customer, tracingService);
            // Then
            Assert.IsFalse(result.Contains(Attributes.Account.Name));
        }
        [TestMethod]
        public void GetAccountEntityForCustomerPayload_EmailCountLessThanZero()
        {
            // Given
            var customer =   Testcustomer;
            customer.Email[1] = null;
            // When        
            var result = AccountPatchHelper.GetAccountEntityForCustomerPayload(customer, tracingService);
            // Then
            Assert.IsFalse(result.Contains(Attributes.Account.EmailAddress2));
        }
        [TestMethod]
        public void GetAccountEntityForCustomerPayload_PhoneCountLessThanZero()
        {
            // Given
            var customer = Testcustomer;
            customer.Phone[1] = null;
            // When        
            var result = AccountPatchHelper.GetAccountEntityForCustomerPayload(customer, tracingService);
            // Then
            Assert.IsFalse(result.Contains(Attributes.Account.Telephone2));
        }
        [TestMethod]
        public void GetAccountEntityForCustomerPayload_AddressCountLessThanZero()
        {
            // Given
            var customer = Testcustomer;
            customer.Address[1] = null;
            // When        
            var result = AccountPatchHelper.GetAccountEntityForCustomerPayload(customer, tracingService);
            // Then
            Assert.IsFalse(result.Contains(Attributes.Account.Address1AdditionalInformation));
        }

        [TestMethod]
        public void GetAccountEntityForCustomerPayload_PopulateEmailField()
        {
            // Given
            var customer = Testcustomer;

            // When        
            var result = AccountPatchHelper.GetAccountEntityForCustomerPayload(customer, tracingService);
            // Then
            Assert.IsFalse(result.Contains(Attributes.Account.Name));
        }
        [TestMethod]
        public void GetAccountEntityForCustomerPayload_PopulatePhoneField()
        {
            // Given
            var customer = Testcustomer;

            // When        
            var result = AccountPatchHelper.GetAccountEntityForCustomerPayload(customer, tracingService);
            // Then
            Assert.IsFalse(result.Contains(Attributes.Account.Name));
        }
        [TestMethod]
        public void GetAccountEntityForCustomerPayload_PopulateAddressField()
        {
            // Given
            var customer = Testcustomer;

            // When        
            var result = AccountPatchHelper.GetAccountEntityForCustomerPayload(customer, tracingService);
            // Then
            Assert.IsFalse(result.Contains(Attributes.Account.Name));
        }

    }
}
