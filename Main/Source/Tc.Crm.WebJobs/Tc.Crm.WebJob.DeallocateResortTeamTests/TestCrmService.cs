using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;
using Tc.Crm.Common.Models;
using Tc.Crm.Common.Services;

namespace Tc.Crm.WebJob.DeallocateResortTeamTests
{
    public enum DataSwitch
    {
        No_Records_Returned = 0,
        Collection_With_No_Records_Returned = 1,
        Returns_Data = 2
    }
    public class TestCrmService : ICrmService
    {
        public DataSwitch Switch { get; set; }
        public XrmFakedContext context;
        public IOrganizationService orgService;

        public TestCrmService()
        {
            context = new XrmFakedContext();
            orgService = GetOrganizationService();
        }
        public void BulkAssign(Collection<AssignInformation> assignRequests)
        {
            foreach (var ar in assignRequests)
            {
                if (ar.EntityName == "tc_booking")
                {
                    if (ar.RecordOwner.OwnerType == Common.OwnerType.Team)
                        context.Data["tc_booking"][ar.RecordId]["ownerid"] = new EntityReference("team", ar.RecordOwner.Id);
                    if (ar.RecordOwner.OwnerType == Common.OwnerType.User)
                        context.Data["tc_booking"][ar.RecordId]["ownerid"] = new EntityReference("systemuser", ar.RecordOwner.Id);
                }
                if (ar.EntityName == "Contact")
                {
                    if (ar.RecordOwner.OwnerType == Common.OwnerType.Team)
                        context.Data["contact"][ar.RecordId]["ownerid"] = new EntityReference("team", ar.RecordOwner.Id);
                    if (ar.RecordOwner.OwnerType == Common.OwnerType.User)
                        context.Data["contact"][ar.RecordId]["ownerid"] = new EntityReference("systemuser", ar.RecordOwner.Id);
                }
            }
        }

        public string CreateXml(XmlDocument doc, string cookie, int page, int count)
        {
            throw new NotImplementedException();
        }

        public string CreateXml(string xml, string cookie, int page, int count)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Microsoft.Xrm.Sdk.IOrganizationService GetOrganizationService()
        {

            return context.GetFakedOrganizationService();
        }

        public Microsoft.Xrm.Sdk.EntityCollection GetRecordsUsingQuery(Microsoft.Xrm.Sdk.Query.QueryExpression queryExpression)
        {
            throw new NotImplementedException();
        }

        public Microsoft.Xrm.Sdk.EntityCollection RetrieveMultipleRecords(string entityName, string[] columns, string[] filterKeys, string[] filterValues)
        {
            if (Switch == DataSwitch.No_Records_Returned)
                return null;
            if (Switch == DataSwitch.Collection_With_No_Records_Returned)
                return new Microsoft.Xrm.Sdk.EntityCollection();

            return null;
        }

        public Microsoft.Xrm.Sdk.EntityCollection RetrieveMultipleRecordsFetchXml(string query)
        {
            if (Switch == DataSwitch.No_Records_Returned)
                return null;
            if (Switch == DataSwitch.Collection_With_No_Records_Returned)
                return new Microsoft.Xrm.Sdk.EntityCollection();

            if (Switch == DataSwitch.Returns_Data)
            {
                return GetBookings(context.Data["tc_booking"].Values.ToList<Entity>()
                                    , context.Data["tc_accommodation"].Values.ToList<Entity>()
                                    , context.Data["tc_hotel"].Values.ToList<Entity>()
                                    , context.Data["tc_customerbookingrole"].Values.ToList<Entity>());
            }

            return null;
        }

        private Entity GetBooking(Entity b)
        {
            var e = new Entity(b.LogicalName, b.Id);
            e["tc_departuredate"] = b["tc_departuredate"];
            e["tc_returndate"] = b["tc_returndate"];
            e["tc_name"] = b["tc_name"];
            e["tc_bookingid"] = b["tc_bookingid"];
            e["ownerid"] = b["ownerid"];
            e["tc_gatewayid"] = b["tc_gatewayid"];
            return e;
        }

