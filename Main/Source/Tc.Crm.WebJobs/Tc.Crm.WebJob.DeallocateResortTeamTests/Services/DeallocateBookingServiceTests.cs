using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Tc.Crm.WebJob.DeallocateResortTeamTests;
using Tc.Crm.Common.Services;
using Microsoft.Xrm.Sdk;
using Tc.Crm.Common.Constants;
using Attributes = Tc.Crm.Common.Constants.Attributes;
using Tc.Crm.Common;

namespace Tc.Crm.WebJob.DeallocateResortTeam.Services.Tests
{
    [TestClass()]
    public class DeallocateBookingServiceTests
    {
        ILogger logger;
        IConfigurationService configurationService;
        IDeallocationService deallocationService;
        TestCrmService crmService;
        public TestContext TestContext{ get; set; }

        [TestInitialize()]
        public void Setup()
        {
            this.logger = new TestLogger();
            this.configurationService = new TestConfigurationService();
            this.crmService = new TestCrmService();
            this.deallocationService = new DeallocationService(this.logger, this.crmService);
        }


        /// <summary>
        /// Booking departure date is sooner then 2 days ago, gateway is valid, owner is hotel team
        /// Expected Result: Booking.Owner not changed  
        /// </summary>
        [TestMethod()]
        public void TestDepartureDateSoonerThanToBeProcessed()
        {
            crmService.PrepareData();
            // setup
            var team = crmService.AddTeam(CreateReference(EntityName.BusinessUnit, Attributes.Team.BusinessUnitId, crmService.BusinessUnitUK.Id), true);
            var booking = crmService.AddBooking(DateTime.Now.AddDays(-3),
                CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id),
                CreateReference(EntityName.Gateway, Attributes.Booking.DestinationGatewayId, crmService.ValidGatewayIds[0]),
                CreateReference(EntityName.Country, Attributes.Booking.SourceMarketId, crmService.SourceMarketCountryUK.Id));

            using (var service = new DeallocateBookingService(logger, deallocationService, configurationService))
            {
                service.Run();
            }

            //asserts
            Assert.AreEqual(team.Id, ((EntityReference)booking[Attributes.Booking.Owner]).Id);
        }

        /// <summary>
        /// Booking departure date is later then 2 days ago, gateway is valid, owner is hotel team
        /// Expected Result: Booking.Owner not changed  
        /// </summary>
        [TestMethod()]
        public void TestDepartureDateLaterThanToBeProcessed()
        {
            crmService.PrepareData();

            // setup
            var team = crmService.AddTeam(CreateReference(EntityName.BusinessUnit, Attributes.Team.BusinessUnitId, crmService.BusinessUnitUK.Id), true);
            var booking = crmService.AddBooking(DateTime.Now.AddDays(-1),
                CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id),
                CreateReference(EntityName.Gateway, Attributes.Booking.DestinationGatewayId, crmService.ValidGatewayIds[0]),
                CreateReference(EntityName.Country, Attributes.Booking.SourceMarketId, crmService.SourceMarketCountryUK.Id));

            using (var service = new DeallocateBookingService(logger, deallocationService, configurationService))
            {
                service.Run();
            }

