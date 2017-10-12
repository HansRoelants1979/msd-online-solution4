using System;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;
using System.Collections.Generic;

namespace Tc.Crm.CustomWorkflowSteps.ProcessCustomer.Services.Tests
{
    [TestClass]
    public class ContactPatchHelperTests
    {
        private ITracingService tracingService;
        private Customer Testcustomer = new Customer
        {
            CustomerIdentifier = new CustomerIdentifier
            {
                CustomerId = "Test customer",
                SourceMarket = Guid.NewGuid().ToString(),
            },
            CustomerIdentity = new CustomerIdentity() {
                AcademicTitle = "Mr",
                Birthdate = null,
                FirstName = "Test 1",
                Gender = Gender.Male,
                Language = null,
                Salutation =null
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
        public void GetContactEntityForCustomerPayload_CustomerIsNull()
        {
            ContactPatchHelper.GetContactEntityForCustomerPayload(null, tracingService);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Tracing service is null.")]
        public void GetContactEntityForCustomerPayload_TraceIsNull()
        {
            ContactPatchHelper.GetContactEntityForCustomerPayload(new Customer(), null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Customer Idenifier is null.")]
        public void GetContactEntityForCustomerPayload_CustomerIdentifierIsNull()
        {
            ContactPatchHelper.GetContactEntityForCustomerPayload(new Customer(), tracingService);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Customer Identifier could not be retrieved from payload.")]
        public void GetContactEntityForCustomerPayload_CustomerIdIsNull()
        {
            ContactPatchHelper.GetContactEntityForCustomerPayload(new Customer(), tracingService);
        }
        
         
        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Customer Patch parameter is null.")]
        public void GetContactEntityForCustomerPayload_PatchParametersIsNotProvided()
        {
            // Given
            var customer = new Customer
            {
                PatchParameters = null
            };

            ContactPatchHelper.GetContactEntityForCustomerPayload(customer, tracingService);
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Customer Patch parameter is null.")]
        public void GetContactEntityForCustomerPayload_PatchParametersCount()
        {
            // Given
            var customer = new Customer
            {
                PatchParameters = new List<string>()
            };

            ContactPatchHelper.GetContactEntityForCustomerPayload(customer, tracingService);
        }

        [TestMethod]
        public void GetContactEntityForCustomerPayload_SourceMarketIsNull()
        {
            // Given
            var customer = Testcustomer;
            customer.CustomerIdentifier.SourceMarket = null;

            // When        
            var result = ContactPatchHelper.GetContactEntityForCustomerPayload(customer, tracingService);

            // Then
            Assert.IsFalse(result.Contains(Attributes.Contact.FirstName));
        }
        [TestMethod]
        public void GetContactEntityForCustomerPayload_EmailIsNull()
        {
            // Given
            var customer = Testcustomer;
            customer.Email = null;
            // When        
            var result = ContactPatchHelper.GetContactEntityForCustomerPayload(customer, tracingService);
            // Then
            Assert.IsFalse(result.Contains(Attributes.Contact.FirstName));
        }
        [TestMethod]
        public void GetContactEntityForCustomerPayload_PhoneIsNull()
        {
            // Given
            var customer = Testcustomer;
            customer.Phone = null;
            // When        
            var result = ContactPatchHelper.GetContactEntityForCustomerPayload(customer, tracingService);
            // Then
            Assert.IsFalse(result.Contains(Attributes.Contact.FirstName));
        }
        [TestMethod]
        public void GetContactEntityForCustomerPayload_AddressIsNull()
        {
            // Given
            var customer = Testcustomer;
            customer.Address = null;
            // When        
            var result = ContactPatchHelper.GetContactEntityForCustomerPayload(customer, tracingService);
            // Then
            Assert.IsFalse(result.Contains(Attributes.Contact.FirstName));
        }
        

        [TestMethod]
        public void GetContactEntityForCustomerPayload_PopulateEmailField()
        {
            // Given
            var customer = Testcustomer;
            customer.Email[1] = null;
            // When        
            var result = ContactPatchHelper.GetContactEntityForCustomerPayload(customer, tracingService);
            // Then
            Assert.IsFalse(result.Contains(Attributes.Contact.EmailAddress2));
        }
        [TestMethod]
        public void GetContactEntityForCustomerPayload_PopulatePhoneField()
        {
            // Given
            var customer = Testcustomer;

            // When        
            var result = ContactPatchHelper.GetContactEntityForCustomerPayload(customer, tracingService);
            // Then
            Assert.IsFalse(result.Contains(Attributes.Contact.FirstName));
        }
        [TestMethod]
        public void GetContactEntityForCustomerPayload_PopulateAddressField()
        {
            // Given
            var customer = Testcustomer;
            // When        
            var result = ContactPatchHelper.GetContactEntityForCustomerPayload(customer, tracingService);
            // Then
            Assert.IsFalse(result.Contains(Attributes.Contact.FirstName));
        }
        [TestMethod]
        public void GetContactEntityForCustomerPayload_PopulateIdentityInformationIsNull()
        {
            // Given
            var customer = Testcustomer;
            customer.CustomerIdentity = null;
            // When        
            var result = ContactPatchHelper.GetContactEntityForCustomerPayload(customer, tracingService);
            // Then
            Assert.IsFalse(result.Contains(Attributes.Contact.FirstName));
        }
        [TestMethod]
        public void GetContactEntityForCustomerPayload_PopulatePermissionIsNull()
        {
            // Given
            var customer = Testcustomer;
            customer.Permissions = null;
            // When        
            var result = ContactPatchHelper.GetContactEntityForCustomerPayload(customer, tracingService);
            // Then
            Assert.IsFalse(result.Contains(Attributes.Contact.SendMarketingByPost));
        }
    }
}
