using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Tc.Crm.Common.Services;
using Tc.Crm.WebJob.AllocateResortTeamTests;
using Microsoft.Xrm.Sdk;

namespace Tc.Crm.WebJob.AllocateResortTeam.Services.Tests
{
    [TestClass()]
    public class AllocateResortTeamServiceTests
    {
        ILogger logger;
        IConfigurationService configurationService;
        IAllocationService allocationService;
        TestCrmService crmService;


        [TestInitialize()]
        public void Setpup()
        {
            this.logger = new TestLogger();
            this.configurationService = new TestConfigurationService();
            this.crmService = new TestCrmService();
            this.allocationService = new AllocationService(this.logger, this.crmService);

        }

        /// <summary>
        /// No booking records were returned from service layer
        /// Expected Result: No records will be updated or processed. A null is returned
        /// No exceptions will be thrown.
        /// </summary>
        [TestMethod()]
        public void RunTest_EntityCollectionIsNulld()
        {
            crmService.Switch = DataSwitch.No_Records_Returned;
            var service = new AllocateResortTeamService(logger, allocationService, configurationService);
            service.Run();
            Assert.IsTrue(true);
        }

        /// <summary>
        /// No booking records were returned from service layer
        /// Expected Result: No records will be updated or processed. An empty EntityCollection instance is returned.
        /// No exceptions will be thrown.
        /// </summary>
        [TestMethod()]
        public void RunTest_EntityCollectionIsEmpty()
        {
            crmService.Switch = DataSwitch.Collection_With_No_Records_Returned;
            var service = new AllocateResortTeamService(logger, allocationService, configurationService);
            service.Run();
            Assert.IsTrue(true);
        }

        /// <summary>
        /// Booking departure date is exactly 14 days from now
        /// Expected Result: Booking.Owner = Hotel.Owner, Customer.Owner = Hotel.Owner
        /// </summary>
        [TestMethod()]
        public void RunTest_BookingDepartureDateIs14DaysFromNow()
        {
            crmService.Switch = DataSwitch.Returns_Data;
            crmService.PrepareData();
            var context = crmService.context;

            //setup 
            var booking = context.Data["tc_booking"].Values.ToList<Entity>()[0];
            var gateway = context.Data["tc_gateway"].Values.ToList<Entity>()[0];
            var user = context.Data["systemuser"].Values.ToList<Entity>()[0];
            var team = context.Data["team"].Values.ToList<Entity>()[0];
            var hotel = context.Data["tc_hotel"].Values.ToList<Entity>()[0];
            var customer = context.Data["contact"].Values.ToList<Entity>()[0];
            var customerBookingRole = context.Data["tc_customerbookingrole"].Values.ToList<Entity>()[0];
            var accommodation = context.Data["tc_bookingaccommodation"].Values.ToList<Entity>()[0];
            var country = context.Data["tc_country"].Values.ToList<Entity>()[0];
            var childTeam = context.Data["team"].Values.ToList<Entity>()[1];

            //Booking start date 14 days from now
            crmService.SetBooking(booking,14,11, gateway.Id, new EntityReference("systemuser", user.Id));
            hotel["ownerid"] = new EntityReference("team", team.Id);
            //accommodation start date 14 days from now
            crmService.SetAccommodation(accommodation,14, 11, booking.Id, hotel.Id);
            customer["ownerid"] = new EntityReference("systemuser", user.Id);
            crmService.SetCustomerBookingRole(customerBookingRole
                                              , new EntityReference("contact", customer.Id)
                                              , new EntityReference("tc_booking", booking.Id));
            crmService.SetSourceMarket(customer, country.Id);
            childTeam["businessunitid"] = new EntityReference("businessunit", ((EntityReference)country["tc_sourcemarketbusinessunitid"]).Id);

            //expected value
            var expectedBookingOwnerId = childTeam.Id;
            var expectedCusomerOwnerId = childTeam.Id;


            var service = new AllocateResortTeamService(logger, allocationService, configurationService);
            service.Run();

            //asserts
            Assert.AreEqual(expectedBookingOwnerId, ((EntityReference)booking["ownerid"]).Id);
            Assert.AreEqual(expectedCusomerOwnerId, ((EntityReference)customer["ownerid"]).Id);           
        }