            //asserts
            Assert.AreEqual(team.Id, ((EntityReference)booking[Attributes.Booking.Owner]).Id);
        }

        /// <summary>
        /// Booking departure date was 2 days ago, gateway is valid, owner of booking is not hotel team
        /// Expected Result: Booking.Owner not changed  
        /// </summary>
        [TestMethod()]
        public void TestOwnerNotHotelTeam()
        {
            crmService.PrepareData();
            // setup
            var team = crmService.AddTeam(CreateReference(EntityName.BusinessUnit, Attributes.Team.BusinessUnitId, crmService.BusinessUnitUK.Id), false);
            var booking = crmService.AddBooking(DateTime.Now.AddDays(-2),
                CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id),
                CreateReference(EntityName.Gateway, Attributes.Booking.DestinationGatewayId, crmService.ValidGatewayIds[0]),
                CreateReference(EntityName.Country, Attributes.Booking.SourceMarketId, crmService.SourceMarketCountryUK.Id));

            using (var service = new DeallocateBookingService(logger, deallocationService, configurationService))
            {
                service.Run();
            }

            //asserts
            Assert.AreEqual(team.Id, ((EntityReference)booking[Attributes.Booking.Owner]).Id);
        }

        /// <summary>
        /// Booking departure date was 2 days ago, gateway is valid, owner of booking is user
        /// Expected Result: Booking.Owner not changed  
        /// </summary>
        [TestMethod()]
        public void TestOwnerUser()
        {
            crmService.PrepareData();
            // setup
            var booking = crmService.AddBooking(DateTime.Now.AddDays(-2),
                null,
                CreateReference(EntityName.Gateway, Attributes.Booking.DestinationGatewayId, crmService.ValidGatewayIds[0]),
                CreateReference(EntityName.Country, Attributes.Booking.SourceMarketId, crmService.SourceMarketCountryUK.Id));

            using (var service = new DeallocateBookingService(logger, deallocationService, configurationService))
            {
                service.Run();
            }

            //asserts
            Assert.AreEqual(crmService.SystemUser.Id, ((EntityReference)booking[Attributes.Booking.Owner]).Id);
        }

        /// <summary>
        /// Booking departure date was 2 days ago, owner of booking is hotel team, gateway is not valid
        /// Expected Result: Booking.Owner not changed  
        /// </summary>
        [TestMethod()]
        public void TestGatewayNotValid()
        {
            crmService.PrepareData();
            // setup
            var team = crmService.AddTeam(CreateReference(EntityName.BusinessUnit, Attributes.Team.BusinessUnitId, crmService.BusinessUnitUK.Id), true);
            var gateway = crmService.AddGateway();
            var booking = crmService.AddBooking(DateTime.Now.AddDays(-2),
                CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id),
                CreateReference(EntityName.Gateway, Attributes.Booking.DestinationGatewayId, gateway.Id),
                CreateReference(EntityName.Country, Attributes.Booking.SourceMarketId, crmService.SourceMarketCountryUK.Id));

            using (var service = new DeallocateBookingService(logger, deallocationService, configurationService))
            {
                service.Run();
            }

            //asserts
            Assert.AreEqual(team.Id, ((EntityReference)booking[Attributes.Booking.Owner]).Id);
        }

        /// <summary>
        /// Booking departure date was 2 days ago, gateway is valid, owner of booking is hotel team
        /// Expected Result: Booking.Owner changed to default team
        /// </summary>
        [TestMethod()]
        public void TestValidBooking()
        {
            crmService.PrepareData();
            // setup
            var team = crmService.AddTeam(CreateReference(EntityName.BusinessUnit, Attributes.Team.BusinessUnitId, crmService.BusinessUnitUK.Id), true);
            var booking = crmService.AddBooking(DateTime.Now.AddDays(-2),
                CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id),
                CreateReference(EntityName.Gateway, Attributes.Booking.DestinationGatewayId, crmService.ValidGatewayIds[0]),
                CreateReference(EntityName.Country, Attributes.Booking.SourceMarketId, crmService.SourceMarketCountryUK.Id));

            using (var service = new DeallocateBookingService(logger, deallocationService, configurationService))
            {
                service.Run();
            }

            //asserts
            Assert.AreEqual(crmService.DefaultTeamUK.Id, ((EntityReference)booking[Attributes.Booking.Owner]).Id);
        }

        /// <summary>
        /// Booking departure date was 2 days ago, gateway is valid, owner of booking is hotel team
        /// Expected Result: Booking.Owner, Contact.Owner changed to default team
        /// </summary>
        [TestMethod()]
        public void TestValidBookingWithContactCustomer()
        {
            crmService.PrepareData();
            // setup
            var team = crmService.AddTeam(CreateReference(EntityName.BusinessUnit, Attributes.Team.BusinessUnitId, crmService.BusinessUnitUK.Id), true);
            var contact = crmService.AddContact(CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id));
            var booking = crmService.AddBooking(DateTime.Now.AddDays(-2),
                CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id),
                CreateReference(EntityName.Gateway, Attributes.Booking.DestinationGatewayId, crmService.ValidGatewayIds[0]),
                CreateReference(EntityName.Country, Attributes.Booking.SourceMarketId, crmService.SourceMarketCountryUK.Id));
            crmService.AddCustomerBookingRole(
                CreateReference(EntityName.Booking, Attributes.CustomerBookingRole.BookingId, booking.Id),
                CreateReference(EntityName.Contact, Attributes.CustomerBookingRole.Customer, contact.Id));

            using (var service = new DeallocateBookingService(logger, deallocationService, configurationService))
            {
                service.Run();
            }

            //asserts
            Assert.AreEqual(crmService.DefaultTeamUK.Id, ((EntityReference)booking[Attributes.Booking.Owner]).Id);
            Assert.AreEqual(crmService.DefaultTeamUK.Id, ((EntityReference)contact[Attributes.Contact.Owner]).Id);
        }

        /// <summary>
        /// Booking departure date was 2 days ago, gateway is valid, owner of booking is hotel team
        /// Expected Result: Booking.Owner, Account.Owner changed to default team
        /// </summary>
        [TestMethod()]
        public void TestValidBookingWithAccountCustomer()
        {
            crmService.PrepareData();
            // setup
            var team = crmService.AddTeam(CreateReference(EntityName.BusinessUnit, Attributes.Team.BusinessUnitId, crmService.BusinessUnitUK.Id), true);
            var account = crmService.AddAccount(CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id));
            var booking = crmService.AddBooking(DateTime.Now.AddDays(-2),
                CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id),
                CreateReference(EntityName.Gateway, Attributes.Booking.DestinationGatewayId, crmService.ValidGatewayIds[0]),
                CreateReference(EntityName.Country, Attributes.Booking.SourceMarketId, crmService.SourceMarketCountryUK.Id));
            crmService.AddCustomerBookingRole(
                CreateReference(EntityName.Booking, Attributes.CustomerBookingRole.BookingId, booking.Id),
                CreateReference(EntityName.Account, Attributes.CustomerBookingRole.Customer, account.Id));

            using (var service = new DeallocateBookingService(logger, deallocationService, configurationService))
            {
                service.Run();
            }

            //asserts
            Assert.AreEqual(crmService.DefaultTeamUK.Id, ((EntityReference)booking[Attributes.Booking.Owner]).Id);
            Assert.AreEqual(crmService.DefaultTeamUK.Id, ((EntityReference)account[Attributes.Account.Owner]).Id);
        }

        /// <summary>
        /// Booking departure date was 2 days ago, gateway is valid, owner of booking is hotel team, booking is not UK market
        /// Expected Result: Booking.Owner, Contact.Owner, Case.Owner changed to default team. Case status changed
        /// </summary>
        [TestMethod()]
        public void TestValidBookingWithContactCustomerAndCaseNonUKMarket()
        {
            crmService.PrepareData();
            // setup
            var team = crmService.AddTeam(CreateReference(EntityName.BusinessUnit, Attributes.Team.BusinessUnitId, crmService.BusinessUnitNonUK.Id), true);
            var contact = crmService.AddContact(CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id));
            var booking = crmService.AddBooking(DateTime.Now.AddDays(-2),
                CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id),
                CreateReference(EntityName.Gateway, Attributes.Booking.DestinationGatewayId, crmService.ValidGatewayIds[0]),
                CreateReference(EntityName.Country, Attributes.Booking.SourceMarketId, crmService.SourceMarketCountryNonUK.Id));
            crmService.AddCustomerBookingRole(
                CreateReference(EntityName.Booking, Attributes.CustomerBookingRole.BookingId, booking.Id),
                CreateReference(EntityName.Contact, Attributes.CustomerBookingRole.Customer, contact.Id));
            var _case = crmService.AddCase(
                CreateReference(EntityName.Contact, Attributes.Case.CustomerId, contact.Id),
                CreateReference(EntityName.Team, Attributes.Booking.OwningTeam, team.Id)
                );

            using (var service = new DeallocateBookingService(logger, deallocationService, configurationService))
            {
                service.Run();
            }

            //asserts
            Assert.AreEqual(crmService.DefaultTeamNonUK.Id, ((EntityReference)booking[Attributes.Booking.Owner]).Id);
            Assert.AreEqual(crmService.DefaultTeamNonUK.Id, ((EntityReference)contact[Attributes.Contact.Owner]).Id);
            Assert.AreNotEqual(crmService.DefaultTeamUK.Id, ((EntityReference)_case[Attributes.Case.Owner]).Id);
           
        }

        /// <summary>
        /// Booking departure date was 2 days ago, gateway is valid, owner of booking is hotel team, booking is UK market
        /// Expected Result: Booking.Owner, Contact.Owner changed to default team, Case.Owner is not changed, case status is not changed
        /// </summary>
        [TestMethod()]
        public void TestValidBookingWithContactCustomerAndCaseUKMarket()
        {
            crmService.PrepareData();
            // setup
            var team = crmService.AddTeam(CreateReference(EntityName.BusinessUnit, Attributes.Team.BusinessUnitId, crmService.BusinessUnitUK.Id), true);
            var contact = crmService.AddContact(CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id));
            var booking = crmService.AddBooking(DateTime.Now.AddDays(-2),
                CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id),
                CreateReference(EntityName.Gateway, Attributes.Booking.DestinationGatewayId, crmService.ValidGatewayIds[0]),
                CreateReference(EntityName.Country, Attributes.Booking.SourceMarketId, crmService.SourceMarketCountryUK.Id));
            crmService.AddCustomerBookingRole(
                CreateReference(EntityName.Booking, Attributes.CustomerBookingRole.BookingId, booking.Id),
                CreateReference(EntityName.Contact, Attributes.CustomerBookingRole.Customer, contact.Id));
            var _case = crmService.AddCase(
                CreateReference(EntityName.Contact, Attributes.Case.CustomerId, contact.Id),
                CreateReference(EntityName.Team, Attributes.Booking.OwningTeam, team.Id)
                );

            using (var service = new DeallocateBookingService(logger, deallocationService, configurationService))
            {
                service.Run();
            }

            //asserts
            Assert.AreEqual(crmService.DefaultTeamUK.Id, ((EntityReference)booking[Attributes.Booking.Owner]).Id);
            Assert.AreEqual(crmService.DefaultTeamUK.Id, ((EntityReference)contact[Attributes.Contact.Owner]).Id);
            Assert.AreNotEqual(crmService.DefaultTeamUK.Id, ((EntityReference)_case[Attributes.Case.Owner]).Id);
            Assert.AreEqual((int)CaseState.Active, ((OptionSetValue)_case[Attributes.Case.State]).Value);
            Assert.AreEqual((int)CaseStatusCode.AssignedToLocalSourceMarket, ((OptionSetValue)_case[Attributes.Case.StatusReason]).Value);

        }

        /// <summary>
        /// Booking departure date was 2 days ago, gateway is valid, owner of booking is hotel team, booking is not UK market
        /// Expected Result: Booking.Owner, Account.Owner, Case.Owner changed to default team. Case status changed
        /// </summary>
        [TestMethod()]
        public void TestValidBookingWithAccountCustomerAndCaseNonUKMarket()
        {
            crmService.PrepareData();
            // setup
            var team = crmService.AddTeam(CreateReference(EntityName.BusinessUnit, Attributes.Team.BusinessUnitId, crmService.BusinessUnitNonUK.Id), true);
            var account = crmService.AddAccount(CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id));
            var booking = crmService.AddBooking(DateTime.Now.AddDays(-2),
                CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id),
                CreateReference(EntityName.Gateway, Attributes.Booking.DestinationGatewayId, crmService.ValidGatewayIds[0]),
                CreateReference(EntityName.Country, Attributes.Booking.SourceMarketId, crmService.SourceMarketCountryNonUK.Id));
            crmService.AddCustomerBookingRole(
                CreateReference(EntityName.Booking, Attributes.CustomerBookingRole.BookingId, booking.Id),
                CreateReference(EntityName.Account, Attributes.CustomerBookingRole.Customer, account.Id));
            var _case = crmService.AddCase(
                CreateReference(EntityName.Account, Attributes.Case.CustomerId, account.Id),
                CreateReference(EntityName.Team, Attributes.Booking.OwningTeam, team.Id)
                );

            using (var service = new DeallocateBookingService(logger, deallocationService, configurationService))
            {
                service.Run();
            }

            //asserts
            Assert.AreEqual(crmService.DefaultTeamNonUK.Id, ((EntityReference)booking[Attributes.Booking.Owner]).Id);
            Assert.AreEqual(crmService.DefaultTeamNonUK.Id, ((EntityReference)account[Attributes.Account.Owner]).Id);
            Assert.AreNotEqual(crmService.DefaultTeamNonUK.Id, ((EntityReference)_case[Attributes.Case.Owner]).Id);
          
        }

        /// <summary>
        /// Booking departure date was 2 days ago, gateway is valid, owner of booking is hotel team, booking is UK market
        /// Expected Result: Booking.Owner, Account.Owner changed to default team, Case.Owner is not changed, case status is not changed
        /// </summary>
        [TestMethod()]
        public void TestValidBookingWithAccountCustomerAndCaseUKMarket()
        {
            crmService.PrepareData();
            // setup
            var team = crmService.AddTeam(CreateReference(EntityName.BusinessUnit, Attributes.Team.BusinessUnitId, crmService.BusinessUnitUK.Id), true);
            var account = crmService.AddAccount(CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id));
            var booking = crmService.AddBooking(DateTime.Now.AddDays(-2),
                CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id),
                CreateReference(EntityName.Gateway, Attributes.Booking.DestinationGatewayId, crmService.ValidGatewayIds[0]),
                CreateReference(EntityName.Country, Attributes.Booking.SourceMarketId, crmService.SourceMarketCountryUK.Id));
            crmService.AddCustomerBookingRole(
                CreateReference(EntityName.Booking, Attributes.CustomerBookingRole.BookingId, booking.Id),
                CreateReference(EntityName.Account, Attributes.CustomerBookingRole.Customer, account.Id));
            var _case = crmService.AddCase(
                CreateReference(EntityName.Account, Attributes.Case.CustomerId, account.Id),
                CreateReference(EntityName.Team, Attributes.Booking.OwningTeam, team.Id)
                );

            using (var service = new DeallocateBookingService(logger, deallocationService, configurationService))
            {
                service.Run();
            }

            //asserts
            Assert.AreEqual(crmService.DefaultTeamUK.Id, ((EntityReference)booking[Attributes.Booking.Owner]).Id);
            Assert.AreEqual(crmService.DefaultTeamUK.Id, ((EntityReference)account[Attributes.Account.Owner]).Id);
            Assert.AreNotEqual(crmService.DefaultTeamUK.Id, ((EntityReference)_case[Attributes.Case.Owner]).Id);
            Assert.AreEqual((int)CaseState.Active, ((OptionSetValue)_case[Attributes.Case.State]).Value);
            Assert.AreEqual((int)CaseStatusCode.AssignedToLocalSourceMarket, ((OptionSetValue)_case[Attributes.Case.StatusReason]).Value);
        }

        /// <summary>
        /// Bulk test on UK and non UK source markets, multiple bookings with customers and cases 
        /// Expected Result: Booking.Owner, Contact.Owner, Case.Owner changed to default team for proper bookings
        /// </summary>
        [TestMethod()]
        public void TestValidBookingMultipleSourceMarketMultipleData()
        {
            crmService.PrepareData();
            // setup
            var hotelTeamUk1 = crmService.AddTeam(CreateReference(EntityName.BusinessUnit, Attributes.Team.BusinessUnitId, crmService.BusinessUnitUK.Id), true);
            var hotelTeamUk2 = crmService.AddTeam(CreateReference(EntityName.BusinessUnit, Attributes.Team.BusinessUnitId, crmService.BusinessUnitUK.Id), true);

            var hotelTeamNonUk1 = crmService.AddTeam(CreateReference(EntityName.BusinessUnit, Attributes.Team.BusinessUnitId, crmService.BusinessUnitNonUK.Id), true);
            var hotelTeamNonUk2 = crmService.AddTeam(CreateReference(EntityName.BusinessUnit, Attributes.Team.BusinessUnitId, crmService.BusinessUnitNonUK.Id), true);

            // UK market booking owner user
            var contactUkOwnerUser = crmService.AddContact(null);
            var bookingUkOwnerUser = crmService.AddBooking(DateTime.Now.AddDays(-2),
                null,
                CreateReference(EntityName.Gateway, Attributes.Booking.DestinationGatewayId, crmService.ValidGatewayIds[0]),
                CreateReference(EntityName.Country, Attributes.Booking.SourceMarketId, crmService.SourceMarketCountryUK.Id));
            crmService.AddCustomerBookingRole(
                CreateReference(EntityName.Booking, Attributes.CustomerBookingRole.BookingId, bookingUkOwnerUser.Id),
                CreateReference(EntityName.Contact, Attributes.CustomerBookingRole.Customer, contactUkOwnerUser.Id));
            var _caseUkOwnerUser = crmService.AddCase(
                CreateReference(EntityName.Contact, Attributes.Case.CustomerId, contactUkOwnerUser.Id),
                null);
            // UK market booking, 2 customers with cases
            var contactUk11 = crmService.AddContact(null);
            var contactUk12 = crmService.AddContact(null);
            var bookingUk1 = crmService.AddBooking(DateTime.Now.AddDays(-2),
                CreateReference(EntityName.Team, Attributes.Booking.OwningTeam, hotelTeamUk1.Id),
                CreateReference(EntityName.Gateway, Attributes.Booking.DestinationGatewayId, crmService.ValidGatewayIds[0]),
                CreateReference(EntityName.Country, Attributes.Booking.SourceMarketId, crmService.SourceMarketCountryUK.Id));
            crmService.AddCustomerBookingRole(
                CreateReference(EntityName.Booking, Attributes.CustomerBookingRole.BookingId, bookingUk1.Id),
                CreateReference(EntityName.Contact, Attributes.CustomerBookingRole.Customer, contactUk11.Id));
            crmService.AddCustomerBookingRole(
                CreateReference(EntityName.Booking, Attributes.CustomerBookingRole.BookingId, bookingUk1.Id),
                CreateReference(EntityName.Contact, Attributes.CustomerBookingRole.Customer, contactUk12.Id));
            var _caseUk11 = crmService.AddCase(
                CreateReference(EntityName.Contact, Attributes.Case.CustomerId, contactUk11.Id), 
                null);
            var _caseUk12 = crmService.AddCase(
                CreateReference(EntityName.Contact, Attributes.Case.CustomerId, contactUk11.Id),
                null);
            var _caseUk13 = crmService.AddCase(
                CreateReference(EntityName.Contact, Attributes.Case.CustomerId, contactUk12.Id),
                null);
            // UK market booking, 3 customers
            var contactUk21 = crmService.AddContact(null);
            var contactUk22 = crmService.AddContact(null);
            var accountUk23 = crmService.AddAccount(null);
            var bookingUk2 = crmService.AddBooking(DateTime.Now.AddDays(-2),
                CreateReference(EntityName.Team, Attributes.Booking.OwningTeam, hotelTeamUk2.Id),
                CreateReference(EntityName.Gateway, Attributes.Booking.DestinationGatewayId, crmService.ValidGatewayIds[0]),
                CreateReference(EntityName.Country, Attributes.Booking.SourceMarketId, crmService.SourceMarketCountryUK.Id));
            crmService.AddCustomerBookingRole(
                CreateReference(EntityName.Booking, Attributes.CustomerBookingRole.BookingId, bookingUk1.Id),
                CreateReference(EntityName.Contact, Attributes.CustomerBookingRole.Customer, contactUk21.Id));
            crmService.AddCustomerBookingRole(
                CreateReference(EntityName.Booking, Attributes.CustomerBookingRole.BookingId, bookingUk1.Id),
                CreateReference(EntityName.Contact, Attributes.CustomerBookingRole.Customer, contactUk22.Id));
            crmService.AddCustomerBookingRole(
                CreateReference(EntityName.Booking, Attributes.CustomerBookingRole.BookingId, bookingUk1.Id),
                CreateReference(EntityName.Contact, Attributes.CustomerBookingRole.Customer, accountUk23.Id));
            var _caseUk21 = crmService.AddCase(
                CreateReference(EntityName.Contact, Attributes.Case.CustomerId, contactUk22.Id),
                null);
            // Non UK market booking owner user
            var contactNonUkOwnerUser = crmService.AddContact(null);
            var bookingNonUkOwnerUser = crmService.AddBooking(DateTime.Now.AddDays(-2),
                null,
                CreateReference(EntityName.Gateway, Attributes.Booking.DestinationGatewayId, crmService.ValidGatewayIds[0]),
                CreateReference(EntityName.Country, Attributes.Booking.SourceMarketId, crmService.SourceMarketCountryNonUK.Id));
            crmService.AddCustomerBookingRole(
                CreateReference(EntityName.Booking, Attributes.CustomerBookingRole.BookingId, bookingNonUkOwnerUser.Id),
                CreateReference(EntityName.Contact, Attributes.CustomerBookingRole.Customer, contactNonUkOwnerUser.Id));
            var _caseNonUkOwnerUser = crmService.AddCase(
                CreateReference(EntityName.Contact, Attributes.Case.CustomerId, contactNonUkOwnerUser.Id),
                null);
            // Non UK market booking, 2 customers\Accounts with cases
            var contactNonUk11 = crmService.AddContact(null);
            var accountNonUk12 = crmService.AddAccount(null);
            var bookingNonUk1 = crmService.AddBooking(DateTime.Now.AddDays(-2),
                CreateReference(EntityName.Team, Attributes.Booking.OwningTeam, hotelTeamNonUk1.Id),
                CreateReference(EntityName.Gateway, Attributes.Booking.DestinationGatewayId, crmService.ValidGatewayIds[0]),
                CreateReference(EntityName.Country, Attributes.Booking.SourceMarketId, crmService.SourceMarketCountryNonUK.Id));
            crmService.AddCustomerBookingRole(
                CreateReference(EntityName.Booking, Attributes.CustomerBookingRole.BookingId, bookingNonUk1.Id),
                CreateReference(EntityName.Contact, Attributes.CustomerBookingRole.Customer, contactNonUk11.Id));
            crmService.AddCustomerBookingRole(
                CreateReference(EntityName.Booking, Attributes.CustomerBookingRole.BookingId, bookingNonUk1.Id),
                CreateReference(EntityName.Contact, Attributes.CustomerBookingRole.Customer, accountNonUk12.Id));
            var _caseNonUk11 = crmService.AddCase(
                CreateReference(EntityName.Contact, Attributes.Case.CustomerId, contactNonUk11.Id),
                null);
            var _caseNonUk12 = crmService.AddCase(
                CreateReference(EntityName.Contact, Attributes.Case.CustomerId, contactNonUk11.Id),
                null);
            var _caseNonUk13 = crmService.AddCase(
                CreateReference(EntityName.Contact, Attributes.Case.CustomerId, accountNonUk12.Id),
                null);
            // Non UK market booking, 3 customers
            var contactNonUk21 = crmService.AddContact(null);
            var accountNonUk22 = crmService.AddAccount(null);
            var accountNonUk23 = crmService.AddAccount(null);
            var bookingNonUk2 = crmService.AddBooking(DateTime.Now.AddDays(-2),
                CreateReference(EntityName.Team, Attributes.Booking.OwningTeam, hotelTeamNonUk2.Id),
                CreateReference(EntityName.Gateway, Attributes.Booking.DestinationGatewayId, crmService.ValidGatewayIds[0]),
                CreateReference(EntityName.Country, Attributes.Booking.SourceMarketId, crmService.SourceMarketCountryNonUK.Id));
            crmService.AddCustomerBookingRole(
                CreateReference(EntityName.Booking, Attributes.CustomerBookingRole.BookingId, bookingNonUk2.Id),
                CreateReference(EntityName.Contact, Attributes.CustomerBookingRole.Customer, contactNonUk21.Id));
            crmService.AddCustomerBookingRole(
                CreateReference(EntityName.Booking, Attributes.CustomerBookingRole.BookingId, bookingNonUk2.Id),
                CreateReference(EntityName.Contact, Attributes.CustomerBookingRole.Customer, accountNonUk22.Id));
            crmService.AddCustomerBookingRole(
                CreateReference(EntityName.Booking, Attributes.CustomerBookingRole.BookingId, bookingNonUk2.Id),
                CreateReference(EntityName.Contact, Attributes.CustomerBookingRole.Customer, accountNonUk23.Id));
            var _caseNonUk21 = crmService.AddCase(
                CreateReference(EntityName.Contact, Attributes.Case.CustomerId, contactNonUk21.Id),
                null);
            var _caseNonUk22 = crmService.AddCase(
                CreateReference(EntityName.Contact, Attributes.Case.CustomerId, accountNonUk22.Id),
                null);

            using (var service = new DeallocateBookingService(logger, deallocationService, configurationService))
            {
                service.Run();
            }

            //asserts
            Assert.AreEqual(crmService.SystemUser.Id, ((EntityReference)contactUkOwnerUser[Attributes.Contact.Owner]).Id);
            Assert.AreEqual(crmService.SystemUser.Id, ((EntityReference)bookingUkOwnerUser[Attributes.Booking.Owner]).Id);
            Assert.AreEqual(crmService.SystemUser.Id, ((EntityReference)_caseUkOwnerUser[Attributes.Case.Owner]).Id);
            Assert.IsFalse(_caseUkOwnerUser.Contains(Attributes.Case.State));
            Assert.IsFalse(_caseUkOwnerUser.Contains(Attributes.Case.StatusReason));
            Assert.AreEqual(crmService.DefaultTeamUK.Id, ((EntityReference)contactUk11[Attributes.Contact.Owner]).Id);
            Assert.AreEqual(crmService.DefaultTeamUK.Id, ((EntityReference)contactUk12[Attributes.Contact.Owner]).Id);
            Assert.AreEqual(crmService.DefaultTeamUK.Id, ((EntityReference)bookingUk1[Attributes.Booking.Owner]).Id);
            Assert.AreEqual(crmService.SystemUser.Id, ((EntityReference)_caseUk11[Attributes.Case.Owner]).Id);
            Assert.IsTrue(_caseUk11.Contains(Attributes.Case.State));
            Assert.IsTrue(_caseUk11.Contains(Attributes.Case.StatusReason));
            Assert.AreEqual(crmService.SystemUser.Id, ((EntityReference)_caseUk12[Attributes.Case.Owner]).Id);
            Assert.IsTrue(_caseUk12.Contains(Attributes.Case.State));
            Assert.IsTrue(_caseUk12.Contains(Attributes.Case.StatusReason));
            Assert.AreEqual(crmService.SystemUser.Id, ((EntityReference)_caseUk13[Attributes.Case.Owner]).Id);
            Assert.IsTrue(_caseUk13.Contains(Attributes.Case.State));
            Assert.IsTrue(_caseUk13.Contains(Attributes.Case.StatusReason));
            Assert.AreEqual(crmService.DefaultTeamUK.Id, ((EntityReference)contactUk21[Attributes.Contact.Owner]).Id);
            Assert.AreEqual(crmService.DefaultTeamUK.Id, ((EntityReference)contactUk22[Attributes.Contact.Owner]).Id);
            Assert.AreEqual(crmService.DefaultTeamUK.Id, ((EntityReference)accountUk23[Attributes.Account.Owner]).Id);
            Assert.AreEqual(crmService.DefaultTeamUK.Id, ((EntityReference)bookingUk2[Attributes.Booking.Owner]).Id);
            Assert.AreEqual(crmService.SystemUser.Id, ((EntityReference)_caseUk21[Attributes.Case.Owner]).Id);
            Assert.IsTrue(_caseUk21.Contains(Attributes.Case.State));
            Assert.IsTrue(_caseUk21.Contains(Attributes.Case.StatusReason));
            Assert.AreEqual(crmService.SystemUser.Id, ((EntityReference)contactNonUkOwnerUser[Attributes.Contact.Owner]).Id);
            Assert.AreEqual(crmService.SystemUser.Id, ((EntityReference)bookingNonUkOwnerUser[Attributes.Booking.Owner]).Id);
            Assert.AreEqual(crmService.SystemUser.Id, ((EntityReference)_caseNonUkOwnerUser[Attributes.Case.Owner]).Id);
            Assert.IsFalse(_caseNonUkOwnerUser.Contains(Attributes.Case.State));
            Assert.IsFalse(_caseNonUkOwnerUser.Contains(Attributes.Case.StatusReason));
            Assert.AreEqual(crmService.DefaultTeamNonUK.Id, ((EntityReference)contactNonUk11[Attributes.Contact.Owner]).Id);
            Assert.AreEqual(crmService.DefaultTeamNonUK.Id, ((EntityReference)accountNonUk12[Attributes.Account.Owner]).Id);
            Assert.AreEqual(crmService.DefaultTeamNonUK.Id, ((EntityReference)bookingNonUk1[Attributes.Booking.Owner]).Id);            
            Assert.AreEqual((int)CaseState.Active, ((OptionSetValue)_caseUk11[Attributes.Case.State]).Value);
            Assert.AreEqual((int)CaseStatusCode.AssignedToLocalSourceMarket, ((OptionSetValue)_caseUk11[Attributes.Case.StatusReason]).Value);            
            Assert.AreEqual((int)CaseState.Active, ((OptionSetValue)_caseUk12[Attributes.Case.State]).Value);
            Assert.AreEqual((int)CaseStatusCode.AssignedToLocalSourceMarket, ((OptionSetValue)_caseUk12[Attributes.Case.StatusReason]).Value);            
            Assert.AreEqual((int)CaseState.Active, ((OptionSetValue)_caseUk13[Attributes.Case.State]).Value);
            Assert.AreEqual((int)CaseStatusCode.AssignedToLocalSourceMarket, ((OptionSetValue)_caseUk13[Attributes.Case.StatusReason]).Value);
            Assert.AreEqual(crmService.DefaultTeamNonUK.Id, ((EntityReference)contactNonUk21[Attributes.Contact.Owner]).Id);
            Assert.AreEqual(crmService.DefaultTeamNonUK.Id, ((EntityReference)accountNonUk22[Attributes.Account.Owner]).Id);
            Assert.AreEqual(crmService.DefaultTeamNonUK.Id, ((EntityReference)accountNonUk23[Attributes.Account.Owner]).Id);
            Assert.AreEqual(crmService.DefaultTeamNonUK.Id, ((EntityReference)bookingNonUk2[Attributes.Booking.Owner]).Id);
            Assert.AreNotEqual(crmService.DefaultTeamNonUK.Id, ((EntityReference)_caseUk21[Attributes.Case.Owner]).Id);
            Assert.AreEqual((int)CaseState.Active, ((OptionSetValue)_caseUk21[Attributes.Case.State]).Value);
            Assert.AreEqual((int)CaseStatusCode.AssignedToLocalSourceMarket, ((OptionSetValue)_caseUk21[Attributes.Case.StatusReason]).Value);
            Assert.AreNotEqual(crmService.DefaultTeamNonUK.Id, ((EntityReference)_caseUk21[Attributes.Case.Owner]).Id);            
        }

        [TestMethod()]
        public void TestBigAmountOfBookings()
        {
            crmService.PrepareData();
            // setup
            List<Entity> bookings = new List<Entity>();
            List<Entity> contacts = new List<Entity>();
            List<Entity> cases = new List<Entity>();
            var team = crmService.AddTeam(CreateReference(EntityName.BusinessUnit, Attributes.Team.BusinessUnitId, crmService.BusinessUnitNonUK.Id), true);
            for (int i = 0; i < 20000; i++)
            {
                var contact1 = crmService.AddContact(CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id));
                var contact2 = crmService.AddContact(CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id));
                var contact3 = crmService.AddContact(CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id));

                var booking = crmService.AddBooking(DateTime.Now.AddDays(-2),
                    CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id),
                    CreateReference(EntityName.Gateway, Attributes.Booking.DestinationGatewayId, crmService.ValidGatewayIds[0]),
                    CreateReference(EntityName.Country, Attributes.Booking.SourceMarketId, crmService.SourceMarketCountryNonUK.Id));
                crmService.AddCustomerBookingRole(
                    CreateReference(EntityName.Booking, Attributes.CustomerBookingRole.BookingId, booking.Id),
                    CreateReference(EntityName.Contact, Attributes.CustomerBookingRole.Customer, contact1.Id));
                crmService.AddCustomerBookingRole(
                    CreateReference(EntityName.Booking, Attributes.CustomerBookingRole.BookingId, booking.Id),
                    CreateReference(EntityName.Contact, Attributes.CustomerBookingRole.Customer, contact2.Id));
                crmService.AddCustomerBookingRole(
                    CreateReference(EntityName.Booking, Attributes.CustomerBookingRole.BookingId, booking.Id),
                    CreateReference(EntityName.Contact, Attributes.CustomerBookingRole.Customer, contact3.Id));
                var _case1 = crmService.AddCase(
                    CreateReference(EntityName.Contact, Attributes.Case.CustomerId, contact1.Id),
                    CreateReference(EntityName.Team, Attributes.Booking.OwningTeam, team.Id)
                    );
                var _case2 = crmService.AddCase(
                    CreateReference(EntityName.Contact, Attributes.Case.CustomerId, contact2.Id),
                    CreateReference(EntityName.Team, Attributes.Booking.OwningTeam, team.Id)
                    );
                bookings.Add(booking);
                contacts.Add(contact1);
                contacts.Add(contact2);
                contacts.Add(contact3);
                cases.Add(_case1);
                cases.Add(_case2);
            }

            TestContext.WriteLine(string.Format("Starting execution: {0}", DateTime.Now));
            using (var service = new DeallocateBookingService(logger, deallocationService, configurationService))
            {
                service.Run();
            }
            TestContext.WriteLine(string.Format("Finished execution: {0}", DateTime.Now));

            // asserts
            foreach (var booking in bookings)
            {
                Assert.AreEqual(crmService.DefaultTeamNonUK.Id, ((EntityReference)booking[Attributes.Booking.Owner]).Id);
            }
            foreach (var contact in contacts)
            {
                Assert.AreEqual(crmService.DefaultTeamNonUK.Id, ((EntityReference)contact[Attributes.Contact.Owner]).Id);
            }
            foreach (var _case in cases)
            {
                Assert.AreNotEqual(crmService.DefaultTeamNonUK.Id, ((EntityReference)_case[Attributes.Case.Owner]).Id);
                // TODO: validate case status
                //Assert.Fail();
            }
        }

        private static EntityReference CreateReference(string entity, string key, Guid id)
        {
            return new EntityReference(entity, key, id) { Id = id };
        }
    }
}