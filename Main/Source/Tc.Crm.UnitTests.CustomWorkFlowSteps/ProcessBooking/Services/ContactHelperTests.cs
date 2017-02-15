using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        [ExpectedException(typeof(InvalidPluginExecutionException), "Customer payload is null.")]
        public void GetContactEntityForBookingPayload_CustomerIsNull()
        {
            ContactHelper.GetContactEntityForBookingPayload(null, trace);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Tracing service is null.")]
        public void GetContactEntityForBookingPayload_TraceIsNull()
        {
            ContactHelper.GetContactEntityForBookingPayload(new Models.Customer(), null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Customer Id could not be retrieved from payload.")]
        public void GetContactEntityForBookingPayload_CustomerIdentifierIsNull()
        {
            ContactHelper.GetContactEntityForBookingPayload(new Models.Customer(), trace);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Customer Id could not be retrieved from payload.")]
        public void GetContactEntityForBookingPayload_CustomerIdIsNull()
        {
            Customer c = new Customer
            {
                CustomerIdentifier = new CustomerIdentifier
                {
                    BusinessArea = "Hotel",
                    SourceMarket = "AX",
                    SourceSystem = "On Tour"
                }
            };
            ContactHelper.GetContactEntityForBookingPayload(c, trace);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Last name could not be retrieved from payload.")]
        public void GetContactEntityForBookingPayload_LastNameIsNull()
        {
            Customer c = new Customer
            {
                CustomerIdentifier = new CustomerIdentifier
                {
                    BusinessArea = "Hotel",
                    CustomerId = "CONT001",
                    SourceMarket = "AX",
                    SourceSystem = "On Tour"
                },
                CustomerIdentity = new CustomerIdentity
                {
                    FirstName = "Joe"
                }
            };
            ContactHelper.GetContactEntityForBookingPayload(c, trace);
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
                    SourceMarket = "AX",
                    SourceSystem = "On Tour"
                },
                CustomerIdentity = new CustomerIdentity
                {
                    FirstName = "Joe",
                    LastName = "Blog",
                    AcademicTitle = "MS",
                    Birthdate = "1982-10-08",
                    Gender = Gender.F,
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
            Assert.AreEqual(950000000, ((OptionSetValue)(contact[Attributes.Contact.Language])).Value);
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
                    SourceMarket = "AX",
                    SourceSystem = "On Tour"
                },
                CustomerIdentity = new CustomerIdentity
                {
                    FirstName = "Joe",
                    LastName = "Blog",
                    AcademicTitle = "MS",
                    Birthdate = "1982-10-08",
                    Gender = Gender.F,
                    Language = "English",
                    MiddleName = "Tom",
                    Salutation = "Mr"
                },
                Phone = new Phone[]
               {
                   new Phone {Number="12345",PhoneType=PhoneType.H },
                    new Phone {Number="123456",PhoneType=PhoneType.M },
                     new Phone {Number="1234567",PhoneType=PhoneType.M }
               }
            };
            var contact = ContactHelper.GetContactEntityForBookingPayload(c, trace);

            Assert.AreEqual(c.Phone[0].Number, contact[Attributes.Contact.Telephone1].ToString());
            Assert.AreEqual(c.Phone[1].Number, contact[Attributes.Contact.Telephone2].ToString());
            Assert.AreEqual(c.Phone[2].Number, contact[Attributes.Contact.Telephone3].ToString());
            Assert.AreEqual(950000001, ((OptionSetValue)(contact[Attributes.Contact.Telephone1Type])).Value);
            Assert.AreEqual(950000000, ((OptionSetValue)(contact[Attributes.Contact.Telephone2Type])).Value);
            Assert.AreEqual(950000000, ((OptionSetValue)(contact[Attributes.Contact.Telephone3Type])).Value);

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
                    SourceMarket = "AX",
                    SourceSystem = "On Tour"
                },
                CustomerIdentity = new CustomerIdentity
                {
                    FirstName = "Joe",
                    LastName = "Blog",
                    AcademicTitle = "MS",
                    Birthdate = "1982-10-08",
                    Gender = Gender.F,
                    Language = "English",
                    MiddleName = "Tom",
                    Salutation = "Mr"
                },
                Email = new Email[]
                {
                    new Email {Address="one@tc.com",EmailType = EmailType.Pri },
                    new Email {Address="two@tc.com",EmailType = EmailType.Pro },
                    new Email {Address="three@tc.com",EmailType = EmailType.Pro }
                }
            };
            var contact = ContactHelper.GetContactEntityForBookingPayload(c, trace);

            Assert.AreEqual(c.Email[0].Address, contact[Attributes.Contact.EMailAddress1].ToString());
            Assert.AreEqual(c.Email[1].Address, contact[Attributes.Contact.EMailAddress2].ToString());
            Assert.AreEqual(c.Email[2].Address, contact[Attributes.Contact.EMailAddress3].ToString());
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
                    SourceMarket = "AX",
                    SourceSystem = "On Tour"
                },
                CustomerIdentity = new CustomerIdentity
                {
                    FirstName = "Joe",
                    LastName = "Blog",
                    AcademicTitle = "MS",
                    Birthdate = "1982-10-08",
                    Gender = Gender.F,
                    Language = "English",
                    MiddleName = "Tom",
                    Salutation = "Mr"
                },
                Address = new Address[]
                {
                    new Address
                    {
                        AdditionalAddressInfo = "Tes",
                        AddressType = AddressType.M,
                        Box = "21",
                        Country = "England",
                        County = "Ferro",
                        FlatNumberUnit="A",
                        HouseNumberBuilding="21",
                        Number="21",
                        PostalCode = "WA113",
                        Street="Handy",
                        Town="Man"
                    },
                    new Address
                    {
                        AdditionalAddressInfo = "Tes",
                        AddressType = AddressType.M,
                        Box = "211",
                        Country = "Engl1and1",
                        County = "Ferro",
                        FlatNumberUnit="A",
                        HouseNumberBuilding="211",
                        Number="211",
                        PostalCode = "WA1131",
                        Street="Handy1",
                        Town="Man1"
                    }
                }
            };
            var contact = ContactHelper.GetContactEntityForBookingPayload(c, trace);

            Assert.AreEqual(c.Address[0].AdditionalAddressInfo, contact[Attributes.Contact.Address1_AdditionalInformation].ToString());
            Assert.AreEqual(c.Address[0].FlatNumberUnit, contact[Attributes.Contact.Address1_FlatOrUnitNumber].ToString());
            Assert.AreEqual(c.Address[0].HouseNumberBuilding, contact[Attributes.Contact.Address1_HouseNumberOrBuilding].ToString());
            Assert.AreEqual(c.Address[0].Town, contact[Attributes.Contact.Address1_Town].ToString());
            Assert.AreEqual(c.Address[0].PostalCode, contact[Attributes.Contact.Address1_PostalCode].ToString());
            Assert.AreEqual(c.Address[0].County, contact[Attributes.Contact.Address1_County].ToString());
            Assert.AreEqual(c.Address[0].Country, ((EntityReference)(contact[Attributes.Contact.Address1_CountryId])).KeyAttributes[Attributes.Country.ISO2Code].ToString());

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
                    SourceMarket = "AX",
                    SourceSystem = "On Tour"
                },
                CustomerIdentity = new CustomerIdentity
                {
                    FirstName = "Joe",
                    LastName = "Blog",
                    AcademicTitle = "MS",
                    Birthdate = "1982-10-08",
                    Gender = Gender.F,
                    Language = "English",
                    MiddleName = "Tom",
                    Salutation = "Mr"
                },
                CustomerGeneral = new CustomerGeneral
                {
                    CustomerStatus = CustomerStatus.B
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