        /// <summary>
        /// Booking departure date is exactly 10 days from now
        /// Expected Result: Booking will not be processed
        /// Booking.Owner = User, Customer.Owner = User
        /// </summary>
        [TestMethod()]
        public void RunTest_BookingDepartureDateIs10DaysFromNow()
        {
            crmService.Switch = DataSwitch.Returns_Data;
            crmService.PrepareData();
            var context = crmService.context;

            //setup 
            var booking = context.Data["tc_booking"].Values.ToList<Entity>()[0];
            var gateway = context.Data["tc_gateway"].Values.ToList<Entity>()[0];
            var user = context.Data["systemuser"].Values.ToList<Entity>()[0];
            var team = context.Data["team"].Values.ToList<Entity>()[0];
            var hotel = context.Data["tc_hotel"].Values.ToList<Entity>()[0];
            var customer = context.Data["contact"].Values.ToList<Entity>()[0];
            var customerBookingRole = context.Data["tc_customerbookingrole"].Values.ToList<Entity>()[0];
            var accommodation = context.Data["tc_bookingaccommodation"].Values.ToList<Entity>()[0];
            var country = context.Data["tc_country"].Values.ToList<Entity>()[0];
            var childTeam = context.Data["team"].Values.ToList<Entity>()[1];

            //Booking start date 10 days from now
            crmService.SetBooking(booking, 15, 17, gateway.Id, new EntityReference("systemuser", user.Id));
            booking["ownerid"] = new EntityReference("systemuser", user.Id);
            hotel["ownerid"] = new EntityReference("team", team.Id);
            //accommodation start date 10 days from now
            crmService.SetAccommodation(accommodation, 15, 17, booking.Id, hotel.Id);
            customer["ownerid"] = new EntityReference("systemuser", user.Id);
            crmService.SetCustomerBookingRole(customerBookingRole
                                              , new EntityReference("contact", customer.Id)
                                              , new EntityReference("tc_booking", booking.Id));
            crmService.SetSourceMarket(customer, country.Id);
            childTeam["businessunitid"] = new EntityReference("businessunit", ((EntityReference)country["tc_sourcemarketbusinessunitid"]).Id);

            //expected value
            var expectedBookingOwnerId = user.Id;
            var expectedCustomerOwnerId = user.Id;

            var service = new AllocateResortTeamService(logger, allocationService, configurationService);
            service.Run();

            //asserts
            Assert.AreEqual(expectedBookingOwnerId, ((EntityReference)booking["ownerid"]).Id);
            Assert.AreEqual(expectedCustomerOwnerId, ((EntityReference)customer["ownerid"]).Id);
        }

        /// <summary>
        /// Booking departure date is exactly 14 days from now
        /// accomodation start date is 10 days from now
        /// Expected Result: Booking.Owner = Hotel.Owner, Customer.Owner = Hotel.Owner
        /// </summary>
        [TestMethod()]
        public void RunTest_BookingDepartsOnOrBefore14DaysButAccomodationsStartsAfter14Days()
        {
            crmService.Switch = DataSwitch.Returns_Data;
            crmService.PrepareData();
            var context = crmService.context;

            //setup 
            var booking = context.Data["tc_booking"].Values.ToList<Entity>()[0];
            var gateway = context.Data["tc_gateway"].Values.ToList<Entity>()[0];
            var user = context.Data["systemuser"].Values.ToList<Entity>()[0];
            var team = context.Data["team"].Values.ToList<Entity>()[0];
            var hotel = context.Data["tc_hotel"].Values.ToList<Entity>()[0];
            var customer = context.Data["contact"].Values.ToList<Entity>()[0];
            var customerBookingRole = context.Data["tc_customerbookingrole"].Values.ToList<Entity>()[0];
            var accommodation = context.Data["tc_bookingaccommodation"].Values.ToList<Entity>()[0];
            var country = context.Data["tc_country"].Values.ToList<Entity>()[0];
            var childTeam = context.Data["team"].Values.ToList<Entity>()[1];

            //Booking start date 14 days from now
            crmService.SetBooking(booking, 14, 17, gateway.Id, new EntityReference("systemuser", user.Id));
            hotel["ownerid"] = new EntityReference("team", team.Id);
            //accommodation start date 15 days from now
            crmService.SetAccommodation(accommodation, 15, 17, booking.Id, hotel.Id);
            customer["ownerid"] = new EntityReference("systemuser", user.Id);
            crmService.SetCustomerBookingRole(customerBookingRole
                                              , new EntityReference("contact", customer.Id)
                                              , new EntityReference("tc_booking", booking.Id));
            crmService.SetSourceMarket(customer, country.Id);
            childTeam["businessunitid"] = new EntityReference("businessunit", ((EntityReference)country["tc_sourcemarketbusinessunitid"]).Id);

            //expected value
            var expectedBookingOwnerId = childTeam.Id;
            var expectedCusomerOwnerId = childTeam.Id;


            var service = new AllocateResortTeamService(logger, allocationService, configurationService);
            service.Run();

            //asserts
            Assert.AreEqual(expectedBookingOwnerId, ((EntityReference)booking["ownerid"]).Id);
            Assert.AreEqual(expectedCusomerOwnerId, ((EntityReference)customer["ownerid"]).Id);
           
        }

