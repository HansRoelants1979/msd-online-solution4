using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using FakeXrmEasy;
using System.Collections.Generic;
using System.Linq;

namespace Tc.Crm.UnitTests.Plugins
{
    [TestClass]
    public class PreCaseLine
    {
        private Entity caseLineEntity = null;
        private Entity incidentEntity = null;
        private Entity bookingEntity = null;
        private Entity bookingAccommodationEntity = null;
        private Entity hotelEntity = null;
        private Entity hotelPromiseEntity = null;
        private Entity brandEntity;

        DateTime currentUTCTime = DateTime.UtcNow;
        XrmFakedContext context = null;

        [TestInitialize]
        public void Initialise()
        {
            //Create a new Booking dummy object  
            brandEntity = new Entity("tc_booking", Guid.NewGuid());
            brandEntity.Attributes.Add("tc_name", "Thomas Cook DE");

            //Create a new Hotel dummy object
            hotelEntity = new Entity("tc_hotel", Guid.NewGuid());
            hotelEntity.Attributes.Add("tc_name", "Steigenberger Aqua Magic");

            //Create a new Hotel Promise dummy object
            hotelPromiseEntity = new Entity("tc_hotelpromises", Guid.NewGuid());
            hotelPromiseEntity.Attributes.Add("tc_promisetype", new OptionSetValue(950000000));
            hotelPromiseEntity.Attributes.Add("tc_brandid", brandEntity.ToEntityReference());
            hotelPromiseEntity.Attributes.Add("tc_name", "Steigenberger Aqua Magic - Thomas Cook UK - 24 Hour");
            hotelPromiseEntity.Attributes.Add("tc_hotelid", hotelEntity.ToEntityReference());

            //Create a new Booking dummy object  
            bookingEntity = new Entity("tc_booking", Guid.NewGuid());
            bookingEntity.Attributes.Add("tc_name", "20170405153");
            bookingEntity.Attributes.Add("tc_brandid", brandEntity.ToEntityReference());

            //Create a new Booking Accommodation dummy object
            bookingAccommodationEntity = new Entity("tc_bookingaccommodation", Guid.NewGuid());
            bookingAccommodationEntity.Attributes.Add("tc_startdateandtime", currentUTCTime.AddDays(5));
            bookingAccommodationEntity.Attributes.Add("tc_hotelid", hotelEntity.ToEntityReference());
            bookingAccommodationEntity.Attributes.Add("tc_name", "Steigenberger Aqua Magic - 20170405153");
            bookingAccommodationEntity.Attributes.Add("tc_bookingid", bookingEntity.ToEntityReference());

            //Create a new Case dummy object            
            incidentEntity = new Entity("incident", Guid.NewGuid());
            incidentEntity.Attributes.Add("ticketnumber", "TC-00539-B7R7");
            incidentEntity.Attributes.Add("createdon", currentUTCTime);
            incidentEntity.Attributes.Add("tc_bookingid", bookingEntity.ToEntityReference());
            incidentEntity.Attributes.Add("tc_bookingreference", true);
            incidentEntity.Attributes.Add("tc_24hourpromise", new OptionSetValue(950000001));

            //Create a new Case Line dummy object
            caseLineEntity = new Entity("tc_caseline", Guid.NewGuid());
            caseLineEntity.Attributes.Add("tc_caseid", incidentEntity.ToEntityReference());
            caseLineEntity.Attributes.Add("tc_servicetype", new OptionSetValue(950000000));
            caseLineEntity.Attributes.Add("tc_name", "20170405153");

            context = new XrmFakedContext();
            context.Initialize(new List<Entity>() { brandEntity, hotelEntity, hotelPromiseEntity, bookingEntity,
                bookingAccommodationEntity,incidentEntity, caseLineEntity});
        }        

        [TestMethod]
        public void PreCaseLinePlugin_CaseLineServiceTypeNotEqualToAccommodation()
        {
            context.CreateQueryFromEntityName(caseLineEntity.LogicalName).FirstOrDefault().Attributes["tc_servicetype"] = new OptionSetValue(950000001);

            var target = context.CreateQueryFromEntityName(caseLineEntity.LogicalName).FirstOrDefault();

            var fakedPlugin = context.ExecutePluginWithTarget<Crm.Plugins.PreCaseLine>(target);

            var incident = context.CreateQueryFromEntityName(incidentEntity.LogicalName).FirstOrDefault();

            Assert.IsTrue((incident.Attributes["tc_24hourpromise"] as OptionSetValue).Value != 950000000);
            Assert.IsTrue(context.GetFakeTracingService().DumpTrace().Contains(string.Format("Case Line Service Type is not equal to 'Accommodation'")));

            context.CreateQueryFromEntityName(caseLineEntity.LogicalName).FirstOrDefault().Attributes["tc_servicetype"] = new OptionSetValue(950000000);
        }

