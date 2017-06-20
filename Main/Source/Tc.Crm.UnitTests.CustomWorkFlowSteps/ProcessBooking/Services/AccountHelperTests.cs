using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tc.Crm.UnitTests.CustomWorkFlowSteps;
using Microsoft.Xrm.Sdk;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;
using System;

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
        public void GetAccountEntityForBookingPayload_CustomerIdIsNull()
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
            AccountHelper.GetAccountEntityForBookingPayload(c, trace);
        }

        [TestMethod()]
        public void GetAccountEntityForBookingPayload_CompanyNameIsNull()
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
            AccountHelper.GetAccountEntityForBookingPayload(c, trace);
        }

        [TestMethod()]
        public void GetAccountEntityForBookingPayload_CompanyIsNull()
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
                    SourceMarket = Guid.NewGuid().ToString(),
                    SourceSystem = "On Tour"
                },
                Company = new Company { CompanyName = "Test" },
                Address = new Address[]
                {
                    new Address
                    {
                        AdditionalAddressInfo = "Tes",
                        AddressType = AddressType.Main,
                        Country = Guid.NewGuid().ToString(),
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


            Assert.AreEqual(c.Address[0].AdditionalAddressInfo, account[Attributes.Account.Address1AdditionalInformation].ToString());
            Assert.AreEqual(c.Address[0].FlatNumberUnit, account[Attributes.Account.Address1FlatOrUnitNumber].ToString());
            Assert.AreEqual(c.Address[0].HouseNumberBuilding, account[Attributes.Account.Address1HouseNumberOrBuilding].ToString());
            Assert.AreEqual(c.Address[0].Town, account[Attributes.Account.Address1Town].ToString());
            Assert.AreEqual(c.Address[0].PostalCode, account[Attributes.Account.Address1PostalCode].ToString());
            Assert.AreEqual(c.Address[0].County, account[Attributes.Account.Address1County].ToString());
            Assert.AreEqual(c.Address[0].Country, ((EntityReference)(account[Attributes.Account.Address1CountryId])).Id.ToString());
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
                    SourceMarket = Guid.NewGuid().ToString(),
                    SourceSystem = "On Tour"
                },
                Company = new Company { CompanyName = "Test" },
                Address = new Address[]
                {
                    new Address
                    {
                        AdditionalAddressInfo = "Tes",
                        AddressType = AddressType.Main,
                        Country = Guid.NewGuid().ToString(),
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
                        Country = Guid.NewGuid().ToString(),
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


            Assert.AreEqual(c.Address[0].AdditionalAddressInfo, account[Attributes.Account.Address1AdditionalInformation].ToString());
            Assert.AreEqual(c.Address[0].FlatNumberUnit, account[Attributes.Account.Address1FlatOrUnitNumber].ToString());
            Assert.AreEqual(c.Address[0].HouseNumberBuilding, account[Attributes.Account.Address1HouseNumberOrBuilding].ToString());
            Assert.AreEqual(c.Address[0].Town, account[Attributes.Account.Address1Town].ToString());
            Assert.AreEqual(c.Address[0].PostalCode, account[Attributes.Account.Address1PostalCode].ToString());
            Assert.AreEqual(c.Address[0].County, account[Attributes.Account.Address1County].ToString());
            Assert.AreEqual(c.Address[0].Country, ((EntityReference)(account[Attributes.Account.Address1CountryId])).Id.ToString());
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
                    SourceMarket = Guid.NewGuid().ToString(),
                    SourceSystem = "On Tour"
                },
                Company = new Company { CompanyName = "Test" },
                Address = new Address[]
                {

                }
            };
            var account = AccountHelper.GetAccountEntityForBookingPayload(c, trace);


            Assert.AreEqual(true, account.Contains(Attributes.Account.Address1AdditionalInformation));
            Assert.AreEqual(true, account.Contains(Attributes.Account.Address1FlatOrUnitNumber));
            Assert.AreEqual(true, account.Contains(Attributes.Account.Address1HouseNumberOrBuilding));
            Assert.AreEqual(true, account.Contains(Attributes.Account.Address1Town));
            Assert.AreEqual(true, account.Contains(Attributes.Account.Address1PostalCode));
            Assert.AreEqual(true, account.Contains(Attributes.Account.Address1County));
            Assert.AreEqual(true, account.Contains(Attributes.Account.Address1CountryId));

            Assert.AreEqual(string.Empty,account[Attributes.Account.Address1AdditionalInformation]);
            Assert.AreEqual(string.Empty, account[Attributes.Account.Address1FlatOrUnitNumber]);
            Assert.AreEqual(string.Empty, account[Attributes.Account.Address1HouseNumberOrBuilding]);
            Assert.AreEqual(string.Empty, account[Attributes.Account.Address1Town]);
            Assert.AreEqual(string.Empty, account[Attributes.Account.Address1PostalCode]);
            Assert.AreEqual(string.Empty, account[Attributes.Account.Address1County]);
            Assert.IsNull(account[Attributes.Account.Address1CountryId]);
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
                    SourceMarket = Guid.NewGuid().ToString(),
                    SourceSystem = "On Tour"
                },
                Company = new Company { CompanyName = "Test" },
                Address = new Address[]
                {
                    null
                }
            };
            var account = AccountHelper.GetAccountEntityForBookingPayload(c, trace);


            Assert.AreEqual(false, account.Contains(Attributes.Account.Address1AdditionalInformation));
            Assert.AreEqual(false, account.Contains(Attributes.Account.Address1FlatOrUnitNumber));
            Assert.AreEqual(false, account.Contains(Attributes.Account.Address1HouseNumberOrBuilding));
            Assert.AreEqual(false, account.Contains(Attributes.Account.Address1Town));
            Assert.AreEqual(false, account.Contains(Attributes.Account.Address1PostalCode));
            Assert.AreEqual(false, account.Contains(Attributes.Account.Address1County));
            Assert.AreEqual(false, account.Contains(Attributes.Account.Address1CountryId));
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
            Assert.AreEqual(true, account.Contains(Attributes.Account.Address1AdditionalInformation));
            Assert.AreEqual(true, account.Contains(Attributes.Account.Address1FlatOrUnitNumber));
            Assert.AreEqual(true, account.Contains(Attributes.Account.Address1HouseNumberOrBuilding));
            Assert.AreEqual(true, account.Contains(Attributes.Account.Address1Town));
            Assert.AreEqual(true, account.Contains(Attributes.Account.Address1PostalCode));
            Assert.AreEqual(true, account.Contains(Attributes.Account.Address1County));
            Assert.AreEqual(true, account.Contains(Attributes.Account.Address1CountryId));

            Assert.AreEqual(string.Empty, account[Attributes.Account.Address1AdditionalInformation]);
            Assert.AreEqual(string.Empty, account[Attributes.Account.Address1FlatOrUnitNumber]);
            Assert.AreEqual(string.Empty, account[Attributes.Account.Address1HouseNumberOrBuilding]);
            Assert.AreEqual(string.Empty, account[Attributes.Account.Address1Town]);
            Assert.AreEqual(string.Empty, account[Attributes.Account.Address1PostalCode]);
            Assert.AreEqual(string.Empty, account[Attributes.Account.Address1County]);
            Assert.IsNull(account[Attributes.Account.Address1CountryId]);
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
                    SourceMarket = Guid.NewGuid().ToString(),
                    SourceSystem = "On Tour"
                },
                Company = new Company { CompanyName = "Test" },
                Email = new Email[]
                {
                    new Email {Address="one@tc.com",EmailType = EmailType.Primary },
                    new Email {Address="two@tc.com",EmailType = EmailType.Promo },
                    new Email {Address="three@tc.com",EmailType = EmailType.Promo }
                }
            };
            var account = AccountHelper.GetAccountEntityForBookingPayload(c, trace);


            Assert.AreEqual(c.Email[0].Address, account[Attributes.Account.EmailAddress1].ToString());
            Assert.AreEqual(c.Email[1].Address, account[Attributes.Account.EmailAddress2].ToString());
            Assert.AreEqual(c.Email[2].Address, account[Attributes.Account.EmailAddress3].ToString());
            Assert.AreEqual(950000000, ((OptionSetValue)(account[Attributes.Account.EmailAddress1Type])).Value);
            Assert.AreEqual(950000001, ((OptionSetValue)(account[Attributes.Account.EmailAddress2Type])).Value);
            Assert.AreEqual(950000001, ((OptionSetValue)(account[Attributes.Account.EmailAddress3Type])).Value);

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
                    SourceMarket = Guid.NewGuid().ToString(),
                    SourceSystem = "On Tour"
                },
                Company = new Company { CompanyName = "Test" },
                Phone = new Phone[]
               {
                   new Phone {Number="12345",PhoneType=PhoneType.Home },
                    new Phone {Number="123456",PhoneType=PhoneType.Mobile },
                     new Phone {Number="1234567",PhoneType=PhoneType.Mobile }
               }
            };
            var account = AccountHelper.GetAccountEntityForBookingPayload(c, trace);


            Assert.AreEqual(c.Phone[0].Number, account[Attributes.Account.Telephone1].ToString());
            Assert.AreEqual(c.Phone[1].Number, account[Attributes.Account.Telephone2].ToString());
            Assert.AreEqual(c.Phone[2].Number, account[Attributes.Account.Telephone3].ToString());
            Assert.AreEqual(950000001, ((OptionSetValue)(account[Attributes.Account.Telephone1Type])).Value);
            Assert.AreEqual(950000000, ((OptionSetValue)(account[Attributes.Account.Telephone2Type])).Value);
            Assert.AreEqual(950000000, ((OptionSetValue)(account[Attributes.Account.Telephone3Type])).Value);

        }
        #endregion
    }
}