        /// <summary>
        /// Booking departure date is exactly 10 days from now
        /// accomodation start date is 14 days from now
        /// Expected Result: Booking will not be processed
        /// Booking.Owner = User, Customer.Owner = User
        /// </summary>
        [TestMethod()]
        public void RunTest_BookingStartsAfter14DaysButAccomodationStartsOnOrBefore14Days()
        {
            crmService.Switch = DataSwitch.Returns_Data;
            crmService.PrepareData();
            var context = crmService.context;

            //setup 
            var booking = context.Data["tc_booking"].Values.ToList<Entity>()[0];
            var gateway = context.Data["tc_gateway"].Values.ToList<Entity>()[0];
            var user = context.Data["systemuser"].Values.ToList<Entity>()[0];
            var team = context.Data["team"].Values.ToList<Entity>()[0];
            var hotel = context.Data["tc_hotel"].Values.ToList<Entity>()[0];
            var customer = context.Data["contact"].Values.ToList<Entity>()[0];
            var customerBookingRole = context.Data["tc_customerbookingrole"].Values.ToList<Entity>()[0];
            var accommodation = context.Data["tc_bookingaccommodation"].Values.ToList<Entity>()[0];
            var country = context.Data["tc_country"].Values.ToList<Entity>()[0];
            var childTeam = context.Data["team"].Values.ToList<Entity>()[1];

            //Booking start date 15 days from now
            crmService.SetBooking(booking, 15, 17, gateway.Id, new EntityReference("systemuser", user.Id));
            hotel["ownerid"] = new EntityReference("team", team.Id);
            //accommodation start date 14 days from now
            crmService.SetAccommodation(accommodation, 16, 17, booking.Id, hotel.Id);
            customer["ownerid"] = new EntityReference("systemuser", user.Id);
            crmService.SetCustomerBookingRole(customerBookingRole
                                              , new EntityReference("contact", customer.Id)
                                              , new EntityReference("tc_booking", booking.Id));
            crmService.SetSourceMarket(customer, country.Id);
            childTeam["businessunitid"] = new EntityReference("businessunit", ((EntityReference)country["tc_sourcemarketbusinessunitid"]).Id);


            //expected value
            var expectedBookingOwnerId = user.Id;
            var expectedCusomerOwnerId = user.Id;

            var service = new AllocateResortTeamService(logger, allocationService, configurationService);
            service.Run();

            //asserts
            Assert.AreEqual(expectedBookingOwnerId, ((EntityReference)booking["ownerid"]).Id);
            Assert.AreEqual(expectedCusomerOwnerId, ((EntityReference)customer["ownerid"]).Id);
        }

        /// <summary>
        /// Booking departure date is is current date
        /// accomodation start date is current date
        /// Expected Result: Booking.Owner = Hotel.Owner, Customer.Owner = Hotel.Owner
        /// </summary>
        [TestMethod()]
        public void RunTest_BookingAndAccomodationStartsToday()
        {
            crmService.Switch = DataSwitch.Returns_Data;
            crmService.PrepareData();
            var context = crmService.context;

            //setup 
            var booking = context.Data["tc_booking"].Values.ToList<Entity>()[0];
            var gateway = context.Data["tc_gateway"].Values.ToList<Entity>()[0];
            var user = context.Data["systemuser"].Values.ToList<Entity>()[0];
            var team = context.Data["team"].Values.ToList<Entity>()[0];
            var hotel = context.Data["tc_hotel"].Values.ToList<Entity>()[0];
            var customer = context.Data["contact"].Values.ToList<Entity>()[0];
            var customerBookingRole = context.Data["tc_customerbookingrole"].Values.ToList<Entity>()[0];
            var accommodation = context.Data["tc_bookingaccommodation"].Values.ToList<Entity>()[0];
            var country = context.Data["tc_country"].Values.ToList<Entity>()[0];
            var childTeam = context.Data["team"].Values.ToList<Entity>()[1];

            //Booking start date current day
            crmService.SetBooking(booking, 0, 11, gateway.Id, new EntityReference("systemuser", user.Id));
            hotel["ownerid"] = new EntityReference("team", team.Id);
            //accommodation start date is current day
            crmService.SetAccommodation(accommodation, 0, 11, booking.Id, hotel.Id);
            customer["ownerid"] = new EntityReference("systemuser", user.Id);
            crmService.SetCustomerBookingRole(customerBookingRole
                                              , new EntityReference("contact", customer.Id)
                                              , new EntityReference("tc_booking", booking.Id));

            crmService.SetSourceMarket(customer, country.Id);
            childTeam["businessunitid"] = new EntityReference("businessunit", ((EntityReference)country["tc_sourcemarketbusinessunitid"]).Id);

            //expected value
            var expectedBookingOwnerId = childTeam.Id;
            var expectedCusomerOwnerId = childTeam.Id;


            var service = new AllocateResortTeamService(logger, allocationService, configurationService);
            service.Run();

            //asserts
            Assert.AreEqual(expectedBookingOwnerId, ((EntityReference)booking["ownerid"]).Id);
            Assert.AreEqual(expectedCusomerOwnerId, ((EntityReference)customer["ownerid"]).Id);
        }

