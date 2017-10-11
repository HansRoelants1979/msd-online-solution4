using System;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;

namespace Tc.Crm.CustomWorkflowSteps.ProcessCustomer.Services.Tests
{
    [TestClass]
    public class ContactHelperTests
    {
	    private ITracingService tracingService;

		[TestInitialize]
        public void Setup()
        {
	        tracingService = A.Fake<ITracingService>();

	        A.CallTo(() => tracingService.Trace(A<string>._)).DoesNothing();
		}

        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Customer is null.")]
        public void GetContactEntityForCustomerPayload_CustomerIsNull()
        {       
			// When     
            ContactHelper.GetContactEntityForCustomerPayload(null, tracingService);            
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Tracing service is null.")]
        public void GetContactEntityForCustomerPayload_TraceIsNull()
        {         
			// When   
            ContactHelper.GetContactEntityForCustomerPayload(new Customer(), null);
        }

		[TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Customer Identifier is null.")]
        public void GetContactEntityForCustomerPayload_CustomerIdentifierIsNull()
        {
			// When 
			ContactHelper.GetContactEntityForCustomerPayload(new Customer(), tracingService);                      
        }        

        [TestMethod]
        public void GetContactEntityForCustomerPayload_PopulateIdentity()
        {
			// Given
	        var customer = new Customer
		    {
				CustomerIdentifier = new CustomerIdentifier { CustomerId = "Test customer" },
			    CustomerIdentity = new CustomerIdentity
				{
					AcademicTitle = "PhD",
					Birthdate = "1982-10-08",
					FirstName = "John",
					Gender = Gender.Male,
					Language = "Language",
					LastName = "Doe",
					Salutation = "Mr"
				}
		    };

	        // When 
			var contact = ContactHelper.GetContactEntityForCustomerPayload(customer, tracingService);

			// Then
			Assert.AreEqual(customer.CustomerIdentity.FirstName, contact[Attributes.Contact.FirstName].ToString());
            Assert.AreEqual(customer.CustomerIdentity.LastName, contact[Attributes.Contact.LastName].ToString());            
            Assert.AreEqual(950000006, ((OptionSetValue)contact[Attributes.Contact.Language]).Value);
            Assert.AreEqual(950000000, ((OptionSetValue)contact[Attributes.Contact.Salutation]).Value);           
        }

        [TestMethod]
        public void GetContactEntityForCustomerPayload_PopulateAddress()
		{   // Given
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
						Town = "Town",
						Country = countryId,
						County = "County",
						PostalCode = "PostalCode",
						Street = "Street"
					},
					new Address
					{
						AdditionalAddressInfo = "AdditionalAddressInfo",
						FlatNumberUnit = "FlatNumberUnit",
						HouseNumberBuilding = "HouseNumberBuilding",
						Town = "Town",
						Country = countryId,
						County = "County",
						PostalCode = "PostalCode",
						Street = "Street"
					}
				}
			};

			// When                       
			var contact = ContactHelper.GetContactEntityForCustomerPayload(customer, tracingService);

			// Then
			Assert.AreEqual(customer.Address[0].AdditionalAddressInfo, contact[Attributes.Account.Address1AdditionalInformation].ToString());
			Assert.AreEqual(customer.Address[0].FlatNumberUnit, contact[Attributes.Account.Address1FlatOrUnitNumber].ToString());
			Assert.AreEqual(customer.Address[0].HouseNumberBuilding, contact[Attributes.Account.Address1HouseNumberOrBuilding].ToString());
			Assert.AreEqual(customer.Address[0].Town, contact[Attributes.Account.Address1Town].ToString());
			Assert.AreEqual(customer.Address[0].Country, ((EntityReference)contact[Attributes.Account.Address1CountryId]).Id.ToString());
			Assert.AreEqual(customer.Address[0].County, contact[Attributes.Account.Address1County].ToString());
			Assert.AreEqual(customer.Address[0].PostalCode, contact[Attributes.Account.Address1PostalCode].ToString());
			Assert.AreEqual(customer.Address[0].Street, contact[Attributes.Account.Address1Street].ToString());
			Assert.AreEqual(customer.Address[1].AdditionalAddressInfo, contact[Attributes.Account.Address1AdditionalInformation].ToString());
			Assert.AreEqual(customer.Address[1].FlatNumberUnit, contact[Attributes.Account.Address1FlatOrUnitNumber].ToString());
			Assert.AreEqual(customer.Address[1].HouseNumberBuilding, contact[Attributes.Account.Address1HouseNumberOrBuilding].ToString());
			Assert.AreEqual(customer.Address[1].Town, contact[Attributes.Account.Address1Town].ToString());
			Assert.AreEqual(customer.Address[1].Country, ((EntityReference)contact[Attributes.Account.Address1CountryId]).Id.ToString());
			Assert.AreEqual(customer.Address[1].County, contact[Attributes.Account.Address1County].ToString());
			Assert.AreEqual(customer.Address[1].PostalCode, contact[Attributes.Account.Address1PostalCode].ToString());
			Assert.AreEqual(customer.Address[1].Street, contact[Attributes.Account.Address1Street].ToString());
		}

		[TestMethod]
		public void GetContactEntityForCustomerPayload_PopulatePermission()
		{
			// Given
			var customer = new Customer
			{
				CustomerIdentifier = new CustomerIdentifier { CustomerId = "Test customer" },
				Permissions = new Permissions
				{
					AllowMarketing = true,
					DoNotAllowEmail = true,
					DoNotAllowMail = true,
					DoNotAllowPhoneCalls = true,
					DoNotAllowSms = true
				}
			};

			// When
			var contact = ContactHelper.GetContactEntityForCustomerPayload(customer, tracingService);

			// Then
			Assert.AreEqual(new OptionSetValue(950000001), contact[Attributes.Contact.MarketingByPhone]);
			Assert.AreEqual(new OptionSetValue(950000001), contact[Attributes.Contact.SendMarketingByEmail]);
			Assert.AreEqual(new OptionSetValue(950000001), contact[Attributes.Contact.SendMarketingByPost]);
			Assert.AreEqual(new OptionSetValue(950000000), contact[Attributes.Contact.ThomasCookMarketingConsent]);
			Assert.AreEqual(new OptionSetValue(950000001), contact[Attributes.Contact.SendMarketingBySms]);
		}

		[TestMethod]
        public void GetContactEntityForCustomerPayload_PopulatePhone()
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
			var contact = ContactHelper.GetContactEntityForCustomerPayload(customer, tracingService);

			// Then
            Assert.AreEqual(customer.Phone[0].Number, contact[Attributes.Contact.Telephone1].ToString());
            Assert.AreEqual(customer.Phone[1].Number, contact[Attributes.Contact.Telephone2].ToString());
            Assert.AreEqual(customer.Phone[2].Number, contact[Attributes.Contact.Telephone3].ToString());
        }

        [TestMethod]
        public void GetContactEntityForCustomerPayload_PopulateEmail()
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
			var contact = ContactHelper.GetContactEntityForCustomerPayload(customer, tracingService);

			// Then
            Assert.AreEqual(customer.Email[0].Address, contact[Attributes.Contact.EmailAddress1].ToString());
            Assert.AreEqual(customer.Email[1].Address, contact[Attributes.Contact.EmailAddress2].ToString());
            Assert.AreEqual(customer.Email[2].Address, contact[Attributes.Contact.EmailAddress3].ToString());
        }

    }
}
