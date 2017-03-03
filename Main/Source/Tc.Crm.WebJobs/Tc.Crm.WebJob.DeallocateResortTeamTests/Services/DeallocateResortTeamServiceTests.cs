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

            //Booking start date 9 days from now
            crmService.SetBooking(booking, 1, 2, gateway.Id, new EntityReference("team", team.Id));
            hotel["ownerid"] = new EntityReference("team", team.Id);
            //accommodation start date 9 days from now
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

            //Booking start date 9 days from now
            crmService.SetBooking(booking, 1, 2, gateway.Id, new EntityReference("team", team.Id));
            hotel["ownerid"] = new EntityReference("team", team.Id);
            //accommodation start date 9 days from now
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

            //Booking start date 9 days from now
            crmService.SetBooking(booking, -5, -1, gateway.Id, new EntityReference("team", team.Id));
            hotel["ownerid"] = new EntityReference("team", team.Id);
            //accommodation start date 9 days from now
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
            var booking = context.Data["tc_booking"].Values.ToList<Entity>();
            var gateway = context.Data["tc_gateway"].Values.ToList<Entity>()[0];
            var user = context.Data["systemuser"].Values.ToList<Entity>()[0];
            var team = context.Data["team"].Values.ToList<Entity>()[0];
            var hotel = context.Data["tc_hotel"].Values.ToList<Entity>()[0];
            var customer = context.Data["contact"].Values.ToList<Entity>()[0];
            var customerBookingRole = context.Data["tc_customerbookingrole"].Values.ToList<Entity>()[0];
            var accommodation = context.Data["tc_bookingaccommodation"].Values.ToList<Entity>();
            //var accommodation1 = context.Data["tc_bookingaccommodation"].Values.ToList<Entity>()[1];

            //Booking start date 9 days from now
            for (int i = 0; i < booking.Count; i++)
            {
                crmService.SetBooking(booking[i], (-10 + i), (0 + i), gateway.Id, new EntityReference("team", team.Id));
                hotel["ownerid"] = new EntityReference("team", team.Id);
                //accommodation start date 9 days from now
                crmService.SetAccommodation(accommodation[i], (-11 + i), 0, booking[i].Id, hotel.Id);
                //crmService.SetAccommodation(accommodation1, (-11 + i), 0, booking[i].Id, hotel.Id);
                customer["ownerid"] = new EntityReference("team", team.Id);
                crmService.SetCustomerBookingRole(customerBookingRole
                                                  , new EntityReference("contact", customer.Id)
                                                  , new EntityReference("tc_booking", booking[i].Id));
            }
           

            //expected value
            var expectedBookingOwnerId = configurationService.DefaultUserId;
            var expectedCustomerOwnerId = configurationService.DefaultUserId;


            var service = new DeallocateResortTeamService(logger, deallocationService, configurationService);
            service.Run();

            //asserts
            for (int i = 0; i < booking.Count; i++)
            {
                Assert.AreEqual(expectedBookingOwnerId, ((EntityReference)booking[i]["ownerid"]).Id);
                Assert.AreEqual(expectedCustomerOwnerId, ((EntityReference)customer["ownerid"]).Id);
            }

        }

       


    }
}