        /// <summary>
        /// Booking departure date is is next day
        /// accomodation start date is next day
        /// Expected Result: Booking.Owner = Hotel.Owner, Customer.Owner = Hotel.Owner
        /// </summary>
        [TestMethod()]
        public void RunTest_BookingAndAccomodationStartsTomorrow()
        {
            crmService.Switch = DataSwitch.Returns_Data;
            crmService.PrepareData();
            var context = crmService.context;

            //setup 
            var booking = context.Data["tc_booking"].Values.ToList<Entity>()[0];
            var gateway = context.Data["tc_gateway"].Values.ToList<Entity>()[0];
            var user = context.Data["systemuser"].Values.ToList<Entity>()[0];
            var team = context.Data["team"].Values.ToList<Entity>()[0];
            var hotel = context.Data["tc_hotel"].Values.ToList<Entity>()[0];
            var customer = context.Data["contact"].Values.ToList<Entity>()[0];
            var customerBookingRole = context.Data["tc_customerbookingrole"].Values.ToList<Entity>()[0];
            var accommodation = context.Data["tc_bookingaccommodation"].Values.ToList<Entity>()[0];
            var country = context.Data["tc_country"].Values.ToList<Entity>()[0];
            var childTeam = context.Data["team"].Values.ToList<Entity>()[1];

            //Booking start date 1 days from now
            crmService.SetBooking(booking, 1, 11, gateway.Id, new EntityReference("systemuser", user.Id));
            hotel["ownerid"] = new EntityReference("team", team.Id);
            //accommodation start date 1 days from now
            crmService.SetAccommodation(accommodation, 1, 11, booking.Id, hotel.Id);
            customer["ownerid"] = new EntityReference("systemuser", user.Id);
            crmService.SetCustomerBookingRole(customerBookingRole
                                              , new EntityReference("contact", customer.Id)
                                              , new EntityReference("tc_booking", booking.Id));

            crmService.SetSourceMarket(customer, country.Id);
            childTeam["businessunitid"] = new EntityReference("businessunit", ((EntityReference)country["tc_sourcemarketbusinessunitid"]).Id);

            //expected value
            var expectedBookingOwnerId = childTeam.Id;
            var expectedCusomerOwnerId = childTeam.Id;


            var service = new AllocateResortTeamService(logger, allocationService, configurationService);
            service.Run();

            //asserts
            Assert.AreEqual(expectedBookingOwnerId, ((EntityReference)booking["ownerid"]).Id);
            Assert.AreEqual(expectedCusomerOwnerId, ((EntityReference)customer["ownerid"]).Id);
        }

        /// <summary>
        /// Booking departure date is previous day
        /// accomodation start date is previous day
        /// Expected Result: Booking will not be processed
        /// Booking.Owner = User, Customer.Owner = User
        /// </summary>
        [TestMethod()]
        public void RunTest_BookingStartsYesterdayAndAccomodationStartsYesterday()
        {
            crmService.Switch = DataSwitch.Returns_Data;
            crmService.PrepareData();
            var context = crmService.context;

            //setup 
            var booking = context.Data["tc_booking"].Values.ToList<Entity>()[0];
            var gateway = context.Data["tc_gateway"].Values.ToList<Entity>()[0];
            var user = context.Data["systemuser"].Values.ToList<Entity>()[0];
            var team = context.Data["team"].Values.ToList<Entity>()[0];
            var hotel = context.Data["tc_hotel"].Values.ToList<Entity>()[0];
            var customer = context.Data["contact"].Values.ToList<Entity>()[0];
            var customerBookingRole = context.Data["tc_customerbookingrole"].Values.ToList<Entity>()[0];
            var accommodation = context.Data["tc_bookingaccommodation"].Values.ToList<Entity>()[0];
            var country = context.Data["tc_country"].Values.ToList<Entity>()[0];
            var childTeam = context.Data["team"].Values.ToList<Entity>()[1];

            //Booking start date is previous day
            crmService.SetBooking(booking, -1, 11, gateway.Id, new EntityReference("systemuser", user.Id));
            hotel["ownerid"] = new EntityReference("team", team.Id);
            //accommodation start date is previous day
            crmService.SetAccommodation(accommodation, -1, 11, booking.Id, hotel.Id);
            customer["ownerid"] = new EntityReference("systemuser", user.Id);
            booking["ownerid"] = new EntityReference("systemuser", user.Id);
            crmService.SetCustomerBookingRole(customerBookingRole
                                              , new EntityReference("contact", customer.Id)
                                              , new EntityReference("tc_booking", booking.Id));


            crmService.SetSourceMarket(customer, country.Id);
            childTeam["businessunitid"] = new EntityReference("businessunit", ((EntityReference)country["tc_sourcemarketbusinessunitid"]).Id);

            //expected value
            var expectedBookingOwnerId = user.Id;
            var expectedCusomerOwnerId = user.Id;

            var service = new AllocateResortTeamService(logger, allocationService, configurationService);
            service.Run();

            //asserts
            Assert.AreEqual(expectedBookingOwnerId, ((EntityReference)booking["ownerid"]).Id);
            Assert.AreEqual(expectedCusomerOwnerId, ((EntityReference)customer["ownerid"]).Id);
        }

