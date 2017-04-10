using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tc.Crm.WebJob.DeallocateResortTeam.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.WebJob.DeallocateResortTeamTests;
using Tc.Crm.Common.Services;
using Microsoft.Xrm.Sdk;
using Tc.Crm.Common.Constants;
using Attributes = Tc.Crm.Common.Constants.Attributes;

namespace Tc.Crm.WebJob.DeallocateResortTeam.Services.Tests
{
    [TestClass()]
    public class DeallocateBookingServiceTests
    {
        ILogger logger;
        IConfigurationService configurationService;
        IDeallocationService deallocationService;
        TestCrmService crmService;


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
        /// Expected Result: Booking.Owner, Contact.Owner, Case.Owner not changed  
        /// </summary>
        [TestMethod()]
        public void TestDepartureDateSoonerThanToBeProcessed()
        {
            crmService.Switch = DataSwitch.Returns_Data;
            crmService.PrepareData();
            var context = crmService.context;

            // setup
            var team = crmService.AddTeam(CreateReference(EntityName.BusinessUnit, Attributes.Team.BusinessUnitId, crmService.BusinessUnit1.Id), true);
            var contact = crmService.AddContact(CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id));
            var booking = crmService.AddBooking(DateTime.Now.AddDays(-3),
                CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id),
                CreateReference(EntityName.Gateway, Attributes.Booking.DestinationGatewayId, crmService.ValidGatewayIds[0]),
                CreateReference(EntityName.Country, Attributes.Booking.SourceMarketId, crmService.SourceMarketCountry1.Id));            
            crmService.AddCustomerBookingRole(
                CreateReference(EntityName.Booking, Attributes.CustomerBookingRole.BookingId, booking.Id),
                CreateReference(EntityName.Contact, Attributes.CustomerBookingRole.Customer, contact.Id));
            var _case = crmService.AddCase(
                CreateReference(EntityName.Contact, Attributes.Case.CustomerId, contact.Id),
                CreateReference(EntityName.Team, Attributes.Booking.OwningTeam, team.Id)
                );
            var service = new DeallocateBookingService(logger, deallocationService, configurationService);
            service.Run();