        [TestMethod]
        public void PreCaseLinePlugin_CaseLineCaseIdReferenceIsNull()
        {
            context.CreateQueryFromEntityName(caseLineEntity.LogicalName).FirstOrDefault().Attributes["tc_caseid"] = null;

            var target = context.CreateQueryFromEntityName(caseLineEntity.LogicalName).FirstOrDefault();
            var fakedPlugin = context.ExecutePluginWithTarget<Crm.Plugins.PreCaseLine>(target);

            var incident = context.CreateQueryFromEntityName(incidentEntity.LogicalName).FirstOrDefault();
            Assert.IsTrue((incident.Attributes["tc_24hourpromise"] as OptionSetValue).Value != 950000000);
            Assert.IsTrue(context.GetFakeTracingService().DumpTrace().Contains(string.Format("No Case / Service Type / Case Line Name is specified.")));

            context.CreateQueryFromEntityName(caseLineEntity.LogicalName).FirstOrDefault().Attributes["tc_caseid"] = incidentEntity.ToEntityReference();
        }

        [TestMethod]
        public void PreCaseLinePlugin_Case24HoursIsNotEqualToNo()
        {
            context.CreateQueryFromEntityName(incidentEntity.LogicalName).FirstOrDefault().Attributes["tc_24hourpromise"] = new OptionSetValue(950000002);

            var target = context.CreateQueryFromEntityName(caseLineEntity.LogicalName).FirstOrDefault();
            var fakedPlugin = context.ExecutePluginWithTarget<Crm.Plugins.PreCaseLine>(target);

            var incident = context.CreateQueryFromEntityName(incidentEntity.LogicalName).FirstOrDefault();
            Assert.IsTrue((incident.Attributes["tc_24hourpromise"] as OptionSetValue).Value != 950000000);
            Assert.IsTrue(context.GetFakeTracingService().DumpTrace().Contains(string.Format("Case's Booking Reference field is not set to 'Yes' or 24 Hours Promise field is not equal to 'No'")));

            context.CreateQueryFromEntityName(incidentEntity.LogicalName).FirstOrDefault().Attributes["tc_24hourpromise"] = new OptionSetValue(950000001);
        }

        [TestMethod]
        public void PreCaseLinePlugin_CasebookingEntityReferenceIsNull()
        {
            context.CreateQueryFromEntityName(incidentEntity.LogicalName).FirstOrDefault().Attributes["tc_bookingid"] = null;

            var target = caseLineEntity;
            var fakedPlugin = context.ExecutePluginWithTarget<Crm.Plugins.PreCaseLine>(target);

            var incident = context.CreateQueryFromEntityName(incidentEntity.LogicalName).FirstOrDefault();

            Assert.IsTrue((incident.Attributes["tc_24hourpromise"] as OptionSetValue).Value != 950000000);
            Assert.IsTrue(context.GetFakeTracingService().DumpTrace().Contains(string.Format("No Booking associated with Case")));

            context.CreateQueryFromEntityName(incidentEntity.LogicalName).FirstOrDefault().Attributes["tc_bookingid"] = bookingEntity.ToEntityReference();
        }

        [TestMethod]
        public void PreCaseLinePlugin_BookingAccommodationIsNull()
        {
            context.CreateQueryFromEntityName(bookingAccommodationEntity.LogicalName).FirstOrDefault().Attributes["tc_bookingid"] = null;

            var target = caseLineEntity;
            var fakedPlugin = context.ExecutePluginWithTarget<Crm.Plugins.PreCaseLine>(target);

            var incident = context.CreateQueryFromEntityName(incidentEntity.LogicalName).FirstOrDefault();

            Assert.IsTrue((incident.Attributes["tc_24hourpromise"] as OptionSetValue).Value != 950000000);
            Assert.IsTrue(context.GetFakeTracingService().DumpTrace().Contains(string.Format("No Booking Accommodation associated with Case Booking")));

            context.CreateQueryFromEntityName(bookingAccommodationEntity.LogicalName).FirstOrDefault().Attributes["tc_bookingid"] = bookingEntity.ToEntityReference();
        }

        [TestMethod]
        public void PreCaseLinePlugin_HotelPromiseTypeIsNull()
        {
            context.CreateQueryFromEntityName(hotelPromiseEntity.LogicalName).FirstOrDefault().Attributes["tc_hotelid"] = null;

            var target = caseLineEntity;
            var fakedPlugin = context.ExecutePluginWithTarget<Crm.Plugins.PreCaseLine>(target);

            var incident = context.CreateQueryFromEntityName(incidentEntity.LogicalName).FirstOrDefault();
            Assert.IsTrue((incident.Attributes["tc_24hourpromise"] as OptionSetValue).Value != 950000000);
            Assert.IsTrue(context.GetFakeTracingService().DumpTrace().Contains(string.Format("No Booking Accommodation associated with Case Booking")));

            context.CreateQueryFromEntityName(hotelPromiseEntity.LogicalName).FirstOrDefault().Attributes["tc_hotelid"] = hotelEntity.ToEntityReference();
        }

