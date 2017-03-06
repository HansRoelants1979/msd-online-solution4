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

namespace Tc.Crm.WebJob.DeallocateResortTeam.Services.Tests
{
    [TestClass()]
    public class DeallocateResortTeamServiceTests
    {
        ILogger logger;
        IConfigurationService configurationService;
        IDeallocationService deallocationService;
        TestCrmService crmService;


        [TestInitialize()]
        public void Setpup()
        {
            this.logger = new TestLogger();
            this.configurationService = new TestConfigurationService();
            this.crmService = new TestCrmService();
            this.deallocationService = new DeallocationService(this.logger, this.crmService);

        }


        /// <summary>
        /// Booking departure date is exactly 9 days from now
        /// Expected Result: Booking.Owner = Hotel.Owner, Customer.Owner = Hotel.Owner
        /// </summary>
        [TestMethod()]
        public void RunTest_AccommodationDateIsMoreThanCurrentDate()
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

            //Booking start date is more than the current date
            crmService.SetBooking(booking, 1, 2, gateway.Id, new EntityReference("team", team.Id));
            hotel["ownerid"] = new EntityReference("team", team.Id);
            //accommodation is more than the current date
            crmService.SetAccommodation(accommodation, 1, 2, booking.Id, hotel.Id);
            customer["ownerid"] = new EntityReference("team", team.Id);
            crmService.SetCustomerBookingRole(customerBookingRole
                                              , new EntityReference("contact", customer.Id)
                                              , new EntityReference("tc_booking", booking.Id));

            //expected value
            var expectedBookingOwnerId = team.Id;
            var expectedCusomerOwnerId = team.Id;


            var service = new DeallocateResortTeamService(logger, deallocationService, configurationService);
            service.Run();

            //asserts
            Assert.AreEqual(expectedBookingOwnerId, ((EntityReference)booking["ownerid"]).Id);
            Assert.AreEqual(expectedCusomerOwnerId, ((EntityReference)customer["ownerid"]).Id);

        }

        [TestMethod()]
        public void RunTest_BookingandCustomerOwnersareUsers()
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

            //Booking start date 9 days from now
            crmService.SetBooking(booking, 1, 2, gateway.Id, new EntityReference("systemuser", user.Id));
            hotel["ownerid"] = new EntityReference("team", team.Id);
            //accommodation start date 9 days from now
            crmService.SetAccommodation(accommodation, 0, 0, booking.Id, hotel.Id);
            customer["ownerid"] = new EntityReference("systemuser", user.Id);
            crmService.SetCustomerBookingRole(customerBookingRole
                                              , new EntityReference("contact", customer.Id)
                                              , new EntityReference("tc_booking", booking.Id));

            //expected value
            var expectedBookingOwnerId = user.Id;
            var expectedCusomerOwnerId = user.Id;


            var service = new DeallocateResortTeamService(logger, deallocationService, configurationService);
            service.Run();

