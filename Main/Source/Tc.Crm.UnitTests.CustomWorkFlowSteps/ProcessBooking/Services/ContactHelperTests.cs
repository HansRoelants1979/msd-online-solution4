using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.Xrm.Sdk;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;
using Tc.Crm.UnitTests.CustomWorkFlowSteps;

namespace Tc.Crm.CustomWorkflowSteps.ProcessBooking.Services.Tests
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
        public void GetContactEntityForBookingPayload_CustomerIsNull()
        {
           var customer = ContactHelper.GetContactEntityForBookingPayload(null, trace);
            Assert.IsNull(customer);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Tracing service is null.")]
        public void GetContactEntityForBookingPayload_TraceIsNull()
        {
            ContactHelper.GetContactEntityForBookingPayload(new Models.Customer(), null);
        }

        [TestMethod()]
        public void GetContactEntityForBookingPayload_CustomerIdentifierIsNull()
        {
            var contact = ContactHelper.GetContactEntityForBookingPayload(new Models.Customer(), trace);
            Assert.IsNotNull(contact);
            Assert.AreEqual("", contact[Attributes.Contact.SourceSystemId].ToString());
        }

        [TestMethod()]
        public void GetContactEntityForBookingPayload_CustomerIdIsNull()
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
            var contact = ContactHelper.GetContactEntityForBookingPayload(c, trace);
            Assert.IsNotNull(contact);
            Assert.AreEqual("", contact[Attributes.Contact.SourceSystemId].ToString());
        }

       
        [TestMethod()]
        public void GetContactEntityForBookingPayload_PopulateIdentity()
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
                    AcademicTitle = "MS",
                    Birthdate = "1982-10-08",
                    Gender = Gender.Female,
                    Language = "English",
                    MiddleName = "Tom",
                    Salutation = "Mr"
                }
            };
            var contact =ContactHelper.GetContactEntityForBookingPayload(c, trace);
            
            Assert.AreEqual(c.CustomerIdentity.AcademicTitle, contact[Attributes.Contact.AcademicTitle].ToString());
            Assert.AreEqual(c.CustomerIdentity.FirstName, contact[Attributes.Contact.FirstName].ToString());
            Assert.AreEqual(c.CustomerIdentity.LastName, contact[Attributes.Contact.LastName].ToString());
            Assert.AreEqual(c.CustomerIdentity.MiddleName, contact[Attributes.Contact.MiddleName].ToString());
            //checking for language which is not available in CRM
            Assert.AreEqual(950000006, ((OptionSetValue)(contact[Attributes.Contact.Language])).Value);
            Assert.AreEqual(950000000, ((OptionSetValue)(contact[Attributes.Contact.Salutation])).Value);
            Assert.AreEqual(950000001, ((OptionSetValue)(contact[Attributes.Contact.Gender])).Value);
       
        }

        [TestMethod()]
        public void GetContactEntityForBookingPayload_PopulatePhone()
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
                    AcademicTitle = "MS",
                    Birthdate = "1982-10-08",
                    Gender = Gender.Female,
                    Language = "EN",
                    MiddleName = "Tom",
                    Salutation = "Mr"
                },
                Phone = new Phone[]
               {
                   new Phone {Number="12345",PhoneType=PhoneType.Home },
                    new Phone {Number="123456",PhoneType=PhoneType.Mobile },
                     new Phone {Number="1234567",PhoneType=PhoneType.Mobile }
               }
            };
            var contact = ContactHelper.GetContactEntityForBookingPayload(c, trace);

            Assert.AreEqual(c.Phone[0].Number, contact[Attributes.Contact.Telephone1].ToString());
            Assert.AreEqual(c.Phone[1].Number, contact[Attributes.Contact.Telephone2].ToString());
            Assert.AreEqual(c.Phone[2].Number, contact[Attributes.Contact.Telephone3].ToString());
            Assert.AreEqual(950000001, ((OptionSetValue)(contact[Attributes.Contact.Telephone1Type])).Value);
            Assert.AreEqual(950000000, ((OptionSetValue)(contact[Attributes.Contact.Telephone2Type])).Value);
            Assert.AreEqual(950000000, ((OptionSetValue)(contact[Attributes.Contact.Telephone3Type])).Value);

            //checking for language = english
            Assert.AreEqual(950000000, ((OptionSetValue)(contact[Attributes.Contact.Language])).Value);

        }

        [TestMethod()]
        public void GetContactEntityForBookingPayload_PopulateEmail()
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
                    AcademicTitle = "MS",
                    Birthdate = "1982-10-08",
                    Gender = Gender.Female,
                    Language = "English",
                    MiddleName = "Tom",
                    Salutation = "Mr"
                },
                Email = new Email[]
                {
                    new Email {Address="one@tc.com",EmailType = EmailType.Primary },
                    new Email {Address="two@tc.com",EmailType = EmailType.Promo },
                    new Email {Address="three@tc.com",EmailType = EmailType.Promo }
                }
            };
            var contact = ContactHelper.GetContactEntityForBookingPayload(c, trace);

            Assert.AreEqual(c.Email[0].Address, contact[Attributes.Contact.EmailAddress1].ToString());
            Assert.AreEqual(c.Email[1].Address, contact[Attributes.Contact.EmailAddress2].ToString());
            Assert.AreEqual(c.Email[2].Address, contact[Attributes.Contact.EmailAddress3].ToString());
            Assert.AreEqual(950000000, ((OptionSetValue)(contact[Attributes.Contact.EmailAddress1Type])).Value);
            Assert.AreEqual(950000001, ((OptionSetValue)(contact[Attributes.Contact.EmailAddress2Type])).Value);
            Assert.AreEqual(950000001, ((OptionSetValue)(contact[Attributes.Contact.EmailAddress3Type])).Value);

        }

        [TestMethod()]
        public void GetContactEntityForBookingPayload_PopulateAddress()
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
                    AcademicTitle = "MS",
                    Birthdate = "1982-10-08",
                    Gender = Gender.Female,
                    Language = "English",
                    MiddleName = "Tom",
                    Salutation = "Mr"
                },
                Address = new Address[]
                {
                    new Address
                    {
                        AdditionalAddressInfo = "Tes",
                        AddressType = AddressType.Main,
                        Country = Guid.NewGuid().ToString(),
                        County = "Warrington",
                        FlatNumberUnit="A",
                        HouseNumberBuilding="21",
                        PostalCode = "WA113",
                        Street="Handy",
                        Town="Man"
                    },
                    new Address
                    {
                        AdditionalAddressInfo = "Tes",
                        AddressType = AddressType.Main,
                        Country = Guid.NewGuid().ToString(),
                        County = "Warrington",
                        FlatNumberUnit="A",
                        HouseNumberBuilding="211",
                        PostalCode = "WA1131",
                        Street="Handy1",
                        Town="Man1"
                    }
                }
            };
            var contact = ContactHelper.GetContactEntityForBookingPayload(c, trace);

            Assert.AreEqual(c.Address[0].AdditionalAddressInfo, contact[Attributes.Contact.Address1AdditionalInformation].ToString());
            Assert.AreEqual(c.Address[0].FlatNumberUnit, contact[Attributes.Contact.Address1FlatOrUnitNumber].ToString());
            Assert.AreEqual(c.Address[0].HouseNumberBuilding, contact[Attributes.Contact.Address1HouseNumberOrBuilding].ToString());
            Assert.AreEqual(c.Address[0].Town, contact[Attributes.Contact.Address1Town].ToString());
            Assert.AreEqual(c.Address[0].PostalCode, contact[Attributes.Contact.Address1PostalCode].ToString());
            Assert.AreEqual(c.Address[0].County, contact[Attributes.Contact.Address1County].ToString());
            Assert.AreEqual(c.Address[0].Country, ((EntityReference)(contact[Attributes.Contact.Address1CountryId])).Id.ToString());

        }

        [TestMethod()]
        public void GetContactEntityForBookingPayload_GeneralAndAdditional()
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
                    AcademicTitle = "MS",
                    Birthdate = "1982-10-08",
                    Gender = Gender.Female,
                    Language = "English",
                    MiddleName = "Tom",
                    Salutation = "Mr"
                },
                CustomerGeneral = new CustomerGeneral
                {
                    CustomerStatus = CustomerStatus.Blacklisted
                },
                Additional = new Additional
                {
                    DateOfDeath = "2016-10-01",
                    Segment = "1"
                }
            };
            var contact = ContactHelper.GetContactEntityForBookingPayload(c, trace);

            Assert.AreEqual(950000000, ((OptionSetValue)(contact[Attributes.Contact.Segment])).Value);
            //Assert.AreEqual(950000001, ((OptionSetValue)(contact[Attributes.Contact.StatusCode])).Value);
        }
    }
}