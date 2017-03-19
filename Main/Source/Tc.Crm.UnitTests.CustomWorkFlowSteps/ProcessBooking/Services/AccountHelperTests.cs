using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.UnitTests.CustomWorkFlowSteps;
using Microsoft.Xrm.Sdk;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;

namespace Tc.Crm.CustomWorkflowSteps.ProcessBooking.Services.Tests
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
        public void GetAccountEntityForBookingPayload_CustomerIsNull()
        {
            AccountHelper.GetAccountEntityForBookingPayload(null, trace);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Tracing service is null.")]
        public void GetAccountEntityForBookingPayload_TraceIsNull()
        {
            AccountHelper.GetAccountEntityForBookingPayload(new Models.Customer(), null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Customer Idenifier is null.")]
        public void GetAccountEntityForBookingPayload_CustomerIdentifierIsNull()
        {
            AccountHelper.GetAccountEntityForBookingPayload(new Models.Customer(), trace);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Customer Id could not be retrieved from payload.")]
        public void GetAccountEntityForBookingPayload_CustomerIdIsNull()
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
            AccountHelper.GetAccountEntityForBookingPayload(c, trace);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Account name could not be retrieved from payload.")]
        public void GetAccountEntityForBookingPayload_CompanyNameIsNull()
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
                Company = new Company
                {

                }
            };
            AccountHelper.GetAccountEntityForBookingPayload(c, trace);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Account name could not be retrieved from payload.")]
        public void GetAccountEntityForBookingPayload_CompanyIsNull()
        {
            Customer c = new Customer
            {
                CustomerIdentifier = new CustomerIdentifier
                {
                    BusinessArea = "Hotel",
                    CustomerId = "CONT001",
                    SourceMarket = "AX",
                    SourceSystem = "On Tour"
                }
            };
            AccountHelper.GetAccountEntityForBookingPayload(c, trace);
        }

        #region Populate Address
        [TestMethod()]
        public void GetAccountEntityForBookingPayload_PopulateAddress()
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
                Company = new Company { CompanyName = "Test" },
                Address = new Address[]
                {
                    new Address
                    {
                        AdditionalAddressInfo = "Tes",
                        AddressType = AddressType.Main,
                        Country = "England",
                        County = "Ferro",
                        FlatNumberUnit="A",
                        HouseNumberBuilding="21",
                        PostalCode = "WA113",
                        Street="Handy",
                        Town="Man"
                    }
                }
            };
            var account = AccountHelper.GetAccountEntityForBookingPayload(c, trace);


            Assert.AreEqual(c.Address[0].AdditionalAddressInfo, account[Attributes.Account.Address1_AdditionalInformation].ToString());
            Assert.AreEqual(c.Address[0].FlatNumberUnit, account[Attributes.Account.Address1_FlatOrUnitNumber].ToString());
            Assert.AreEqual(c.Address[0].HouseNumberBuilding, account[Attributes.Account.Address1_HouseNumberOrBuilding].ToString());
            Assert.AreEqual(c.Address[0].Town, account[Attributes.Account.Address1_Town].ToString());
            Assert.AreEqual(c.Address[0].PostalCode, account[Attributes.Account.Address1_PostalCode].ToString());
            Assert.AreEqual(c.Address[0].County, account[Attributes.Account.Address1_County].ToString());
            Assert.AreEqual(c.Address[0].Country, ((EntityReference)(account[Attributes.Account.Address1_CountryId])).KeyAttributes[Attributes.Country.ISO2Code].ToString());
        }

        [TestMethod()]
        public void GetAccountEntityForBookingPayload_PopulateAddress_Muliple()
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
                Company = new Company { CompanyName = "Test" },
                Address = new Address[]
                {
                    new Address
                    {
                        AdditionalAddressInfo = "Tes",
                        AddressType = AddressType.Main,
                        Country = "England",
                        County = "Ferro",
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
                        Country = "Engl1and1",
                        County = "Ferro",
                        FlatNumberUnit="A",
                        HouseNumberBuilding="211",
                        PostalCode = "WA1131",
                        Street="Handy1",
                        Town="Man1"
                    }
                }
            };
            var account = AccountHelper.GetAccountEntityForBookingPayload(c, trace);


            Assert.AreEqual(c.Address[0].AdditionalAddressInfo, account[Attributes.Account.Address1_AdditionalInformation].ToString());
            Assert.AreEqual(c.Address[0].FlatNumberUnit, account[Attributes.Account.Address1_FlatOrUnitNumber].ToString());
            Assert.AreEqual(c.Address[0].HouseNumberBuilding, account[Attributes.Account.Address1_HouseNumberOrBuilding].ToString());
            Assert.AreEqual(c.Address[0].Town, account[Attributes.Account.Address1_Town].ToString());
            Assert.AreEqual(c.Address[0].PostalCode, account[Attributes.Account.Address1_PostalCode].ToString());
            Assert.AreEqual(c.Address[0].County, account[Attributes.Account.Address1_County].ToString());
            Assert.AreEqual(c.Address[0].Country, ((EntityReference)(account[Attributes.Account.Address1_CountryId])).KeyAttributes[Attributes.Country.ISO2Code].ToString());
        }

        [TestMethod()]
        public void GetAccountEntityForBookingPayload_PopulateAddress_AddressEmpty()
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
                Company = new Company { CompanyName = "Test" },
                Address = new Address[]
                {

                }
            };
            var account = AccountHelper.GetAccountEntityForBookingPayload(c, trace);


            Assert.AreEqual(true, account.Contains(Attributes.Account.Address1_AdditionalInformation));
            Assert.AreEqual(true, account.Contains(Attributes.Account.Address1_FlatOrUnitNumber));
            Assert.AreEqual(true, account.Contains(Attributes.Account.Address1_HouseNumberOrBuilding));
            Assert.AreEqual(true, account.Contains(Attributes.Account.Address1_Town));
            Assert.AreEqual(true, account.Contains(Attributes.Account.Address1_PostalCode));
            Assert.AreEqual(true, account.Contains(Attributes.Account.Address1_County));
            Assert.AreEqual(true, account.Contains(Attributes.Account.Address1_CountryId));

            Assert.AreEqual(string.Empty,account[Attributes.Account.Address1_AdditionalInformation]);
            Assert.AreEqual(string.Empty, account[Attributes.Account.Address1_FlatOrUnitNumber]);
            Assert.AreEqual(string.Empty, account[Attributes.Account.Address1_HouseNumberOrBuilding]);
            Assert.AreEqual(string.Empty, account[Attributes.Account.Address1_Town]);
            Assert.AreEqual(string.Empty, account[Attributes.Account.Address1_PostalCode]);
            Assert.AreEqual(string.Empty, account[Attributes.Account.Address1_County]);
            Assert.IsNull(account[Attributes.Account.Address1_CountryId]);
        }

        [TestMethod()]
        public void GetAccountEntityForBookingPayload_PopulateAddress_AddressZeroNull()
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
                Company = new Company { CompanyName = "Test" },
                Address = new Address[]
                {
                    null
                }
            };
            var account = AccountHelper.GetAccountEntityForBookingPayload(c, trace);


            Assert.AreEqual(false, account.Contains(Attributes.Account.Address1_AdditionalInformation));
            Assert.AreEqual(false, account.Contains(Attributes.Account.Address1_FlatOrUnitNumber));
            Assert.AreEqual(false, account.Contains(Attributes.Account.Address1_HouseNumberOrBuilding));
            Assert.AreEqual(false, account.Contains(Attributes.Account.Address1_Town));
            Assert.AreEqual(false, account.Contains(Attributes.Account.Address1_PostalCode));
            Assert.AreEqual(false, account.Contains(Attributes.Account.Address1_County));
            Assert.AreEqual(false, account.Contains(Attributes.Account.Address1_CountryId));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Account entiy is null.")]
        public void PopulateAddress_CustomerIsNull()
        {
            AccountHelper.PopulateAddress(null, new Address[] { new Address() }, trace);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidPluginExecutionException), "Tracing service is null.")]
        public void PopulateAddress_TraceIsNull()
        {
            AccountHelper.PopulateAddress(new Entity("account"), new Address[] { new Address() }, null);
        }

        [TestMethod()]
        public void PopulateAddress_AddressesIsNull()
        {
            Entity account = new Entity("account");
            AccountHelper.PopulateAddress(account, null, trace);
            Assert.AreEqual(true, account.Contains(Attributes.Account.Address1_AdditionalInformation));
            Assert.AreEqual(true, account.Contains(Attributes.Account.Address1_FlatOrUnitNumber));
            Assert.AreEqual(true, account.Contains(Attributes.Account.Address1_HouseNumberOrBuilding));
            Assert.AreEqual(true, account.Contains(Attributes.Account.Address1_Town));
            Assert.AreEqual(true, account.Contains(Attributes.Account.Address1_PostalCode));
            Assert.AreEqual(true, account.Contains(Attributes.Account.Address1_County));
            Assert.AreEqual(true, account.Contains(Attributes.Account.Address1_CountryId));

            Assert.AreEqual(string.Empty, account[Attributes.Account.Address1_AdditionalInformation]);
            Assert.AreEqual(string.Empty, account[Attributes.Account.Address1_FlatOrUnitNumber]);
            Assert.AreEqual(string.Empty, account[Attributes.Account.Address1_HouseNumberOrBuilding]);
            Assert.AreEqual(string.Empty, account[Attributes.Account.Address1_Town]);
            Assert.AreEqual(string.Empty, account[Attributes.Account.Address1_PostalCode]);
            Assert.AreEqual(string.Empty, account[Attributes.Account.Address1_County]);
            Assert.IsNull(account[Attributes.Account.Address1_CountryId]);
        }
        #endregion Populate Address

        #region Populate Email
        [TestMethod()]
        public void GetAccountEntityForBookingPayload_PopulateEmail()
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
                Company = new Company { CompanyName = "Test" },
                Email = new Email[]
                {
                    new Email {Address="one@tc.com",EmailType = EmailType.Pri },
                    new Email {Address="two@tc.com",EmailType = EmailType.Pro },
                    new Email {Address="three@tc.com",EmailType = EmailType.Pro }
                }
            };
            var account = AccountHelper.GetAccountEntityForBookingPayload(c, trace);


            Assert.AreEqual(c.Email[0].Address, account[Attributes.Account.EMailAddress1].ToString());
            Assert.AreEqual(c.Email[1].Address, account[Attributes.Account.EMailAddress2].ToString());
            Assert.AreEqual(c.Email[2].Address, account[Attributes.Account.EMailAddress3].ToString());
            Assert.AreEqual(950000000, ((OptionSetValue)(account[Attributes.Account.EmailAddress1_Type])).Value);
            Assert.AreEqual(950000001, ((OptionSetValue)(account[Attributes.Account.EmailAddress2_Type])).Value);
            Assert.AreEqual(950000001, ((OptionSetValue)(account[Attributes.Account.EmailAddress3_Type])).Value);

        }
        #endregion Populate Email

        #region Populate Phone
        [TestMethod]
        public void GetAccountEntityForBookingPayload_PopulatePhone()
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
                Company = new Company { CompanyName = "Test" },
                Phone = new Phone[]
               {
                   new Phone {Number="12345",PhoneType=PhoneType.H },
                    new Phone {Number="123456",PhoneType=PhoneType.M },
                     new Phone {Number="1234567",PhoneType=PhoneType.M }
               }
            };
            var account = AccountHelper.GetAccountEntityForBookingPayload(c, trace);


            Assert.AreEqual(c.Phone[0].Number, account[Attributes.Account.Telephone1].ToString());
            Assert.AreEqual(c.Phone[1].Number, account[Attributes.Account.Telephone2].ToString());
            Assert.AreEqual(c.Phone[2].Number, account[Attributes.Account.Telephone3].ToString());
            Assert.AreEqual(950000001, ((OptionSetValue)(account[Attributes.Account.Telephone1_Type])).Value);
            Assert.AreEqual(950000000, ((OptionSetValue)(account[Attributes.Account.Telephone2_Type])).Value);
            Assert.AreEqual(950000000, ((OptionSetValue)(account[Attributes.Account.Telephone3_Type])).Value);

        }
        #endregion
    }
}