            //asserts
            Assert.AreEqual(expectedBookingOwnerId, ((EntityReference)booking["ownerid"]).Id);
            Assert.AreEqual(expectedCusomerOwnerId, ((EntityReference)customer["ownerid"]).Id);

        }

        [TestMethod()]
        public void RunTest_BookingOwnerisTeamEndDateisCurrentDate()
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

            crmService.SetBooking(booking, -2, 2, gateway.Id, new EntityReference("team", team.Id));
            hotel["ownerid"] = new EntityReference("team", team.Id);
            crmService.SetAccommodation(accommodation, -2, 0, booking.Id, hotel.Id);
            customer["ownerid"] = new EntityReference("team", team.Id);
            crmService.SetCustomerBookingRole(customerBookingRole
                                              , new EntityReference("contact", customer.Id)
                                              , new EntityReference("tc_booking", booking.Id));

            //expected value
            var expectedBookingOwnerId = configurationService.DefaultUserId;
            var expectedCusomerOwnerId = configurationService.DefaultUserId;


            var service = new DeallocateResortTeamService(logger, deallocationService, configurationService);
            service.Run();

            //asserts
            Assert.AreEqual(expectedBookingOwnerId, ((EntityReference)booking["ownerid"]).Id);
            Assert.AreEqual(expectedCusomerOwnerId, ((EntityReference)customer["ownerid"]).Id);

        }

        [TestMethod()]
        public void RunTest_BookingOwnerisTeamEndDateisLessthanCurrentDate()
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

            crmService.SetBooking(booking, -5, -1, gateway.Id, new EntityReference("team", team.Id));
            hotel["ownerid"] = new EntityReference("team", team.Id);
            crmService.SetAccommodation(accommodation, -4, -1, booking.Id, hotel.Id);
            customer["ownerid"] = new EntityReference("team", team.Id);
            crmService.SetCustomerBookingRole(customerBookingRole
                                              , new EntityReference("contact", customer.Id)
                                              , new EntityReference("tc_booking", booking.Id));

            //expected value
            var expectedBookingOwnerId = team.Id;
            var expectedCusomerOwnerId = team.Id;


            var service = new DeallocateResortTeamService(logger, deallocationService, configurationService);
            service.Run();

            //asserts
            Assert.AreEqual(expectedBookingOwnerId, ((EntityReference)booking["ownerid"]).Id);
            Assert.AreEqual(expectedCusomerOwnerId, ((EntityReference)customer["ownerid"]).Id);

        }

        [TestMethod()]
        public void RunTest_MultipleBookingsWithEndDateasCurrentDate()
        {
            crmService.Switch = DataSwitch.Returns_Data;
            crmService.PrepareData();
            var context = crmService.context;

            //setup 
            //setup 
            crmService.AddBooking();
            crmService.AddHotel();
            crmService.AddAccommodation();
            crmService.AddCustomerBookingRole();
            crmService.AddCustomer();

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
            var customer2 = context.Data["contact"].Values.ToList<Entity>()[1];
            var customerBookingRole1 = context.Data["tc_customerbookingrole"].Values.ToList<Entity>()[0];
            var customerBookingRole2 = context.Data["tc_customerbookingrole"].Values.ToList<Entity>()[1];
            var accommodation1 = context.Data["tc_bookingaccommodation"].Values.ToList<Entity>()[0];
            var accommodation2 = context.Data["tc_bookingaccommodation"].Values.ToList<Entity>()[1];

            //Booking 1 start date is today
            crmService.SetBooking(booking1, -2, 0, gateway1.Id, new EntityReference("team", team1.Id));
            //Booking 1 start date is 2 days from now
            crmService.SetBooking(booking2, -3, 0, gateway2.Id, new EntityReference("team", team2.Id));
            hotel1["ownerid"] = new EntityReference("team", team1.Id);
            hotel2["ownerid"] = new EntityReference("team", team2.Id);
            //accommodation start date is today
            crmService.SetAccommodation(accommodation1, -2, 0, booking1.Id, hotel1.Id);
            crmService.SetAccommodation(accommodation2, -3, 0, booking2.Id, hotel2.Id);
            customer1["ownerid"] = new EntityReference("team", team1.Id);
            customer2["ownerid"] = new EntityReference("team", team2.Id);
            crmService.SetCustomerBookingRole(customerBookingRole1
                                              , new EntityReference("contact", customer1.Id)
                                              , new EntityReference("tc_booking", booking1.Id));
            crmService.SetCustomerBookingRole(customerBookingRole2
                                              , new EntityReference("contact", customer2.Id)
                                              , new EntityReference("tc_booking", booking2.Id));



            //expected value
            var expectedBookingOwnerId = configurationService.DefaultUserId;
            var expectedCustomerOwnerId = configurationService.DefaultUserId;


            var service = new DeallocateResortTeamService(logger, deallocationService, configurationService);
            service.Run();

            //asserts

            Assert.AreEqual(expectedBookingOwnerId, ((EntityReference)booking1["ownerid"]).Id);
            Assert.AreEqual(expectedCustomerOwnerId, ((EntityReference)customer1["ownerid"]).Id);
            Assert.AreEqual(expectedBookingOwnerId, ((EntityReference)booking2["ownerid"]).Id);
            Assert.AreEqual(expectedCustomerOwnerId, ((EntityReference)customer2["ownerid"]).Id);

        }




    }
}