        /// <summary>
        /// Booking departure date is previous day
        /// accomodation start date is current day
        /// Expected Result: Booking.Owner = Hotel.Owner, Customer.Owner = Hotel.Owner
        /// </summary>
        [TestMethod()]
        public void RunTest_BookingStartsYesterdayButAccomodationStartsToday()
        {
            crmService.Switch = DataSwitch.Returns_Data;
            crmService.PrepareData();
            var context = crmService.context;

            //setup 
            var booking = context.Data["tc_booking"].Values.ToList<Entity>()[0];
            var gateway = context.Data["tc_gateway"].Values.ToList<Entity>()[0];
            var user = context.Data["systemuser"].Values.ToList<Entity>()[0];
            var team = context.Data["team"].Values.ToList<Entity>()[0];
            var hotel = context.Data["tc_hotel"].Values.ToList<Entity>()[0];
            var customer = context.Data["contact"].Values.ToList<Entity>()[0];
            var customerBookingRole = context.Data["tc_customerbookingrole"].Values.ToList<Entity>()[0];
            var accommodation = context.Data["tc_bookingaccommodation"].Values.ToList<Entity>()[0];
            var country = context.Data["tc_country"].Values.ToList<Entity>()[0];
            var childTeam = context.Data["team"].Values.ToList<Entity>()[1];

            //Booking start date is previous day
            crmService.SetBooking(booking, -1, 11, gateway.Id, new EntityReference("systemuser", user.Id));
            hotel["ownerid"] = new EntityReference("team", team.Id);
            //accommodation start date is today
            crmService.SetAccommodation(accommodation, 0, 11, booking.Id, hotel.Id);
            customer["ownerid"] = new EntityReference("systemuser", user.Id);
            crmService.SetCustomerBookingRole(customerBookingRole
                                              , new EntityReference("contact", customer.Id)
                                              , new EntityReference("tc_booking", booking.Id));

            crmService.SetSourceMarket(customer, country.Id);
            childTeam["businessunitid"] = new EntityReference("businessunit", ((EntityReference)country["tc_sourcemarketbusinessunitid"]).Id);

            //expected value
            var expectedBookingOwnerId = childTeam.Id;
            var expectedCusomerOwnerId = childTeam.Id;

            var service = new AllocateResortTeamService(logger, allocationService, configurationService);
            service.Run();

            //asserts
            Assert.AreEqual(expectedBookingOwnerId, ((EntityReference)booking["ownerid"]).Id);
            Assert.AreEqual(expectedCusomerOwnerId, ((EntityReference)customer["ownerid"]).Id);
        }


        /// <summary>
        /// Booking has multiple accommodations. First 1 yet to start
        /// Expected Result: Booking.Owner = FirstAccommodation.Hotel.Owner
        /// , Customer.Owner = FirstAccommodation.Hotel.Owner
        /// </summary>
        [TestMethod()]
        public void RunTest_BookingHasMultipleAccommodationsFirstOneYetToStart()
        {
            crmService.Switch = DataSwitch.Returns_Data;
            crmService.PrepareData();
            var context = crmService.context;

            //setup 
            crmService.AddAccommodation();
            crmService.AddHotel();
            var booking = context.Data["tc_booking"].Values.ToList<Entity>()[0];
            var gateway = context.Data["tc_gateway"].Values.ToList<Entity>()[0];
            var user = context.Data["systemuser"].Values.ToList<Entity>()[0];
            var team1 = context.Data["team"].Values.ToList<Entity>()[0];
            var team2 = context.Data["team"].Values.ToList<Entity>()[1];
            var hotel1 = context.Data["tc_hotel"].Values.ToList<Entity>()[0];
            var hotel2 = context.Data["tc_hotel"].Values.ToList<Entity>()[1];
            var customer = context.Data["contact"].Values.ToList<Entity>()[0];
            var customerBookingRole = context.Data["tc_customerbookingrole"].Values.ToList<Entity>()[0];
            var accommodation1 = context.Data["tc_bookingaccommodation"].Values.ToList<Entity>()[0];
            var accommodation2 = context.Data["tc_bookingaccommodation"].Values.ToList<Entity>()[1];
            var country = context.Data["tc_country"].Values.ToList<Entity>()[0];
            var childTeam = context.Data["team"].Values.ToList<Entity>()[1];

            //Booking start date is previous day
            crmService.SetBooking(booking, 0, 4, gateway.Id, new EntityReference("systemuser", user.Id));
            hotel1["ownerid"] = new EntityReference("team", team1.Id);
            hotel2["ownerid"] = new EntityReference("team", team2.Id);
            //accommodation start date is today
            crmService.SetAccommodation(accommodation1, 0, 2, booking.Id, hotel1.Id);
            crmService.SetAccommodation(accommodation2, 2, 4, booking.Id, hotel2.Id);
            customer["ownerid"] = new EntityReference("systemuser", user.Id);
            crmService.SetCustomerBookingRole(customerBookingRole
                                              , new EntityReference("contact", customer.Id)
                                              , new EntityReference("tc_booking", booking.Id));


            crmService.SetSourceMarket(customer, country.Id);
            childTeam["businessunitid"] = new EntityReference("businessunit", ((EntityReference)country["tc_sourcemarketbusinessunitid"]).Id);

            //expected value
            var expectedBookingOwnerId = childTeam.Id;
            var expectedCusomerOwnerId = childTeam.Id;

            var service = new AllocateResortTeamService(logger, allocationService, configurationService);
            service.Run();

            //asserts
            Assert.AreEqual(expectedBookingOwnerId, ((EntityReference)booking["ownerid"]).Id);
            Assert.AreEqual(expectedCusomerOwnerId, ((EntityReference)customer["ownerid"]).Id);           
        }