        private EntityCollection GetBookings(List<Entity> bookingCollection
                                                        , List<Entity> accommodations
                                                        , List<Entity> hotels
                                                        , List<Entity> customerBookingRoles)
        {
            var bookings = new List<Entity>();
            var bookings1 = (from b in bookingCollection
                            join a in accommodations on b.Id equals ((EntityReference)a["tc_bookingid"]).Id
                            join h in hotels on ((EntityReference)a["tc_hotelid"]).Id equals h.Id
                            join cbr in customerBookingRoles on b.Id equals ((EntityReference)cbr["tc_bookingid"]).Id
                            where ((DateTime)b["tc_departuredate"]) <= DateTime.Now.Date.AddDays(9)
                            && ((DateTime)b["tc_returndate"]) >= DateTime.Now.Date
                            orderby b["tc_name"].ToString(), ((DateTime)a["tc_startdateandtime"])
                            select new
                            {
                                BookingId = b.Id,
                                Name = b["tc_name"].ToString(),
                                OwnerId = ((EntityReference)b["ownerid"]),
                                DepartureDate = ((DateTime)b["tc_departuredate"]),
                                ReturnDate = ((DateTime)b["tc_returndate"]),
                                AccommodationStart = ((DateTime)a["tc_startdateandtime"]),
                                AccommodationEnd = ((DateTime)a["tc_enddateandtime"]),
                                HotelOwner = ((EntityReference)h["ownerid"]),
                                Customer = ((EntityReference)cbr["tc_customer"])
                            }).ToList();

            
            foreach (var item in bookings1)
            {
                var b = new Entity("tc_booking",item.BookingId);
                b["tc_departuredate"] = item.DepartureDate;
                b["tc_returndate"] = item.ReturnDate;
                b["tc_name"] = item.Name;
                b["tc_bookingid"] = item.BookingId;
                b["ownerid"] = item.OwnerId;
                b["accommodation.tc_startdateandtime"] = new AliasedValue("tc_accommodation", "tc_startdateandtime", item.AccommodationStart);
                b["accommodation.tc_enddateandtime"] = new AliasedValue("tc_accommodation", "tc_enddateandtime", item.AccommodationEnd);
                b["hotel.ownerid"] = new AliasedValue("tc_hotel", "ownerid", item.HotelOwner);
                b["role.tc_customer"] = new AliasedValue("tc_customerbookingrole", "tc_customer", item.Customer);
                bookings.Add(b);
            }


                 
            //foreach (var booking in bookingCollection)
            //{
            //    var bookingStartDate = (DateTime)booking["tc_departuredate"] ;
            //    if (bookingStartDate > DateTime.Now.Date.AddDays(9)) continue;
            //    var bookingEndDate = (DateTime)booking["tc_returndate"];
            //    if (bookingEndDate < DateTime.Now.Date) continue;

            //    foreach (var a in accommodations)
            //    {
            //        var b = GetBooking(booking);
            //        if (b.Id == ((EntityReference)a["tc_bookingid"]).Id)
            //        {
            //            b["accommodation.tc_startdateandtime"] = new AliasedValue("tc_accommodation", "tc_startdateandtime", a["tc_startdateandtime"]);
            //            b["accommodation.tc_enddateandtime"] = new AliasedValue("tc_accommodation", "tc_enddateandtime", a["tc_enddateandtime"]);
            //            var hotel = hotels.Find(h => h.Id == ((EntityReference)a["tc_hotelid"]).Id);
            //            b["hotel.ownerid"] = new AliasedValue("tc_hotel", "ownerid", hotel["ownerid"]);
            //            var customer = customerBookingRoles.Find(cbr => ((EntityReference)cbr["tc_bookingid"]).Id == b.Id);
            //            b["role.tc_customer"] = new AliasedValue("tc_customerbookingrole", "tc_customer", customer["tc_customer"]);
            //            bookings.Add(b);
            //        }
            //    }
            //}
            return new EntityCollection(bookings);

        }
    }
}
