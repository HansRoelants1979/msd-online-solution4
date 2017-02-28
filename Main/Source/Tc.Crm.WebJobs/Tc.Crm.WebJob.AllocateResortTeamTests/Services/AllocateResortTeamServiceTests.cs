using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tc.Crm.WebJob.AllocateResortTeam.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        [TestMethod()]
        public void RunTest_No_Rcords_Returned()
        {
            crmService.Switch = DataSwitch.No_Records_Returned;
            var service = new AllocateResortTeamService(logger, allocationService, configurationService);
            service.Run();
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void RunTest_Collection_With_No_Records()
        {
            crmService.Switch = DataSwitch.Collection_With_No_Records_Returned;
            var service = new AllocateResortTeamService(logger, allocationService, configurationService);
            service.Run();
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void RunTest_Only_1_Accomodation_Departure_9th_Day_Exactly()
        {
            crmService.Switch = DataSwitch.Returns_Data;
            var context = crmService.context;
            context.Data.Clear();
            //prepare data

            #region User
            var userEntityCollection = new Dictionary<Guid, Entity>();
            var u1Id = Guid.NewGuid();
            var u1 = new Entity("systemuser", u1Id);
            u1["systemuserid"] = u1Id;
            u1["firstname"] = "Pra";
            userEntityCollection.Add(u1Id, u1);
            #endregion User

            #region Team
            var teamEntityCollection = new Dictionary<Guid, Entity>();
            var t1Id = Guid.NewGuid();
            var t1 = new Entity("team", t1Id);
            t1["teamid"] = t1Id;
            t1["name"] = "Resort Team 1";

            var t2Id = Guid.NewGuid();
            var t2 = new Entity("team", t2Id);
            t2["teamid"] = t1Id;
            t2["name"] = "Resort Team 2";

            var t3Id = Guid.NewGuid();
            var t3 = new Entity("team", t3Id);
            t3["teamid"] = t1Id;
            t3["name"] = "Resort Team 3";

            teamEntityCollection.Add(t1Id, t1);
            teamEntityCollection.Add(t2Id, t2);
            teamEntityCollection.Add(t3Id, t3);
            #endregion Team

            #region contact
            var contactEntityCollection = new Dictionary<Guid, Entity>();
            var con1Id = Guid.NewGuid();
            var con1 = new Entity("contact", con1Id);
            con1["contactid"] = con1Id;
            con1["lastname"] = "Gogol";
            con1["firstname"] = "Nikolai";
            con1["ownerid"] = new EntityReference("systemuser",u1Id);
            contactEntityCollection.Add(con1Id, con1);
            #endregion contact

            #region Gateway
            var gatewayEntityCollection = new Dictionary<Guid, Entity>();

            var g1Id = new Guid("37c4bfbe-29f8-e611-810b-1458d041f8e8");
            var g1 = new Entity("tc_gateway", g1Id);
            g1["tc_gateway"] = "UK";

            gatewayEntityCollection.Add(g1Id, g1);
            #endregion Gateway

            #region Booking
            var bookingEntityCollection = new Dictionary<Guid, Entity>();

            var b1Id = Guid.NewGuid();
            var b1 = new Entity("tc_booking",b1Id);
            b1["tc_bookingid"] = b1Id;
            b1["tc_name"] = "b1";
            b1["ownerid"] = new EntityReference("systemuser",u1Id);
            b1["tc_departuredate"] = DateTime.Now.Date.AddDays(9);
            b1["tc_returndate"] = DateTime.Now.Date.AddDays(11);
            b1["tc_gatewayid"] = new EntityReference("tc_gateway", g1Id);

            bookingEntityCollection.Add(b1Id, b1);
            #endregion Booking

            #region Hotel
            var hotelEntityCollection = new Dictionary<Guid, Entity>();

            var hot1Id = Guid.NewGuid();
            var hot1 = new Entity("tc_hotel", hot1Id);
            hot1["ownerid"] = new EntityReference("team",t1Id);

            hotelEntityCollection.Add(hot1Id,hot1);
            #endregion Hotel

           

            #region accommodation
            var accommodationEntityCollection = new Dictionary<Guid, Entity>();
            var acc1Id = Guid.NewGuid();
            var acc1 = new Entity("tc_accommodation",acc1Id);
            acc1["tc_accommodationid"] = acc1Id;
            acc1["tc_startdateandtime"] = DateTime.Now.Date.AddDays(9);
            acc1["tc_enddateandtime"] = DateTime.Now.Date.AddDays(11);
            acc1["tc_bookingid"] = new EntityReference("tc_booking",b1Id);
            acc1["tc_hotelid"] = new EntityReference("tc_hotel", hot1Id);
            accommodationEntityCollection.Add(acc1Id,acc1);
            #endregion

            #region BookingRole
            var customerBookingRoleEntityCollection = new Dictionary<Guid, Entity>();

            var cbr1Id = Guid.NewGuid();
            var cbr1 = new Entity("tc_customerbookingrole");
            cbr1["tc_customerbookingrole"] = cbr1Id;
            cbr1["tc_bookingid"] = new EntityReference("tc_booking", b1Id);
            cbr1["tc_customer"] = new EntityReference("contact",con1Id);

            customerBookingRoleEntityCollection.Add(cbr1Id, cbr1);
            #endregion BookingRole

            context.Data.Add("systemuser", userEntityCollection);
            context.Data.Add("team", teamEntityCollection);
            context.Data.Add("contact", contactEntityCollection);
            context.Data.Add("tc_hotel", hotelEntityCollection);
            context.Data.Add("tc_booking", bookingEntityCollection);
            context.Data.Add("tc_accommodation", accommodationEntityCollection);
            context.Data.Add("tc_customerbookingrole", customerBookingRoleEntityCollection);

            var service = new AllocateResortTeamService(logger, allocationService, configurationService);
            service.Run();
            var bookingOwner = (EntityReference)(context.Data["tc_booking"][b1Id]["ownerid"]);
            var customerOwner = (EntityReference)(context.Data["contact"][con1Id]["ownerid"]);
            Assert.AreEqual(bookingOwner.Id, t1.Id);
            Assert.AreEqual(customerOwner.Id, t1.Id);
        }

        [TestMethod()]
        public void RunTest_Only_1_Accomodation_Departure_10th_Day_Exactly()
        {
            crmService.Switch = DataSwitch.Returns_Data;
            var context = crmService.context;
            context.Data.Clear();
            //prepare data

            #region User
            var userEntityCollection = new Dictionary<Guid, Entity>();
            var u1Id = Guid.NewGuid();
            var u1 = new Entity("systemuser", u1Id);
            u1["systemuserid"] = u1Id;
            u1["firstname"] = "Pra";
            userEntityCollection.Add(u1Id, u1);
            #endregion User

            #region Team
            var teamEntityCollection = new Dictionary<Guid, Entity>();
            var t1Id = Guid.NewGuid();
            var t1 = new Entity("team", t1Id);
            t1["teamid"] = t1Id;
            t1["name"] = "Resort Team 1";

            var t2Id = Guid.NewGuid();
            var t2 = new Entity("team", t2Id);
            t2["teamid"] = t1Id;
            t2["name"] = "Resort Team 2";

            var t3Id = Guid.NewGuid();
            var t3 = new Entity("team", t3Id);
            t3["teamid"] = t1Id;
            t3["name"] = "Resort Team 3";

            teamEntityCollection.Add(t1Id, t1);
            teamEntityCollection.Add(t2Id, t2);
            teamEntityCollection.Add(t3Id, t3);
            #endregion Team

            #region contact
            var contactEntityCollection = new Dictionary<Guid, Entity>();
            var con1Id = Guid.NewGuid();
            var con1 = new Entity("contact", con1Id);
            con1["contactid"] = con1Id;
            con1["lastname"] = "Gogol";
            con1["firstname"] = "Nikolai";
            con1["ownerid"] = new EntityReference("systemuser", u1Id);
            contactEntityCollection.Add(con1Id, con1);
            #endregion contact

            #region Gateway
            var gatewayEntityCollection = new Dictionary<Guid, Entity>();

            var g1Id = new Guid("37c4bfbe-29f8-e611-810b-1458d041f8e8");
            var g1 = new Entity("tc_gateway", g1Id);
            g1["tc_gateway"] = "UK";

            gatewayEntityCollection.Add(g1Id, g1);
            #endregion Gateway

            #region Booking
            var bookingEntityCollection = new Dictionary<Guid, Entity>();

            var b1Id = Guid.NewGuid();
            var b1 = new Entity("tc_booking", b1Id);
            b1["tc_bookingid"] = b1Id;
            b1["tc_name"] = "b1";
            b1["ownerid"] = new EntityReference("systemuser", u1Id);
            b1["tc_departuredate"] = DateTime.Now.Date.AddDays(10);
            b1["tc_returndate"] = DateTime.Now.Date.AddDays(12);
            b1["tc_gatewayid"] = new EntityReference("tc_gateway", g1Id);

            bookingEntityCollection.Add(b1Id, b1);
            #endregion Booking

            #region Hotel
            var hotelEntityCollection = new Dictionary<Guid, Entity>();

            var hot1Id = Guid.NewGuid();
            var hot1 = new Entity("tc_hotel", hot1Id);
            hot1["ownerid"] = new EntityReference("team", t1Id);

            hotelEntityCollection.Add(hot1Id, hot1);
            #endregion Hotel



            #region accommodation
            var accommodationEntityCollection = new Dictionary<Guid, Entity>();
            var acc1Id = Guid.NewGuid();
            var acc1 = new Entity("tc_accommodation", acc1Id);
            acc1["tc_accommodationid"] = acc1Id;
            acc1["tc_startdateandtime"] = DateTime.Now.Date.AddDays(10);
            acc1["tc_enddateandtime"] = DateTime.Now.Date.AddDays(12);
            acc1["tc_bookingid"] = new EntityReference("tc_booking", b1Id);
            acc1["tc_hotelid"] = new EntityReference("tc_hotel", hot1Id);
            accommodationEntityCollection.Add(acc1Id, acc1);
            #endregion

            #region BookingRole
            var customerBookingRoleEntityCollection = new Dictionary<Guid, Entity>();

            var cbr1Id = Guid.NewGuid();
            var cbr1 = new Entity("tc_customerbookingrole");
            cbr1["tc_customerbookingrole"] = cbr1Id;
            cbr1["tc_bookingid"] = new EntityReference("tc_booking", b1Id);
            cbr1["tc_customer"] = new EntityReference("contact", con1Id);

            customerBookingRoleEntityCollection.Add(cbr1Id, cbr1);
            #endregion BookingRole

            context.Data.Add("systemuser", userEntityCollection);
            context.Data.Add("team", teamEntityCollection);
            context.Data.Add("contact", contactEntityCollection);
            context.Data.Add("tc_hotel", hotelEntityCollection);
            context.Data.Add("tc_booking", bookingEntityCollection);
            context.Data.Add("tc_accommodation", accommodationEntityCollection);
            context.Data.Add("tc_customerbookingrole", customerBookingRoleEntityCollection);

            var service = new AllocateResortTeamService(logger, allocationService, configurationService);
            service.Run();
            var bookingOwner = (EntityReference)(context.Data["tc_booking"][b1Id]["ownerid"]);
            var customerOwner = (EntityReference)(context.Data["contact"][con1Id]["ownerid"]);
            Assert.AreEqual(bookingOwner.Id, u1.Id);
            Assert.AreEqual(customerOwner.Id, u1.Id);
        }

        [TestMethod()]
        public void RunTest_Only_1_Accomodation_Departure_9th_Day_Exactly_Acc_On_10th_Day()
        {
            crmService.Switch = DataSwitch.Returns_Data;
            var context = crmService.context;
            context.Data.Clear();
            //prepare data

            #region User
            var userEntityCollection = new Dictionary<Guid, Entity>();
            var u1Id = Guid.NewGuid();
            var u1 = new Entity("systemuser", u1Id);
            u1["systemuserid"] = u1Id;
            u1["firstname"] = "Pra";
            userEntityCollection.Add(u1Id, u1);
            #endregion User

            #region Team
            var teamEntityCollection = new Dictionary<Guid, Entity>();
            var t1Id = Guid.NewGuid();
            var t1 = new Entity("team", t1Id);
            t1["teamid"] = t1Id;
            t1["name"] = "Resort Team 1";

            var t2Id = Guid.NewGuid();
            var t2 = new Entity("team", t2Id);
            t2["teamid"] = t1Id;
            t2["name"] = "Resort Team 2";

            var t3Id = Guid.NewGuid();
            var t3 = new Entity("team", t3Id);
            t3["teamid"] = t1Id;
            t3["name"] = "Resort Team 3";

            teamEntityCollection.Add(t1Id, t1);
            teamEntityCollection.Add(t2Id, t2);
            teamEntityCollection.Add(t3Id, t3);
            #endregion Team

            #region contact
            var contactEntityCollection = new Dictionary<Guid, Entity>();
            var con1Id = Guid.NewGuid();
            var con1 = new Entity("contact", con1Id);
            con1["contactid"] = con1Id;
            con1["lastname"] = "Gogol";
            con1["firstname"] = "Nikolai";
            con1["ownerid"] = new EntityReference("systemuser", u1Id);
            contactEntityCollection.Add(con1Id, con1);
            #endregion contact

            #region Gateway
            var gatewayEntityCollection = new Dictionary<Guid, Entity>();

            var g1Id = new Guid("37c4bfbe-29f8-e611-810b-1458d041f8e8");
            var g1 = new Entity("tc_gateway", g1Id);
            g1["tc_gateway"] = "UK";

            gatewayEntityCollection.Add(g1Id, g1);
            #endregion Gateway

            #region Booking
            var bookingEntityCollection = new Dictionary<Guid, Entity>();

            var b1Id = Guid.NewGuid();
            var b1 = new Entity("tc_booking", b1Id);
            b1["tc_bookingid"] = b1Id;
            b1["tc_name"] = "b1";
            b1["ownerid"] = new EntityReference("systemuser", u1Id);
            b1["tc_departuredate"] = DateTime.Now.Date.AddDays(9);
            b1["tc_returndate"] = DateTime.Now.Date.AddDays(11);
            b1["tc_gatewayid"] = new EntityReference("tc_gateway", g1Id);

            bookingEntityCollection.Add(b1Id, b1);
            #endregion Booking

            #region Hotel
            var hotelEntityCollection = new Dictionary<Guid, Entity>();

            var hot1Id = Guid.NewGuid();
            var hot1 = new Entity("tc_hotel", hot1Id);
            hot1["ownerid"] = new EntityReference("team", t1Id);

            hotelEntityCollection.Add(hot1Id, hot1);
            #endregion Hotel



            #region accommodation
            var accommodationEntityCollection = new Dictionary<Guid, Entity>();
            var acc1Id = Guid.NewGuid();
            var acc1 = new Entity("tc_accommodation", acc1Id);
            acc1["tc_accommodationid"] = acc1Id;
            acc1["tc_startdateandtime"] = DateTime.Now.Date.AddDays(10);
            acc1["tc_enddateandtime"] = DateTime.Now.Date.AddDays(11);
            acc1["tc_bookingid"] = new EntityReference("tc_booking", b1Id);
            acc1["tc_hotelid"] = new EntityReference("tc_hotel", hot1Id);
            accommodationEntityCollection.Add(acc1Id, acc1);
            #endregion

            #region BookingRole
            var customerBookingRoleEntityCollection = new Dictionary<Guid, Entity>();

            var cbr1Id = Guid.NewGuid();
            var cbr1 = new Entity("tc_customerbookingrole");
            cbr1["tc_customerbookingrole"] = cbr1Id;
            cbr1["tc_bookingid"] = new EntityReference("tc_booking", b1Id);
            cbr1["tc_customer"] = new EntityReference("contact", con1Id);

            customerBookingRoleEntityCollection.Add(cbr1Id, cbr1);
            #endregion BookingRole

            context.Data.Add("systemuser", userEntityCollection);
            context.Data.Add("team", teamEntityCollection);
            context.Data.Add("contact", contactEntityCollection);
            context.Data.Add("tc_hotel", hotelEntityCollection);
            context.Data.Add("tc_booking", bookingEntityCollection);
            context.Data.Add("tc_accommodation", accommodationEntityCollection);
            context.Data.Add("tc_customerbookingrole", customerBookingRoleEntityCollection);

            var service = new AllocateResortTeamService(logger, allocationService, configurationService);
            service.Run();
            var bookingOwner = (EntityReference)(context.Data["tc_booking"][b1Id]["ownerid"]);
            var customerOwner = (EntityReference)(context.Data["contact"][con1Id]["ownerid"]);
            Assert.AreEqual(bookingOwner.Id, u1.Id);
            Assert.AreEqual(customerOwner.Id, u1.Id);
        }

        [TestMethod()]
        public void RunTest_Only_1_Accomodation_Departure_10th_Day_Exactly_Acc_On_9th_Day()
        {
            crmService.Switch = DataSwitch.Returns_Data;
            var context = crmService.context;
            context.Data.Clear();
            //prepare data

            #region User
            var userEntityCollection = new Dictionary<Guid, Entity>();
            var u1Id = Guid.NewGuid();
            var u1 = new Entity("systemuser", u1Id);
            u1["systemuserid"] = u1Id;
            u1["firstname"] = "Pra";
            userEntityCollection.Add(u1Id, u1);
            #endregion User

            #region Team
            var teamEntityCollection = new Dictionary<Guid, Entity>();
            var t1Id = Guid.NewGuid();
            var t1 = new Entity("team", t1Id);
            t1["teamid"] = t1Id;
            t1["name"] = "Resort Team 1";

            var t2Id = Guid.NewGuid();
            var t2 = new Entity("team", t2Id);
            t2["teamid"] = t1Id;
            t2["name"] = "Resort Team 2";

            var t3Id = Guid.NewGuid();
            var t3 = new Entity("team", t3Id);
            t3["teamid"] = t1Id;
            t3["name"] = "Resort Team 3";

            teamEntityCollection.Add(t1Id, t1);
            teamEntityCollection.Add(t2Id, t2);
            teamEntityCollection.Add(t3Id, t3);
            #endregion Team

            #region contact
            var contactEntityCollection = new Dictionary<Guid, Entity>();
            var con1Id = Guid.NewGuid();
            var con1 = new Entity("contact", con1Id);
            con1["contactid"] = con1Id;
            con1["lastname"] = "Gogol";
            con1["firstname"] = "Nikolai";
            con1["ownerid"] = new EntityReference("systemuser", u1Id);
            contactEntityCollection.Add(con1Id, con1);
            #endregion contact

            #region Gateway
            var gatewayEntityCollection = new Dictionary<Guid, Entity>();

            var g1Id = new Guid("37c4bfbe-29f8-e611-810b-1458d041f8e8");
            var g1 = new Entity("tc_gateway", g1Id);
            g1["tc_gateway"] = "UK";

            gatewayEntityCollection.Add(g1Id, g1);
            #endregion Gateway

            #region Booking
            var bookingEntityCollection = new Dictionary<Guid, Entity>();

            var b1Id = Guid.NewGuid();
            var b1 = new Entity("tc_booking", b1Id);
            b1["tc_bookingid"] = b1Id;
            b1["tc_name"] = "b1";
            b1["ownerid"] = new EntityReference("systemuser", u1Id);
            b1["tc_departuredate"] = DateTime.Now.Date.AddDays(10);
            b1["tc_returndate"] = DateTime.Now.Date.AddDays(11);
            b1["tc_gatewayid"] = new EntityReference("tc_gateway", g1Id);

            bookingEntityCollection.Add(b1Id, b1);
            #endregion Booking

            #region Hotel
            var hotelEntityCollection = new Dictionary<Guid, Entity>();

            var hot1Id = Guid.NewGuid();
            var hot1 = new Entity("tc_hotel", hot1Id);
            hot1["ownerid"] = new EntityReference("team", t1Id);

            hotelEntityCollection.Add(hot1Id, hot1);
            #endregion Hotel



            #region accommodation
            var accommodationEntityCollection = new Dictionary<Guid, Entity>();
            var acc1Id = Guid.NewGuid();
            var acc1 = new Entity("tc_accommodation", acc1Id);
            acc1["tc_accommodationid"] = acc1Id;
            acc1["tc_startdateandtime"] = DateTime.Now.Date.AddDays(9);
            acc1["tc_enddateandtime"] = DateTime.Now.Date.AddDays(11);
            acc1["tc_bookingid"] = new EntityReference("tc_booking", b1Id);
            acc1["tc_hotelid"] = new EntityReference("tc_hotel", hot1Id);
            accommodationEntityCollection.Add(acc1Id, acc1);
            #endregion

            #region BookingRole
            var customerBookingRoleEntityCollection = new Dictionary<Guid, Entity>();

            var cbr1Id = Guid.NewGuid();
            var cbr1 = new Entity("tc_customerbookingrole");
            cbr1["tc_customerbookingrole"] = cbr1Id;
            cbr1["tc_bookingid"] = new EntityReference("tc_booking", b1Id);
            cbr1["tc_customer"] = new EntityReference("contact", con1Id);

            customerBookingRoleEntityCollection.Add(cbr1Id, cbr1);
            #endregion BookingRole

            context.Data.Add("systemuser", userEntityCollection);
            context.Data.Add("team", teamEntityCollection);
            context.Data.Add("contact", contactEntityCollection);
            context.Data.Add("tc_hotel", hotelEntityCollection);
            context.Data.Add("tc_booking", bookingEntityCollection);
            context.Data.Add("tc_accommodation", accommodationEntityCollection);
            context.Data.Add("tc_customerbookingrole", customerBookingRoleEntityCollection);

            var service = new AllocateResortTeamService(logger, allocationService, configurationService);
            service.Run();
            var bookingOwner = (EntityReference)(context.Data["tc_booking"][b1Id]["ownerid"]);
            var customerOwner = (EntityReference)(context.Data["contact"][con1Id]["ownerid"]);
            Assert.AreEqual(bookingOwner.Id, u1.Id);
            Assert.AreEqual(customerOwner.Id, u1.Id);
        }

        [TestMethod()]
        public void RunTest_Only_1_Accomodation_Departure_8th_Day_Exactly()
        {
            crmService.Switch = DataSwitch.Returns_Data;
            var context = crmService.context;
            context.Data.Clear();
            //prepare data

            #region User
            var userEntityCollection = new Dictionary<Guid, Entity>();
            var u1Id = Guid.NewGuid();
            var u1 = new Entity("systemuser", u1Id);
            u1["systemuserid"] = u1Id;
            u1["firstname"] = "Pra";
            userEntityCollection.Add(u1Id, u1);
            #endregion User

            #region Team
            var teamEntityCollection = new Dictionary<Guid, Entity>();
            var t1Id = Guid.NewGuid();
            var t1 = new Entity("team", t1Id);
            t1["teamid"] = t1Id;
            t1["name"] = "Resort Team 1";

            var t2Id = Guid.NewGuid();
            var t2 = new Entity("team", t2Id);
            t2["teamid"] = t1Id;
            t2["name"] = "Resort Team 2";

            var t3Id = Guid.NewGuid();
            var t3 = new Entity("team", t3Id);
            t3["teamid"] = t1Id;
            t3["name"] = "Resort Team 3";

            teamEntityCollection.Add(t1Id, t1);
            teamEntityCollection.Add(t2Id, t2);
            teamEntityCollection.Add(t3Id, t3);
            #endregion Team

            #region contact
            var contactEntityCollection = new Dictionary<Guid, Entity>();
            var con1Id = Guid.NewGuid();
            var con1 = new Entity("contact", con1Id);
            con1["contactid"] = con1Id;
            con1["lastname"] = "Gogol";
            con1["firstname"] = "Nikolai";
            con1["ownerid"] = new EntityReference("systemuser", u1Id);
            contactEntityCollection.Add(con1Id, con1);
            #endregion contact

            #region Gateway
            var gatewayEntityCollection = new Dictionary<Guid, Entity>();

            var g1Id = new Guid("37c4bfbe-29f8-e611-810b-1458d041f8e8");
            var g1 = new Entity("tc_gateway", g1Id);
            g1["tc_gateway"] = "UK";

            gatewayEntityCollection.Add(g1Id, g1);
            #endregion Gateway

            #region Booking
            var bookingEntityCollection = new Dictionary<Guid, Entity>();

            var b1Id = Guid.NewGuid();
            var b1 = new Entity("tc_booking", b1Id);
            b1["tc_bookingid"] = b1Id;
            b1["tc_name"] = "b1";
            b1["ownerid"] = new EntityReference("systemuser", u1Id);
            b1["tc_departuredate"] = DateTime.Now.Date.AddDays(8);
            b1["tc_returndate"] = DateTime.Now.Date.AddDays(11);
            b1["tc_gatewayid"] = new EntityReference("tc_gateway", g1Id);

            bookingEntityCollection.Add(b1Id, b1);
            #endregion Booking

            #region Hotel
            var hotelEntityCollection = new Dictionary<Guid, Entity>();

            var hot1Id = Guid.NewGuid();
            var hot1 = new Entity("tc_hotel", hot1Id);
            hot1["ownerid"] = new EntityReference("team", t1Id);

            hotelEntityCollection.Add(hot1Id, hot1);
            #endregion Hotel



            #region accommodation
            var accommodationEntityCollection = new Dictionary<Guid, Entity>();
            var acc1Id = Guid.NewGuid();
            var acc1 = new Entity("tc_accommodation", acc1Id);
            acc1["tc_accommodationid"] = acc1Id;
            acc1["tc_startdateandtime"] = DateTime.Now.Date.AddDays(8);
            acc1["tc_enddateandtime"] = DateTime.Now.Date.AddDays(11);
            acc1["tc_bookingid"] = new EntityReference("tc_booking", b1Id);
            acc1["tc_hotelid"] = new EntityReference("tc_hotel", hot1Id);
            accommodationEntityCollection.Add(acc1Id, acc1);
            #endregion

            #region BookingRole
            var customerBookingRoleEntityCollection = new Dictionary<Guid, Entity>();

            var cbr1Id = Guid.NewGuid();
            var cbr1 = new Entity("tc_customerbookingrole");
            cbr1["tc_customerbookingrole"] = cbr1Id;
            cbr1["tc_bookingid"] = new EntityReference("tc_booking", b1Id);
            cbr1["tc_customer"] = new EntityReference("contact", con1Id);

            customerBookingRoleEntityCollection.Add(cbr1Id, cbr1);
            #endregion BookingRole

            context.Data.Add("systemuser", userEntityCollection);
            context.Data.Add("team", teamEntityCollection);
            context.Data.Add("contact", contactEntityCollection);
            context.Data.Add("tc_hotel", hotelEntityCollection);
            context.Data.Add("tc_booking", bookingEntityCollection);
            context.Data.Add("tc_accommodation", accommodationEntityCollection);
            context.Data.Add("tc_customerbookingrole", customerBookingRoleEntityCollection);

            var service = new AllocateResortTeamService(logger, allocationService, configurationService);
            service.Run();
            var bookingOwner = (EntityReference)(context.Data["tc_booking"][b1Id]["ownerid"]);
            var customerOwner = (EntityReference)(context.Data["contact"][con1Id]["ownerid"]);
            Assert.AreEqual(bookingOwner.Id, t1.Id);
            Assert.AreEqual(customerOwner.Id, t1.Id);
        }

        [TestMethod()]
        public void RunTest_Only_1_Accomodation_Current_Day()
        {
            crmService.Switch = DataSwitch.Returns_Data;
            var context = crmService.context;
            context.Data.Clear();
            //prepare data

            #region User
            var userEntityCollection = new Dictionary<Guid, Entity>();
            var u1Id = Guid.NewGuid();
            var u1 = new Entity("systemuser", u1Id);
            u1["systemuserid"] = u1Id;
            u1["firstname"] = "Pra";
            userEntityCollection.Add(u1Id, u1);
            #endregion User

            #region Team
            var teamEntityCollection = new Dictionary<Guid, Entity>();
            var t1Id = Guid.NewGuid();
            var t1 = new Entity("team", t1Id);
            t1["teamid"] = t1Id;
            t1["name"] = "Resort Team 1";

            var t2Id = Guid.NewGuid();
            var t2 = new Entity("team", t2Id);
            t2["teamid"] = t1Id;
            t2["name"] = "Resort Team 2";

            var t3Id = Guid.NewGuid();
            var t3 = new Entity("team", t3Id);
            t3["teamid"] = t1Id;
            t3["name"] = "Resort Team 3";

            teamEntityCollection.Add(t1Id, t1);
            teamEntityCollection.Add(t2Id, t2);
            teamEntityCollection.Add(t3Id, t3);
            #endregion Team

            #region contact
            var contactEntityCollection = new Dictionary<Guid, Entity>();
            var con1Id = Guid.NewGuid();
            var con1 = new Entity("contact", con1Id);
            con1["contactid"] = con1Id;
            con1["lastname"] = "Gogol";
            con1["firstname"] = "Nikolai";
            con1["ownerid"] = new EntityReference("systemuser", u1Id);
            contactEntityCollection.Add(con1Id, con1);
            #endregion contact

            #region Gateway
            var gatewayEntityCollection = new Dictionary<Guid, Entity>();

            var g1Id = new Guid("37c4bfbe-29f8-e611-810b-1458d041f8e8");
            var g1 = new Entity("tc_gateway", g1Id);
            g1["tc_gateway"] = "UK";

            gatewayEntityCollection.Add(g1Id, g1);
            #endregion Gateway

            #region Booking
            var bookingEntityCollection = new Dictionary<Guid, Entity>();

            var b1Id = Guid.NewGuid();
            var b1 = new Entity("tc_booking", b1Id);
            b1["tc_bookingid"] = b1Id;
            b1["tc_name"] = "b1";
            b1["ownerid"] = new EntityReference("systemuser", u1Id);
            b1["tc_departuredate"] = DateTime.Now.Date;
            b1["tc_returndate"] = DateTime.Now.Date.AddDays(11);
            b1["tc_gatewayid"] = new EntityReference("tc_gateway", g1Id);

            bookingEntityCollection.Add(b1Id, b1);
            #endregion Booking

            #region Hotel
            var hotelEntityCollection = new Dictionary<Guid, Entity>();

            var hot1Id = Guid.NewGuid();
            var hot1 = new Entity("tc_hotel", hot1Id);
            hot1["ownerid"] = new EntityReference("team", t1Id);

            hotelEntityCollection.Add(hot1Id, hot1);
            #endregion Hotel



            #region accommodation
            var accommodationEntityCollection = new Dictionary<Guid, Entity>();
            var acc1Id = Guid.NewGuid();
            var acc1 = new Entity("tc_accommodation", acc1Id);
            acc1["tc_accommodationid"] = acc1Id;
            acc1["tc_startdateandtime"] = DateTime.Now.Date;
            acc1["tc_enddateandtime"] = DateTime.Now.Date.AddDays(11);
            acc1["tc_bookingid"] = new EntityReference("tc_booking", b1Id);
            acc1["tc_hotelid"] = new EntityReference("tc_hotel", hot1Id);
            accommodationEntityCollection.Add(acc1Id, acc1);
            #endregion

            #region BookingRole
            var customerBookingRoleEntityCollection = new Dictionary<Guid, Entity>();

            var cbr1Id = Guid.NewGuid();
            var cbr1 = new Entity("tc_customerbookingrole");
            cbr1["tc_customerbookingrole"] = cbr1Id;
            cbr1["tc_bookingid"] = new EntityReference("tc_booking", b1Id);
            cbr1["tc_customer"] = new EntityReference("contact", con1Id);

            customerBookingRoleEntityCollection.Add(cbr1Id, cbr1);
            #endregion BookingRole

            context.Data.Add("systemuser", userEntityCollection);
            context.Data.Add("team", teamEntityCollection);
            context.Data.Add("contact", contactEntityCollection);
            context.Data.Add("tc_hotel", hotelEntityCollection);
            context.Data.Add("tc_booking", bookingEntityCollection);
            context.Data.Add("tc_accommodation", accommodationEntityCollection);
            context.Data.Add("tc_customerbookingrole", customerBookingRoleEntityCollection);

            var service = new AllocateResortTeamService(logger, allocationService, configurationService);
            service.Run();
            var bookingOwner = (EntityReference)(context.Data["tc_booking"][b1Id]["ownerid"]);
            var customerOwner = (EntityReference)(context.Data["contact"][con1Id]["ownerid"]);
            Assert.AreEqual(bookingOwner.Id, t1.Id);
            Assert.AreEqual(customerOwner.Id, t1.Id);
        }

        [TestMethod()]
        public void RunTest_Only_1_Accomodation_Current_Day_Plus_One()
        {
            crmService.Switch = DataSwitch.Returns_Data;
            var context = crmService.context;
            context.Data.Clear();
            //prepare data

            #region User
            var userEntityCollection = new Dictionary<Guid, Entity>();
            var u1Id = Guid.NewGuid();
            var u1 = new Entity("systemuser", u1Id);
            u1["systemuserid"] = u1Id;
            u1["firstname"] = "Pra";
            userEntityCollection.Add(u1Id, u1);
            #endregion User

            #region Team
            var teamEntityCollection = new Dictionary<Guid, Entity>();
            var t1Id = Guid.NewGuid();
            var t1 = new Entity("team", t1Id);
            t1["teamid"] = t1Id;
            t1["name"] = "Resort Team 1";

            var t2Id = Guid.NewGuid();
            var t2 = new Entity("team", t2Id);
            t2["teamid"] = t1Id;
            t2["name"] = "Resort Team 2";

            var t3Id = Guid.NewGuid();
            var t3 = new Entity("team", t3Id);
            t3["teamid"] = t1Id;
            t3["name"] = "Resort Team 3";

            teamEntityCollection.Add(t1Id, t1);
            teamEntityCollection.Add(t2Id, t2);
            teamEntityCollection.Add(t3Id, t3);
            #endregion Team

            #region contact
            var contactEntityCollection = new Dictionary<Guid, Entity>();
            var con1Id = Guid.NewGuid();
            var con1 = new Entity("contact", con1Id);
            con1["contactid"] = con1Id;
            con1["lastname"] = "Gogol";
            con1["firstname"] = "Nikolai";
            con1["ownerid"] = new EntityReference("systemuser", u1Id);
            contactEntityCollection.Add(con1Id, con1);
            #endregion contact

            #region Gateway
            var gatewayEntityCollection = new Dictionary<Guid, Entity>();

            var g1Id = new Guid("37c4bfbe-29f8-e611-810b-1458d041f8e8");
            var g1 = new Entity("tc_gateway", g1Id);
            g1["tc_gateway"] = "UK";

            gatewayEntityCollection.Add(g1Id, g1);
            #endregion Gateway

            #region Booking
            var bookingEntityCollection = new Dictionary<Guid, Entity>();

            var b1Id = Guid.NewGuid();
            var b1 = new Entity("tc_booking", b1Id);
            b1["tc_bookingid"] = b1Id;
            b1["tc_name"] = "b1";
            b1["ownerid"] = new EntityReference("systemuser", u1Id);
            b1["tc_departuredate"] = DateTime.Now.Date.AddDays(1);
            b1["tc_returndate"] = DateTime.Now.Date.AddDays(11);
            b1["tc_gatewayid"] = new EntityReference("tc_gateway", g1Id);

            bookingEntityCollection.Add(b1Id, b1);
            #endregion Booking

            #region Hotel
            var hotelEntityCollection = new Dictionary<Guid, Entity>();

            var hot1Id = Guid.NewGuid();
            var hot1 = new Entity("tc_hotel", hot1Id);
            hot1["ownerid"] = new EntityReference("team", t1Id);

            hotelEntityCollection.Add(hot1Id, hot1);
            #endregion Hotel



            #region accommodation
            var accommodationEntityCollection = new Dictionary<Guid, Entity>();
            var acc1Id = Guid.NewGuid();
            var acc1 = new Entity("tc_accommodation", acc1Id);
            acc1["tc_accommodationid"] = acc1Id;
            acc1["tc_startdateandtime"] = DateTime.Now.Date.AddDays(1);
            acc1["tc_enddateandtime"] = DateTime.Now.Date.AddDays(11);
            acc1["tc_bookingid"] = new EntityReference("tc_booking", b1Id);
            acc1["tc_hotelid"] = new EntityReference("tc_hotel", hot1Id);
            accommodationEntityCollection.Add(acc1Id, acc1);
            #endregion

            #region BookingRole
            var customerBookingRoleEntityCollection = new Dictionary<Guid, Entity>();

            var cbr1Id = Guid.NewGuid();
            var cbr1 = new Entity("tc_customerbookingrole");
            cbr1["tc_customerbookingrole"] = cbr1Id;
            cbr1["tc_bookingid"] = new EntityReference("tc_booking", b1Id);
            cbr1["tc_customer"] = new EntityReference("contact", con1Id);

            customerBookingRoleEntityCollection.Add(cbr1Id, cbr1);
            #endregion BookingRole

            context.Data.Add("systemuser", userEntityCollection);
            context.Data.Add("team", teamEntityCollection);
            context.Data.Add("contact", contactEntityCollection);
            context.Data.Add("tc_hotel", hotelEntityCollection);
            context.Data.Add("tc_booking", bookingEntityCollection);
            context.Data.Add("tc_accommodation", accommodationEntityCollection);
            context.Data.Add("tc_customerbookingrole", customerBookingRoleEntityCollection);

            var service = new AllocateResortTeamService(logger, allocationService, configurationService);
            service.Run();
            var bookingOwner = (EntityReference)(context.Data["tc_booking"][b1Id]["ownerid"]);
            var customerOwner = (EntityReference)(context.Data["contact"][con1Id]["ownerid"]);
            Assert.AreEqual(bookingOwner.Id, t1.Id);
            Assert.AreEqual(customerOwner.Id, t1.Id);
        }

        [TestMethod()]
        public void RunTest_Only_1_Accomodation_Current_Day_Minus_One()
        {
            crmService.Switch = DataSwitch.Returns_Data;
            var context = crmService.context;
            context.Data.Clear();
            //prepare data

            #region User
            var userEntityCollection = new Dictionary<Guid, Entity>();
            var u1Id = Guid.NewGuid();
            var u1 = new Entity("systemuser", u1Id);
            u1["systemuserid"] = u1Id;
            u1["firstname"] = "Pra";
            userEntityCollection.Add(u1Id, u1);
            #endregion User

            #region Team
            var teamEntityCollection = new Dictionary<Guid, Entity>();
            var t1Id = Guid.NewGuid();
            var t1 = new Entity("team", t1Id);
            t1["teamid"] = t1Id;
            t1["name"] = "Resort Team 1";

            var t2Id = Guid.NewGuid();
            var t2 = new Entity("team", t2Id);
            t2["teamid"] = t1Id;
            t2["name"] = "Resort Team 2";

            var t3Id = Guid.NewGuid();
            var t3 = new Entity("team", t3Id);
            t3["teamid"] = t1Id;
            t3["name"] = "Resort Team 3";

            teamEntityCollection.Add(t1Id, t1);
            teamEntityCollection.Add(t2Id, t2);
            teamEntityCollection.Add(t3Id, t3);
            #endregion Team

            #region contact
            var contactEntityCollection = new Dictionary<Guid, Entity>();
            var con1Id = Guid.NewGuid();
            var con1 = new Entity("contact", con1Id);
            con1["contactid"] = con1Id;
            con1["lastname"] = "Gogol";
            con1["firstname"] = "Nikolai";
            con1["ownerid"] = new EntityReference("systemuser", u1Id);
            contactEntityCollection.Add(con1Id, con1);
            #endregion contact

            #region Gateway
            var gatewayEntityCollection = new Dictionary<Guid, Entity>();

            var g1Id = new Guid("37c4bfbe-29f8-e611-810b-1458d041f8e8");
            var g1 = new Entity("tc_gateway", g1Id);
            g1["tc_gateway"] = "UK";

            gatewayEntityCollection.Add(g1Id, g1);
            #endregion Gateway

            #region Booking
            var bookingEntityCollection = new Dictionary<Guid, Entity>();

            var b1Id = Guid.NewGuid();
            var b1 = new Entity("tc_booking", b1Id);
            b1["tc_bookingid"] = b1Id;
            b1["tc_name"] = "b1";
            b1["ownerid"] = new EntityReference("systemuser", u1Id);
            b1["tc_departuredate"] = DateTime.Now.Date.AddDays(-1);
            b1["tc_returndate"] = DateTime.Now.Date.AddDays(11);
            b1["tc_gatewayid"] = new EntityReference("tc_gateway", g1Id);

            bookingEntityCollection.Add(b1Id, b1);
            #endregion Booking

            #region Hotel
            var hotelEntityCollection = new Dictionary<Guid, Entity>();

            var hot1Id = Guid.NewGuid();
            var hot1 = new Entity("tc_hotel", hot1Id);
            hot1["ownerid"] = new EntityReference("team", t1Id);

            hotelEntityCollection.Add(hot1Id, hot1);
            #endregion Hotel



            #region accommodation
            var accommodationEntityCollection = new Dictionary<Guid, Entity>();
            var acc1Id = Guid.NewGuid();
            var acc1 = new Entity("tc_accommodation", acc1Id);
            acc1["tc_accommodationid"] = acc1Id;
            acc1["tc_startdateandtime"] = DateTime.Now.Date.AddDays(-1);
            acc1["tc_enddateandtime"] = DateTime.Now.Date.AddDays(11);
            acc1["tc_bookingid"] = new EntityReference("tc_booking", b1Id);
            acc1["tc_hotelid"] = new EntityReference("tc_hotel", hot1Id);
            accommodationEntityCollection.Add(acc1Id, acc1);
            #endregion

            #region BookingRole
            var customerBookingRoleEntityCollection = new Dictionary<Guid, Entity>();

            var cbr1Id = Guid.NewGuid();
            var cbr1 = new Entity("tc_customerbookingrole");
            cbr1["tc_customerbookingrole"] = cbr1Id;
            cbr1["tc_bookingid"] = new EntityReference("tc_booking", b1Id);
            cbr1["tc_customer"] = new EntityReference("contact", con1Id);

            customerBookingRoleEntityCollection.Add(cbr1Id, cbr1);
            #endregion BookingRole

            context.Data.Add("systemuser", userEntityCollection);
            context.Data.Add("team", teamEntityCollection);
            context.Data.Add("contact", contactEntityCollection);
            context.Data.Add("tc_hotel", hotelEntityCollection);
            context.Data.Add("tc_booking", bookingEntityCollection);
            context.Data.Add("tc_accommodation", accommodationEntityCollection);
            context.Data.Add("tc_customerbookingrole", customerBookingRoleEntityCollection);

            var service = new AllocateResortTeamService(logger, allocationService, configurationService);
            service.Run();
            var bookingOwner = (EntityReference)(context.Data["tc_booking"][b1Id]["ownerid"]);
            var customerOwner = (EntityReference)(context.Data["contact"][con1Id]["ownerid"]);
            Assert.AreEqual(bookingOwner.Id, u1.Id);
            Assert.AreEqual(customerOwner.Id, u1.Id);
        }
    }
}