        /// <summary>
        /// Booking has multiple accommodations. Second to start, First completed
        /// Expected Result: Booking.Owner = SecondAccommodation.Hotel.Owner
        /// , Customer.Owner = SecondAccommodation.Hotel.Owner
        /// </summary>
        [TestMethod()]
        public void RunTest_BookingHasMultipleAccommodationsSecondOneYetToStart()
        {
            crmService.Switch = DataSwitch.Returns_Data;
            crmService.PrepareData();
            var context = crmService.context;

            //setup 
            crmService.AddAccommodation();
            crmService.AddHotel();
            var booking = context.Data["tc_booking"].Values.ToList<Entity>()[0];
            var gateway = context.Data["tc_gateway"].Values.ToList<Entity>()[0];
            var user = context.Data["systemuser"].Values.ToList<Entity>()[0];
            var team1 = context.Data["team"].Values.ToList<Entity>()[1];
            var team2 = context.Data["team"].Values.ToList<Entity>()[0];
            var hotel1 = context.Data["tc_hotel"].Values.ToList<Entity>()[0];
            var hotel2 = context.Data["tc_hotel"].Values.ToList<Entity>()[1];
            var customer = context.Data["contact"].Values.ToList<Entity>()[0];
            var customerBookingRole = context.Data["tc_customerbookingrole"].Values.ToList<Entity>()[0];
            var accommodation1 = context.Data["tc_bookingaccommodation"].Values.ToList<Entity>()[0];
            var accommodation2 = context.Data["tc_bookingaccommodation"].Values.ToList<Entity>()[1];
            var country = context.Data["tc_country"].Values.ToList<Entity>()[0];
            var childTeam2 = context.Data["team"].Values.ToList<Entity>()[2];

            //Booking start date is previous day
            crmService.SetBooking(booking, -2, 4, gateway.Id, new EntityReference("systemuser", user.Id));
            hotel1["ownerid"] = new EntityReference("team", team1.Id);
            hotel2["ownerid"] = new EntityReference("team", team2.Id);
            //accommodation start date is today
            crmService.SetAccommodation(accommodation1, -2, -1, booking.Id, hotel1.Id);
            crmService.SetAccommodation(accommodation2, 0, 4, booking.Id, hotel2.Id);
            customer["ownerid"] = new EntityReference("systemuser", user.Id);
            crmService.SetCustomerBookingRole(customerBookingRole
                                              , new EntityReference("contact", customer.Id)
                                              , new EntityReference("tc_booking", booking.Id));

            crmService.SetSourceMarket(customer, country.Id);
            childTeam2["businessunitid"] = new EntityReference("businessunit", ((EntityReference)country["tc_sourcemarketbusinessunitid"]).Id);

            //expected value
            var expectedBookingOwnerId = childTeam2.Id;
            var expectedCusomerOwnerId = childTeam2.Id;

            var service = new AllocateResortTeamService(logger, allocationService, configurationService);
            service.Run();

            //asserts
            Assert.AreEqual(expectedBookingOwnerId, ((EntityReference)booking["ownerid"]).Id);
            Assert.AreEqual(expectedCusomerOwnerId, ((EntityReference)customer["ownerid"]).Id);
        }

        /// <summary>
        /// Booking has multiple customers.
        /// Expected Result: Booking.Owner = Hotel.Owner
        /// , AllCustomers.Owner = Hotel.Owner
        /// </summary>
        [TestMethod()]
        public void RunTest_BookingHasMultipleCustomers()
        {
            crmService.Switch = DataSwitch.Returns_Data;
            crmService.PrepareData();
            var context = crmService.context;

            //setup 
            crmService.AddCustomer();
            crmService.AddCustomerBookingRole();
            var booking = context.Data["tc_booking"].Values.ToList<Entity>()[0];
            var gateway = context.Data["tc_gateway"].Values.ToList<Entity>()[0];
            var user = context.Data["systemuser"].Values.ToList<Entity>()[0];
            var team1 = context.Data["team"].Values.ToList<Entity>()[0];
            var hotel1 = context.Data["tc_hotel"].Values.ToList<Entity>()[0];
            var customer1 = context.Data["contact"].Values.ToList<Entity>()[0];
            var customer2 = context.Data["contact"].Values.ToList<Entity>()[1];
            var customerBookingRole1 = context.Data["tc_customerbookingrole"].Values.ToList<Entity>()[0];
            var customerBookingRole2 = context.Data["tc_customerbookingrole"].Values.ToList<Entity>()[1];
            var accommodation1 = context.Data["tc_bookingaccommodation"].Values.ToList<Entity>()[0];
            var country = context.Data["tc_country"].Values.ToList<Entity>()[0];
            var childTeam = context.Data["team"].Values.ToList<Entity>()[1];

            //Booking start date is today
            crmService.SetBooking(booking, 0, 1, gateway.Id, new EntityReference("systemuser", user.Id));
            hotel1["ownerid"] = new EntityReference("team", team1.Id);
            //accommodation start date is today
            crmService.SetAccommodation(accommodation1, 0, 1, booking.Id, hotel1.Id);
            customer1["ownerid"] = new EntityReference("systemuser", user.Id);
            customer2["ownerid"] = new EntityReference("systemuser", user.Id);
            crmService.SetCustomerBookingRole(customerBookingRole1
                                              , new EntityReference("contact", customer1.Id)
                                              , new EntityReference("tc_booking", booking.Id));
            crmService.SetCustomerBookingRole(customerBookingRole2
                                              , new EntityReference("contact", customer2.Id)
                                              , new EntityReference("tc_booking", booking.Id));

            crmService.SetSourceMarket(customer1, country.Id);
            crmService.SetSourceMarket(customer2, country.Id);
            childTeam["businessunitid"] = new EntityReference("businessunit", ((EntityReference)country["tc_sourcemarketbusinessunitid"]).Id);

            //expected value
            var expectedBookingOwnerId = childTeam.Id;
            var expectedCusomerOwnerId = childTeam.Id;

            var service = new AllocateResortTeamService(logger, allocationService, configurationService);
            service.Run();

            //asserts
            Assert.AreEqual(expectedBookingOwnerId, ((EntityReference)booking["ownerid"]).Id);
            Assert.AreEqual(expectedCusomerOwnerId, ((EntityReference)customer1["ownerid"]).Id);
            Assert.AreEqual(expectedCusomerOwnerId, ((EntityReference)customer2["ownerid"]).Id);

        }

