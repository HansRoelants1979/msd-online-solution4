using System;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;

namespace Tc.Crm.CustomWorkflowSteps.ProcessCustomer.Services.Tests
{
    [TestClass]
    public class AccountHelperTests
    {
        private ITracingService tracingService;

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
			AccountHelper.GetAccountEntityForCustomerPayload(null, tracingService);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Tracing service is null.")]
        public void GetAccountEntityForCustomerPayload_TraceIsNull()
        {
            AccountHelper.GetAccountEntityForCustomerPayload(new Customer(), null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Customer Idenifier is null.")]
        public void GetAccountEntityForCustomerPayload_CustomerIdentifierIsNull()
        {
            AccountHelper.GetAccountEntityForCustomerPayload(new Customer(), tracingService);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Customer Identifier could not be retrieved from payload.")]
		public void GetAccountEntityForCustomerPayload_CustomerIdIsNull()
        {           
            AccountHelper.GetAccountEntityForCustomerPayload(new Customer(), tracingService);
        }

        [TestMethod]
        public void GetAccountEntityForCustomerPayload_CompanyNameIsNull()
        {
			// Given
			var customer = new Customer
			{
				CustomerIdentifier = new CustomerIdentifier { CustomerId = "Test customer" },
				Company = new Company()
			};

	        // When        
	        var result = AccountHelper.GetAccountEntityForCustomerPayload(customer, tracingService);

	        // Then
	        Assert.IsFalse(result.Contains(Attributes.Account.Name));
		}

        [TestMethod]
        public void GetAccountEntityForCustomerPayload_CompanyIsNotProvided()
        {    
			// Given
	        var customer = new Customer
		    {
			    CustomerIdentifier = new CustomerIdentifier { CustomerId = "Test customer" }
		    };  

			// When        
            var result = AccountHelper.GetAccountEntityForCustomerPayload(customer, tracingService);

			// Then
			Assert.IsFalse(result.Contains(Attributes.Account.Name));
        }

        [TestMethod]
        public void GetAccountEntityForCustomerPayload_PopulatePhone()
        {
			// Given
	        var customer = new Customer
		    {
			    CustomerIdentifier = new CustomerIdentifier { CustomerId = "Test customer" },
				Phone = new[]
				{
					new Phone
					{
						Number = "111-11-11"
					},
					new Phone
					{
						Number = "222-22-22"
					},
					new Phone
					{
						Number = "333-33-33"
					}
				}
			};

	        // When        
	        var result = AccountHelper.GetAccountEntityForCustomerPayload(customer, tracingService);

			// Then
			Assert.AreEqual(customer.Phone[0].Number, result[Attributes.Account.Telephone1].ToString());
            Assert.AreEqual(customer.Phone[1].Number, result[Attributes.Account.Telephone2].ToString());
            Assert.AreEqual(customer.Phone[2].Number, result[Attributes.Account.Telephone3].ToString());
        }

        [TestMethod]
        public void GetAccountEntityForCustomerPayload_PopulateEmail()
        {
			// Given
	        var customer = new Customer
		    {
			    CustomerIdentifier = new CustomerIdentifier { CustomerId = "Test customer" },
			    Email = new[]
				{
					new Email
					{
						Address = "test1@test.com"
					},
					new Email
					{
						Address = "test2@test.com"
					},
					new Email
					{
						Address = "test3@test.com"
					}
				}
		    };

	        // When        
	        var result = AccountHelper.GetAccountEntityForCustomerPayload(customer, tracingService);

	        // Then
			Assert.AreEqual(customer.Email[0].Address, result[Attributes.Account.EmailAddress1].ToString());
            Assert.AreEqual(customer.Email[1].Address, result[Attributes.Account.EmailAddress2].ToString());
            Assert.AreEqual(customer.Email[2].Address, result[Attributes.Account.EmailAddress3].ToString());
        }

        [TestMethod]
        public void GetAccountEntityForCustomerPayload_PopulateAddress()
        {
			// Given
	        var countryId = Guid.NewGuid().ToString();
	        var customer = new Customer
		    {
			    CustomerIdentifier = new CustomerIdentifier { CustomerId = "Test customer" },
			    Address = new[]
				{
					new Address
					{
						AdditionalAddressInfo = "AdditionalAddressInfo",
						FlatNumberUnit = "FlatNumberUnit",
						HouseNumberBuilding = "HouseNumberBuilding",
						Town = "London",
						Country = countryId,
						County = "County",
						PostalCode = "PostalCode",
						Street = "Street"
					}
				}
		    };

			// When   
			var result = AccountHelper.GetAccountEntityForCustomerPayload(customer, tracingService);

			// Then
	        Assert.AreEqual(customer.Address[0].AdditionalAddressInfo, result[Attributes.Account.Address1AdditionalInformation].ToString());
	        Assert.AreEqual(customer.Address[0].FlatNumberUnit, result[Attributes.Account.Address1FlatOrUnitNumber].ToString());
	        Assert.AreEqual(customer.Address[0].HouseNumberBuilding, result[Attributes.Account.Address1HouseNumberOrBuilding].ToString());
	        Assert.AreEqual(customer.Address[0].Town, result[Attributes.Account.Address1Town].ToString());
	        Assert.AreEqual(customer.Address[0].Country, ((EntityReference)result[Attributes.Account.Address1CountryId]).Id.ToString());
			Assert.AreEqual(customer.Address[0].County, result[Attributes.Account.Address1County].ToString());
	        Assert.AreEqual(customer.Address[0].PostalCode, result[Attributes.Account.Address1PostalCode].ToString());
			Assert.AreEqual(customer.Address[0].Street, result[Attributes.Account.Address1Street].ToString());
		}
    }
}
