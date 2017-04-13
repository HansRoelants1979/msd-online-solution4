using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Tc.Crm.Common.Models;
using Tc.Crm.Common.Services;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;

namespace Tc.Crm.WebJob.AllocateResortTeamTests
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
        public List<Guid> validGatewayIds;

        public TestCrmService()
        {
            context = new XrmFakedContext();
            validGatewayIds = new List<Guid>();
            PrepareData();
        }

        public void AddUsersToContext()
        {
            var userEntityCollection = new Dictionary<Guid, Entity>();
            var user1Id = Guid.NewGuid();
            var user1 = new Entity("systemuser", user1Id);
            user1["systemuserid"] = user1Id;
            user1["firstname"] = "Pra";
            userEntityCollection.Add(user1Id, user1);

            context.Data.Add("systemuser", userEntityCollection);
        }

        public void AddResortTeamsToContext()
        {
            var teamEntityCollection = new Dictionary<Guid, Entity>();
            var resortTeam1Id = Guid.NewGuid();
            var resortTeam1 = new Entity("team", resortTeam1Id);
            resortTeam1["teamid"] = resortTeam1Id;
            resortTeam1["name"] = "Resort Team 1";

            var resortTeam2Id = Guid.NewGuid();
            var resortTeam2 = new Entity("team", resortTeam2Id);
            resortTeam2["teamid"] = resortTeam1Id;
            resortTeam2["name"] = "Resort Team 2";

            var resortTeam3Id = Guid.NewGuid();
            var resortTeam3 = new Entity("team", resortTeam3Id);
            resortTeam3["teamid"] = resortTeam1Id;
            resortTeam3["name"] = "Resort Team 3";

            teamEntityCollection.Add(resortTeam1Id, resortTeam1);
            teamEntityCollection.Add(resortTeam2Id, resortTeam2);
            teamEntityCollection.Add(resortTeam3Id, resortTeam3);

            context.Data.Add("team", teamEntityCollection);
        }

        public void AddCustomersToContext()
        {
            var contactEntityCollection = new Dictionary<Guid, Entity>();
            var customer1Id = Guid.NewGuid();
            var customer1 = new Entity("contact", customer1Id);
            customer1["contactid"] = customer1Id;
            customer1["lastname"] = "Gogol";
            customer1["firstname"] = "Nikolai";
            contactEntityCollection.Add(customer1Id, customer1);

            context.Data.Add("contact", contactEntityCollection);
        }

        public void AddGatewaysToContext()
        {
            var gatewayEntityCollection = new Dictionary<Guid, Entity>();

            var configurationService = new TestConfigurationService();
            var gatewayIdsString = configurationService.DestinationGatewayIds;
            var gatewayIds = gatewayIdsString.Split(',');
            for (int i = 0; i < gatewayIds.Length; i++)
            {
                var gatewayId = new Guid(gatewayIds[i]);
                var gateway = new Entity("tc_gateway", gatewayId);
                gateway["tc_gateway"] = "gateway_" + i.ToString();
                gatewayEntityCollection.Add(gatewayId, gateway);
                validGatewayIds.Add(gatewayId);
            }

            //invalid gateway
            var invalidGatewayId = Guid.NewGuid();
            var invalidGateway = new Entity("tc_gateway", invalidGatewayId);
            invalidGateway["tc_gateway"] = "invalid gateway";
            gatewayEntityCollection.Add(invalidGatewayId, invalidGateway);


            context.Data.Add("tc_gateway", gatewayEntityCollection);
        }

        public void AddBookingsToContext()
        {

            var bookingEntityCollection = new Dictionary<Guid, Entity>();

            var booking1Id = Guid.NewGuid();
            var booking1 = new Entity("tc_booking", booking1Id);
            booking1["tc_bookingid"] = booking1Id;
            booking1["tc_name"] = "b1";
            booking1["tc_departuredate"] = DateTime.Now.Date;
            booking1["tc_returndate"] = DateTime.Now.Date;

            bookingEntityCollection.Add(booking1Id, booking1);

            context.Data.Add("tc_booking", bookingEntityCollection);
        }

        public void AddHotelsToContext()
        {
            var hotelEntityCollection = new Dictionary<Guid, Entity>();

            var jwMariotId = Guid.NewGuid();
            var jwMariot = new Entity("tc_hotel", jwMariotId);

            hotelEntityCollection.Add(jwMariotId, jwMariot);

            context.Data.Add("tc_hotel", hotelEntityCollection);
        }

        public void AddAccomodationsToContext()
        {
            var accommodationEntityCollection = new Dictionary<Guid, Entity>();
            var accommodation1Id = Guid.NewGuid();
            var accommodation1 = new Entity("tc_bookingaccommodation", accommodation1Id);
            accommodation1["tc_bookingaccommodationid"] = accommodation1Id;
            accommodation1["tc_startdateandtime"] = DateTime.Now.Date;
            accommodation1["tc_enddateandtime"] = DateTime.Now.Date;

            accommodationEntityCollection.Add(accommodation1Id, accommodation1);

            context.Data.Add("tc_bookingaccommodation", accommodationEntityCollection);
        }

        public void AddBookingRolesToContext()
        {
            var customerBookingRoleEntityCollection = new Dictionary<Guid, Entity>();

            var customerBookingRole1Id = Guid.NewGuid();
            var customerBookingRole1 = new Entity("tc_customerbookingrole", customerBookingRole1Id);
            customerBookingRole1["tc_customerbookingroleid"] = customerBookingRole1Id;

            customerBookingRoleEntityCollection.Add(customerBookingRole1Id, customerBookingRole1);

            context.Data.Add("tc_customerbookingrole", customerBookingRoleEntityCollection);
        }

        public void PrepareData()
        {
            context.Data.Clear();
            AddUsersToContext();
            AddResortTeamsToContext();
            AddCustomersToContext();
            AddGatewaysToContext();
            AddBookingsToContext();
            AddHotelsToContext();
            AddAccomodationsToContext();
            AddBookingRolesToContext();
        }

        public void SetAccommodation(Entity accommodation,int startOffset,int endOffset, Guid bookingId, Guid hotelId)
        {
            accommodation["tc_startdateandtime"] = DateTime.Now.Date.AddDays(startOffset);
            accommodation["tc_enddateandtime"] = DateTime.Now.Date.AddDays(endOffset);
            accommodation["tc_bookingid"] = new EntityReference("tc_booking", bookingId);
            accommodation["tc_hotelid"] = new EntityReference("tc_hotel", hotelId);
        }

        public void AddAccommodation()
        {
            var accommodation1Id = Guid.NewGuid();
            var accommodation1 = new Entity("tc_bookingaccommodation", accommodation1Id);
            accommodation1["tc_bookingaccommodationid"] = accommodation1Id;
            context.Data["tc_bookingaccommodation"].Add(accommodation1Id, accommodation1);
        }

        public void AddHotel()
        {
            var tajId = Guid.NewGuid();
            var taj = new Entity("tc_hotel", tajId);
            context.Data["tc_hotel"].Add(tajId, taj);
        }

        public void AddCustomer()
        {
            var customer1Id = Guid.NewGuid();
            var customer1 = new Entity("contact", customer1Id);
            customer1["contactid"] = customer1Id;
            customer1["lastname"] = "Purple";
            customer1["firstname"] = "Nikolai";
            context.Data["contact"].Add(customer1Id, customer1);
        }

        public void AddCustomerBookingRole()
        {
            var customerBookingRole1Id = Guid.NewGuid();
            var customerBookingRole1 = new Entity("tc_customerbookingrole", customerBookingRole1Id);
            customerBookingRole1["tc_customerbookingroleid"] = customerBookingRole1Id;

            context.Data["tc_customerbookingrole"].Add( customerBookingRole1Id, customerBookingRole1);
        }

        public void AddBooking()
        {
            var booking1Id = Guid.NewGuid();
            var booking1 = new Entity("tc_booking", booking1Id);
            booking1["tc_bookingid"] = booking1Id;
            booking1["tc_name"] = "b1";
            booking1["tc_departuredate"] = DateTime.Now.Date;
            booking1["tc_returndate"] = DateTime.Now.Date;

            context.Data["tc_booking"].Add(booking1Id, booking1);
        }

        public void SetBooking(Entity booking, int startOffset, int endOffset,Guid gatewayId, EntityReference owner)
        {
            booking["tc_destinationgatewayid"] = new EntityReference("tc_gateway", gatewayId);
            booking["ownerid"] = owner;
            booking["tc_departuredate"] = DateTime.Now.Date.AddDays(startOffset);
            booking["tc_returndate"] = DateTime.Now.Date.AddDays(endOffset);
        }

        public void SetCustomerBookingRole(Entity customerBookingRole,EntityReference contact,EntityReference booking)
        {
            customerBookingRole["tc_customer"] = contact;
            customerBookingRole["tc_bookingid"] = booking;
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

        public void BulkUpdate(IEnumerable<Entity> entities)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Microsoft.Xrm.Sdk.EntityCollection GetRecordsUsingQuery(Microsoft.Xrm.Sdk.Query.QueryExpression queryExpression)
        {
            throw new NotImplementedException();
        }

        public Microsoft.Xrm.Sdk.EntityCollection RetrieveMultipleRecords(string entityName, string[] columns, string[] filterKeys, string[] filterValues)
        {
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
                                    , context.Data["tc_bookingaccommodation"].Values.ToList<Entity>()
                                    , context.Data["tc_hotel"].Values.ToList<Entity>()
                                    , context.Data["tc_customerbookingrole"].Values.ToList<Entity>()
                                    , context.Data["tc_gateway"].Values.ToList<Entity>()
                                    , context.Data["contact"].Values.ToList<Entity>());
            }

            return null;
        }

        private EntityCollection GetBookings(List<Entity> bookingCollection
                                                        , List<Entity> accommodations
                                                        , List<Entity> hotels
                                                        , List<Entity> customerBookingRoles
                                                        , List<Entity> gateways
                                                        , List<Entity> contacts)
        {
            var bookings = new List<Entity>();
            var bookings1 = (from b in bookingCollection
                             join a in accommodations on b.Id equals ((EntityReference)a["tc_bookingid"]).Id
                             join h in hotels on ((EntityReference)a["tc_hotelid"]).Id equals h.Id
                             join cbr in customerBookingRoles on b.Id equals ((EntityReference)cbr["tc_bookingid"]).Id
                             join g in gateways on ((EntityReference)b["tc_destinationgatewayid"]).Id equals g.Id
                             join c in contacts on ((EntityReference)cbr["tc_customer"]).Id equals c.Id
                             where ((DateTime)b["tc_departuredate"]) <= DateTime.Now.Date.AddDays(9)
                             && ((DateTime)b["tc_returndate"]) >= DateTime.Now.Date
                             && validGatewayIds.Contains(((EntityReference)b["tc_destinationgatewayid"]).Id)
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
                                 Customer = ((EntityReference)cbr["tc_customer"]),
                                 CustomerOwner = ((EntityReference)c["ownerid"])
                             }).ToList();


            foreach (var item in bookings1)
            {
                var b = new Entity("tc_booking", item.BookingId);
                b["tc_departuredate"] = item.DepartureDate;
                b["tc_returndate"] = item.ReturnDate;
                b["tc_name"] = item.Name;
                b["tc_bookingid"] = item.BookingId;
                b["ownerid"] = item.OwnerId;
                b["accommodation.tc_startdateandtime"] = new AliasedValue("tc_bookingaccommodation", "tc_startdateandtime", item.AccommodationStart);
                b["accommodation.tc_enddateandtime"] = new AliasedValue("tc_bookingaccommodation", "tc_enddateandtime", item.AccommodationEnd);
                b["hotel.ownerid"] = new AliasedValue("tc_hotel", "ownerid", item.HotelOwner);
                b["role.tc_customer"] = new AliasedValue("tc_customerbookingrole", "tc_customer", item.Customer);
                b["contact.ownerid"] = new AliasedValue("contact", "ownerid", item.CustomerOwner);
                bookings.Add(b);
            }

            
            return new EntityCollection(bookings);

        }

        public void ExecuteBulkAssignRequests(ExecuteMultipleRequest request)
        {
            throw new NotImplementedException();
        }

        public string FormatFaultException(AssignRequest assignRequest, OrganizationServiceFault fault)
        {
            throw new NotImplementedException();
        }
    }
}