        /// <summary>
        /// Multiple Booking belonging to same customer.
        /// Expected Result: EarliestBooking.Owner = EarliestBooking.Accommodation.Hotel.Owner
        /// LatterBooking,Owner = User, will not be processed
        /// Customer.Owner = EarliestBooking.Accommodation.Hotel.Owner
        /// </summary>
        [TestMethod()]
        public void RunTest_MultipleBookingsSameCustomer()
        {
            crmService.Switch = DataSwitch.Returns_Data;
            crmService.PrepareData();
            var context = crmService.context;

            //setup 
            crmService.AddBooking();
            crmService.AddHotel();
            crmService.AddAccommodation();
            crmService.AddCustomerBookingRole();

            var booking1 = context.Data["tc_booking"].Values.ToList<Entity>()[0];
            var booking2 = context.Data["tc_booking"].Values.ToList<Entity>()[1];
            var gateway1 = context.Data["tc_gateway"].Values.ToList<Entity>()[0];
            var gateway2 = context.Data["tc_gateway"].Values.ToList<Entity>()[0];
            var user = context.Data["systemuser"].Values.ToList<Entity>()[0];
            var team1 = context.Data["team"].Values.ToList<Entity>()[0];
            var team2 = context.Data["team"].Values.ToList<Entity>()[0];
            var hotel1 = context.Data["tc_hotel"].Values.ToList<Entity>()[0];
            var hotel2 = context.Data["tc_hotel"].Values.ToList<Entity>()[1];
            var customer1 = context.Data["contact"].Values.ToList<Entity>()[0];
            var customerBookingRole1 = context.Data["tc_customerbookingrole"].Values.ToList<Entity>()[0];
            var customerBookingRole2 = context.Data["tc_customerbookingrole"].Values.ToList<Entity>()[1];
            var accommodation1 = context.Data["tc_bookingaccommodation"].Values.ToList<Entity>()[0];
            var accommodation2 = context.Data["tc_bookingaccommodation"].Values.ToList<Entity>()[1];
            var country = context.Data["tc_country"].Values.ToList<Entity>()[0];
            var childTeam = context.Data["team"].Values.ToList<Entity>()[1];

            //Booking 1 start date is today
            crmService.SetBooking(booking1, 0, 1, gateway1.Id, new EntityReference("systemuser", user.Id));
            //Booking 1 start date is 2 days from now
            crmService.SetBooking(booking2, 2, 3, gateway2.Id, new EntityReference("systemuser", user.Id));
            hotel1["ownerid"] = new EntityReference("team", team1.Id);
            hotel2["ownerid"] = new EntityReference("team", team2.Id);
            //accommodation start date is today
            crmService.SetAccommodation(accommodation1, 0, 1, booking1.Id, hotel1.Id);
            crmService.SetAccommodation(accommodation2, 2, 3, booking2.Id, hotel2.Id);
            customer1["ownerid"] = new EntityReference("systemuser", user.Id);
            crmService.SetCustomerBookingRole(customerBookingRole1
                                              , new EntityReference("contact", customer1.Id)
                                              , new EntityReference("tc_booking", booking1.Id));
            crmService.SetCustomerBookingRole(customerBookingRole2
                                              , new EntityReference("contact", customer1.Id)
                                              , new EntityReference("tc_booking", booking2.Id));

            crmService.SetSourceMarket(customer1, country.Id);          
            childTeam["businessunitid"] = new EntityReference("businessunit", ((EntityReference)country["tc_sourcemarketbusinessunitid"]).Id);

            //expected value
            var expectedBooking1OwnerId = childTeam.Id;
            var expectedBooking2OwnerId = user.Id;
            var expectedCusomer1OwnerId = childTeam.Id;

            var service = new AllocateResortTeamService(logger, allocationService, configurationService);
            service.Run();

            //asserts
            Assert.AreEqual(expectedBooking1OwnerId, ((EntityReference)booking1["ownerid"]).Id);
            Assert.AreEqual(expectedBooking2OwnerId, ((EntityReference)booking2["ownerid"]).Id);
            Assert.AreEqual(expectedCusomer1OwnerId, ((EntityReference)customer1["ownerid"]).Id);
        }