            //asserts
            Assert.AreEqual(team.Id, ((EntityReference)booking[Attributes.Booking.Owner]).Id);
            Assert.AreEqual(team.Id, ((EntityReference)contact[Attributes.Contact.Owner]).Id);
            Assert.AreEqual(team.Id, ((EntityReference)contact[Attributes.Contact.Owner]).Id);
        }

        /// <summary>
        /// Booking departure date is later then 2 days ago, gateway is valid, owner is hotel team
        /// Expected Result: Booking.Owner, Contact.Owner, Case.Owner not changed  
        /// </summary>
        [TestMethod()]
        public void TestDepartureDateLaterThanToBeProcessed()
        {
            crmService.Switch = DataSwitch.Returns_Data;
            crmService.PrepareData();
            var context = crmService.context;

            // setup
            var team = crmService.AddTeam(CreateReference(EntityName.BusinessUnit, Attributes.Team.BusinessUnitId, crmService.BusinessUnit1.Id), true);
            var contact = crmService.AddContact(CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id));
            var booking = crmService.AddBooking(DateTime.Now.AddDays(-1),
                CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id),
                CreateReference(EntityName.Gateway, Attributes.Booking.DestinationGatewayId, crmService.ValidGatewayIds[0]),
                CreateReference(EntityName.Country, Attributes.Booking.SourceMarketId, crmService.SourceMarketCountry1.Id));
            crmService.AddCustomerBookingRole(
                CreateReference(EntityName.Booking, Attributes.CustomerBookingRole.BookingId, booking.Id),
                CreateReference(EntityName.Contact, Attributes.CustomerBookingRole.Customer, contact.Id));
            var _case = crmService.AddCase(
                CreateReference(EntityName.Contact, Attributes.Case.CustomerId, contact.Id),
                CreateReference(EntityName.Team, Attributes.Booking.OwningTeam, team.Id)
                );
            var service = new DeallocateBookingService(logger, deallocationService, configurationService);
            service.Run();

            //asserts
            Assert.AreEqual(team.Id, ((EntityReference)booking[Attributes.Booking.Owner]).Id);
            Assert.AreEqual(team.Id, ((EntityReference)contact[Attributes.Contact.Owner]).Id);
            Assert.AreEqual(team.Id, ((EntityReference)contact[Attributes.Contact.Owner]).Id);
        }

        /// <summary>
        /// Booking departure date was 2 days ago, gateway is valid, owner of booking is not hotel team
        /// Expected Result: Booking.Owner, Contact.Owner, Case.Owner not changed  
        /// </summary>
        [TestMethod()]
        public void TestOwnerNotHotelTeam()
        {
            crmService.Switch = DataSwitch.Returns_Data;
            crmService.PrepareData();
            var context = crmService.context;

            // setup
            var team = crmService.AddTeam(CreateReference(EntityName.BusinessUnit, Attributes.Team.BusinessUnitId, crmService.BusinessUnit1.Id), false);
            var contact = crmService.AddContact(CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id));
            var booking = crmService.AddBooking(DateTime.Now.AddDays(-2),
                CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id),
                CreateReference(EntityName.Gateway, Attributes.Booking.DestinationGatewayId, crmService.ValidGatewayIds[0]),
                CreateReference(EntityName.Country, Attributes.Booking.SourceMarketId, crmService.SourceMarketCountry1.Id));
            crmService.AddCustomerBookingRole(
                CreateReference(EntityName.Booking, Attributes.CustomerBookingRole.BookingId, booking.Id),
                CreateReference(EntityName.Contact, Attributes.CustomerBookingRole.Customer, contact.Id));
            var _case = crmService.AddCase(
                CreateReference(EntityName.Contact, Attributes.Case.CustomerId, contact.Id),
                CreateReference(EntityName.Team, Attributes.Booking.OwningTeam, team.Id)
                );
            var service = new DeallocateBookingService(logger, deallocationService, configurationService);
            service.Run();

            //asserts
            Assert.AreEqual(team.Id, ((EntityReference)booking[Attributes.Booking.Owner]).Id);
            Assert.AreEqual(team.Id, ((EntityReference)contact[Attributes.Contact.Owner]).Id);
            Assert.AreEqual(team.Id, ((EntityReference)contact[Attributes.Contact.Owner]).Id);
        }

        /// <summary>
        /// Booking departure date was 2 days ago, owner of booking is hotel team, gateway is not valid
        /// Expected Result: Booking.Owner, Contact.Owner, Case.Owner not changed  
        /// </summary>
        [TestMethod()]
        public void TestGatewayNotValid()
        {
            crmService.Switch = DataSwitch.Returns_Data;
            crmService.PrepareData();
            var context = crmService.context;

            // setup
            var team = crmService.AddTeam(CreateReference(EntityName.BusinessUnit, Attributes.Team.BusinessUnitId, crmService.BusinessUnit1.Id), true);
            var contact = crmService.AddContact(CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id));
            var gateway = crmService.AddGateway();
            var booking = crmService.AddBooking(DateTime.Now.AddDays(-2),
                CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id),
                CreateReference(EntityName.Gateway, Attributes.Booking.DestinationGatewayId, gateway.Id),
                CreateReference(EntityName.Country, Attributes.Booking.SourceMarketId, crmService.SourceMarketCountry1.Id));
            crmService.AddCustomerBookingRole(
                CreateReference(EntityName.Booking, Attributes.CustomerBookingRole.BookingId, booking.Id),
                CreateReference(EntityName.Contact, Attributes.CustomerBookingRole.Customer, contact.Id));
            var _case = crmService.AddCase(
                CreateReference(EntityName.Contact, Attributes.Case.CustomerId, contact.Id),
                CreateReference(EntityName.Team, Attributes.Booking.OwningTeam, team.Id)
                );
            var service = new DeallocateBookingService(logger, deallocationService, configurationService);
            service.Run();

            //asserts
            Assert.AreEqual(team.Id, ((EntityReference)booking[Attributes.Booking.Owner]).Id);
            Assert.AreEqual(team.Id, ((EntityReference)contact[Attributes.Contact.Owner]).Id);
            Assert.AreEqual(team.Id, ((EntityReference)contact[Attributes.Contact.Owner]).Id);
        }

        /// <summary>
        /// Booking departure date was 2 days ago, gateway is valid, owner of booking is hotel team
        /// Expected Result: Booking.Owner, Contact.Owner, Case.Owner changed to default team
        /// </summary>
        [TestMethod()]
        public void TestValidBooking()
        {
            crmService.Switch = DataSwitch.Returns_Data;
            crmService.PrepareData();
            var context = crmService.context;

            // setup
            var team = crmService.AddTeam(CreateReference(EntityName.BusinessUnit, Attributes.Team.BusinessUnitId, crmService.BusinessUnit1.Id), true);
            var contact = crmService.AddContact(CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id));
            var booking = crmService.AddBooking(DateTime.Now.AddDays(-2),
                CreateReference(EntityName.Team, Attributes.Team.TeamId, team.Id),
                CreateReference(EntityName.Gateway, Attributes.Booking.DestinationGatewayId, crmService.ValidGatewayIds[0]),
                CreateReference(EntityName.Country, Attributes.Booking.SourceMarketId, crmService.SourceMarketCountry1.Id));
            crmService.AddCustomerBookingRole(
                CreateReference(EntityName.Booking, Attributes.CustomerBookingRole.BookingId, booking.Id),
                CreateReference(EntityName.Contact, Attributes.CustomerBookingRole.Customer, contact.Id));
            var _case = crmService.AddCase(
                CreateReference(EntityName.Contact, Attributes.Case.CustomerId, contact.Id),
                CreateReference(EntityName.Team, Attributes.Booking.OwningTeam, team.Id)
                );
            var service = new DeallocateBookingService(logger, deallocationService, configurationService);
            service.Run();

            //asserts
            Assert.AreEqual(crmService.DefaultTeam1.Id, ((EntityReference)booking[Attributes.Booking.Owner]).Id);
            Assert.AreEqual(crmService.DefaultTeam1.Id, ((EntityReference)contact[Attributes.Contact.Owner]).Id);
            Assert.AreEqual(crmService.DefaultTeam1.Id, ((EntityReference)contact[Attributes.Contact.Owner]).Id);

        }

        /// <summary>
        /// Booking departure date was 2 days ago, gateway is valid, owner of booking is hotel team, source market is UK, customer is contact
        /// Expected Result: Booking.Owner, Contact.Owner, Case.Owner changed to default team
        /// </summary>
        //[TestMethod()]
        public void TestValidBookingUkMarketContactCustomer()
        {
            Assert.Fail();
        }

        /// <summary>
        /// Booking departure date was 2 days ago, gateway is valid, owner of booking is hotel team, , source market is UK, customer is account
        /// Expected Result: Booking.Owner, Contact.Owner, Case.Owner changed to default team
        /// </summary>
        //[TestMethod()]
        public void TestValidBookingNonUkMarketContactCustomer()
        {
            Assert.Fail();
        }

        /// <summary>
        /// Booking departure date was 2 days ago, gateway is valid, owner of booking is hotel team
        /// Expected Result: Booking.Owner, Contact.Owner, Case.Owner changed to default team
        /// </summary>
        //[TestMethod()]
        public void TestValidBookingUkMarketAccountCustomer()
        {
            Assert.Fail();
        }

        /// <summary>
        /// Booking departure date was 2 days ago, gateway is valid, owner of booking is hotel team, , source market is UK, customer is account
        /// Expected Result: Booking.Owner, Contact.Owner, Case.Owner changed to default team
        /// </summary>
        //[TestMethod()]
        public void TestValidBookingNonUkAccountCustomer()
        {
            Assert.Fail();
        }

        /// <summary>
        /// Booking departure date was 2 days ago, gateway is valid, owner of booking is hotel team
        /// Expected Result: Booking.Owner, Contact.Owner, Case.Owner changed to default team
        /// </summary>
        //[TestMethod()]
        public void TestValidBookingMultipleSourceMarket()
        {
            Assert.Fail();
        }

        private EntityReference CreateReference(string entity, string key, Guid id)
        {
            return new EntityReference(entity, key, id) { Id = id };
        }
    }
}