        [TestMethod]
        public void PreCaseLinePlugin_HotelPromiseTypeValueNotEqualTo24Hours()
        {
            context.CreateQueryFromEntityName(hotelPromiseEntity.LogicalName).FirstOrDefault().Attributes["tc_promisetype"] = new OptionSetValue(950000001);

            var target = caseLineEntity;
            var fakedPlugin = context.ExecutePluginWithTarget<Crm.Plugins.PreCaseLine>(target);


            var incident = context.CreateQueryFromEntityName(incidentEntity.LogicalName).FirstOrDefault();
            Assert.IsTrue((incident.Attributes["tc_24hourpromise"] as OptionSetValue).Value != 950000000);
            Assert.IsTrue(context.GetFakeTracingService().DumpTrace().Contains(string.Format("No Hotel Promise Type set or not equal to '24 Hours'")));

            context.CreateQueryFromEntityName(hotelPromiseEntity.LogicalName).FirstOrDefault().Attributes["tc_promisetype"] = new OptionSetValue(950000000);
        }

        [TestMethod]
        public void PreCaseLinePlugin_BookingAccommodationStartDateTimeLessThanCaseCreatedOnDate()
        {
            context.CreateQueryFromEntityName(incidentEntity.LogicalName).FirstOrDefault().Attributes["createdon"] = currentUTCTime.AddDays(6).AddMinutes(1); //BookingAccommodation --> StartDate + 24 Hours + 1 Min

            var target = caseLineEntity;
            var fakedPlugin = context.ExecutePluginWithTarget<Crm.Plugins.PreCaseLine>(target);


            var incident = context.CreateQueryFromEntityName(incidentEntity.LogicalName).FirstOrDefault();
            Assert.IsTrue((incident.Attributes["tc_24hourpromise"] as OptionSetValue).Value != 950000000);
            Assert.IsTrue(context.GetFakeTracingService().DumpTrace().Contains(string.Format("No Booking Accommodation Start Date or it's less than the Case CreatedOn.")));

            context.CreateQueryFromEntityName(incidentEntity.LogicalName).FirstOrDefault().Attributes["createdon"] = currentUTCTime;
        }

        [TestMethod]
        public void PreCaseLinePlugin_BookingAccommodationStartDateTimeEqualToCaseCreatedOnDate()
        {
            context.CreateQueryFromEntityName(incidentEntity.LogicalName).FirstOrDefault().Attributes["createdon"] = currentUTCTime.AddDays(5); //Same as BookingAccommodation --> StartDate 

            var target = caseLineEntity;
            var fakedPlugin = context.ExecutePluginWithTarget<Crm.Plugins.PreCaseLine>(target);


            var incident = context.CreateQueryFromEntityName(incidentEntity.LogicalName).FirstOrDefault();
            Assert.IsTrue((incident.Attributes["tc_24hourpromise"] as OptionSetValue).Value == 950000000);  // CaseLine.24hourpromise != 'Yes'
            Assert.IsTrue(context.GetFakeTracingService().DumpTrace().Contains(string.Format("Successfully 24 Hours Promise on the Case record has been set")));

            context.CreateQueryFromEntityName(incidentEntity.LogicalName).FirstOrDefault().Attributes["createdon"] = DateTime.UtcNow;
        }

        [TestMethod]
        public void PreCaseLinePlugin_BookingAccommodationStartDateTimeGreaterThanCaseCreatedOnDate()
        {
            context.CreateQueryFromEntityName(incidentEntity.LogicalName).FirstOrDefault().Attributes["createdon"] = DateTime.UtcNow;

            var target = caseLineEntity;
            var fakedPlugin = context.ExecutePluginWithTarget<Crm.Plugins.PreCaseLine>(target);


            var incident = context.CreateQueryFromEntityName(incidentEntity.LogicalName).FirstOrDefault();
            Assert.IsTrue((incident.Attributes["tc_24hourpromise"] as OptionSetValue).Value == 950000000);  // CaseLine.24hourpromise != 'Yes'
            Assert.IsTrue(context.GetFakeTracingService().DumpTrace().Contains(string.Format("Successfully 24 Hours Promise on the Case record has been set")));

            context.CreateQueryFromEntityName(incidentEntity.LogicalName).FirstOrDefault().Attributes["createdon"] = DateTime.UtcNow;
        }
    }
}