        /// <summary>
        /// Booking gateway does not belong to the list in config
        /// Expected Result: Booking will not get processed
        /// Booking.Owner = User, Customer.Owner = User
        /// </summary>
        [TestMethod()]
        public void RunTest_InvalidGateway()
        {
            crmService.Switch = DataSwitch.Returns_Data;
            crmService.PrepareData();
            var context = crmService.context;

            //setup 
            var booking = context.Data["tc_booking"].Values.ToList<Entity>()[0];
            var gatewayList = context.Data["tc_gateway"].Values.ToList<Entity>();
            var gateway = gatewayList[gatewayList.Count - 1];
            var user = context.Data["systemuser"].Values.ToList<Entity>()[0];
            var team = context.Data["team"].Values.ToList<Entity>()[0];
            var hotel = context.Data["tc_hotel"].Values.ToList<Entity>()[0];
            var customer = context.Data["contact"].Values.ToList<Entity>()[0];
            var customerBookingRole = context.Data["tc_customerbookingrole"].Values.ToList<Entity>()[0];
            var accommodation = context.Data["tc_bookingaccommodation"].Values.ToList<Entity>()[0];
            var country = context.Data["tc_country"].Values.ToList<Entity>()[0];
            var childTeam = context.Data["team"].Values.ToList<Entity>()[1];

            //Booking start date is today
            crmService.SetBooking(booking, 0, 1, gateway.Id, new EntityReference("systemuser", user.Id));
            hotel["ownerid"] = new EntityReference("team", team.Id);
            //accommodation start date 14 days from now
            crmService.SetAccommodation(accommodation, 0, 1, booking.Id, hotel.Id);
            customer["ownerid"] = new EntityReference("systemuser", user.Id);
            crmService.SetCustomerBookingRole(customerBookingRole
                                              , new EntityReference("contact", customer.Id)
                                              , new EntityReference("tc_booking", booking.Id));

            crmService.SetSourceMarket(customer, country.Id);
            childTeam["businessunitid"] = new EntityReference("businessunit", ((EntityReference)country["tc_sourcemarketbusinessunitid"]).Id);

            //expected value
            var expectedBookingOwnerId = user.Id;
            var expectedCusomerOwnerId = user.Id;


            var service = new AllocateResortTeamService(logger, allocationService, configurationService);
            service.Run();

            //asserts
            Assert.AreEqual(expectedBookingOwnerId, ((EntityReference)booking["ownerid"]).Id);
            Assert.AreEqual(expectedCusomerOwnerId, ((EntityReference)customer["ownerid"]).Id);
        }

        /// <summary>
        /// When no Hotel child teams found for the parent hotel team, then it wont assign to any of the team
        /// Booking.Owner = User, Customer.Owner = User
        /// </summary>
        [TestMethod()]
        public void RunTest_NoChildHotelTeamsFound()
        {
            crmService.Switch = DataSwitch.Returns_Data;
            crmService.PrepareData();
            var context = crmService.context;

            //setup 
            var booking = context.Data["tc_booking"].Values.ToList<Entity>()[0];
            var gatewayList = context.Data["tc_gateway"].Values.ToList<Entity>();
            var gateway = context.Data["tc_gateway"].Values.ToList<Entity>()[0];
            var user = context.Data["systemuser"].Values.ToList<Entity>()[0];
            var team = context.Data["team"].Values.ToList<Entity>()[1];
            var hotel = context.Data["tc_hotel"].Values.ToList<Entity>()[0];
            var customer = context.Data["contact"].Values.ToList<Entity>()[0];
            var customerBookingRole = context.Data["tc_customerbookingrole"].Values.ToList<Entity>()[0];
            var accommodation = context.Data["tc_bookingaccommodation"].Values.ToList<Entity>()[0];
            var country = context.Data["tc_country"].Values.ToList<Entity>()[0];
            var childTeam2 = context.Data["team"].Values.ToList<Entity>()[2];

            //Booking start date is today
            crmService.SetBooking(booking, 0, 1, gateway.Id, new EntityReference("systemuser", user.Id));
            hotel["ownerid"] = new EntityReference("team", team.Id);
            //accommodation start date 14 days from now
            crmService.SetAccommodation(accommodation, 0, 1, booking.Id, hotel.Id);
            customer["ownerid"] = new EntityReference("systemuser", user.Id);
            crmService.SetCustomerBookingRole(customerBookingRole
                                              , new EntityReference("contact", customer.Id)
                                              , new EntityReference("tc_booking", booking.Id));

            crmService.SetSourceMarket(customer, country.Id);
            childTeam2["businessunitid"] = new EntityReference("businessunit", ((EntityReference)country["tc_sourcemarketbusinessunitid"]).Id);

            //expected value
            var expectedBookingOwnerId = user.Id;
            var expectedCusomerOwnerId = user.Id;


            var service = new AllocateResortTeamService(logger, allocationService, configurationService);
            service.Run();

            //asserts
            Assert.AreEqual(expectedBookingOwnerId, ((EntityReference)booking["ownerid"]).Id);
            Assert.AreEqual(expectedCusomerOwnerId, ((EntityReference)customer["ownerid"]).Id);
        }
    }
}