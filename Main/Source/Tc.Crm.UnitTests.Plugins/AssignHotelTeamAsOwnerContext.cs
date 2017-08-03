using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;

namespace Tc.Crm.UnitTests.Plugins
{
    public class AssignHotelTeamAsOwnerContext
    {
        XrmFakedContext context = null;
        public XrmFakedContext InitialiseContext(Guid caseId, Guid userId, Guid? roleId = null, Guid? customerId = null, Guid? businessUnitId = null, Guid? bookingId = null)
        {
            context = new XrmFakedContext();
            var entities = new List<Entity>();

            if (caseId != Guid.Empty)
                entities.AddRange(InitialiseCase(caseId, userId, customerId, bookingId));

            if (roleId.HasValue)
                entities.AddRange(InitialiseRoles(userId, roleId.Value));

            if (customerId.HasValue && businessUnitId.HasValue)
                entities.AddRange(InitialiseCustomerSourceMarket(customerId.Value, businessUnitId.Value, userId));

            if (bookingId.HasValue && businessUnitId.HasValue)
                entities.AddRange(InitialiseBookingHotelTeam(bookingId.Value, businessUnitId.Value));

            if (businessUnitId.HasValue)
                entities.AddRange(InitialiseUserHotelTeam(userId, businessUnitId.Value));

            if (entities.Count > 0)
                context.Initialize(entities);

            return context;
        }

        public List<Entity> InitialiseCase(Guid caseId, Guid initiatingUserId, Guid? customerId = null, Guid? bookingId = null)
        {
            var entCase = new Entity("incident", caseId);
            entCase.Attributes["incidentid"] = caseId;
            if (customerId.HasValue)
                entCase.Attributes["customerid"] = new EntityReference("contact", customerId.Value);
            entCase.Attributes["ownerid"] = new EntityReference("systemuser", initiatingUserId);
            if (bookingId.HasValue)
                entCase.Attributes["tc_bookingid"] = new EntityReference("tc_booking", bookingId.Value);

            return new List<Entity>() { entCase };
        }

        public List<Entity> InitialiseRoles(Guid userId, Guid roleId)
        {
            var role = new Entity("role", roleId);
            role.Attributes["roleid"] = roleId;
            role.Attributes["name"] = "Tc.Ids.Rep";

            var userRoles = new Entity("systemuserroles", Guid.NewGuid());
            userRoles.Attributes["systemuserid"] = userId;
            userRoles.Attributes["roleid"] = new EntityReference("role", roleId);

            var user = new Entity("systemuser", userId);
            user.Attributes["systemuserid"] = new EntityReference("systemuser", userId);

            return new List<Entity>() { role, userRoles, user };
        }

        public List<Entity> InitialiseCustomerSourceMarket(Guid customerId, Guid businessUnitId, Guid userId)
        {
            var countryId = Guid.NewGuid();
            var country = new Entity("tc_country", countryId);
            country.Attributes["tc_sourcemarketbusinessunitid"] = new EntityReference("businessunit", businessUnitId);
            country.Attributes["tc_countryid"] = countryId;

            var contact = new Entity("contact", customerId);
            contact.Attributes["contactid"] = customerId;
            contact.Attributes["ownerid"] = new EntityReference("systemuser", userId);
            contact.Attributes["tc_sourcemarketid"] = new EntityReference("tc_country", countryId);

            return new List<Entity>() { contact, country };

        }

        public List<Entity> InitialiseBookingHotelTeam(Guid bookingId, Guid businessUnitId)
        {
            var teamId = Guid.NewGuid();
            var team = new Entity("team", teamId);
            team.Attributes["teamid"] = teamId;

            var hotelTeamId = Guid.NewGuid();
            var hotelTeam = new Entity("team", hotelTeamId);
            hotelTeam.Attributes["teamid"] = hotelTeamId;
            hotelTeam.Attributes["tc_hotelteamid"] = new EntityReference("team", teamId);
            hotelTeam.Attributes["businessunitid"] = new EntityReference("businessunit", businessUnitId);

            var hotelId = Guid.NewGuid();
            var hotel = new Entity("tc_hotel", hotelId);
            hotel.Attributes["tc_hotel"] = hotelId;
            hotel.Attributes["owningteam"] = new EntityReference("team", teamId);

            var bookingAccommodationId = Guid.NewGuid();
            var bookingAccommodation = new Entity("tc_bookingaccommodation", bookingAccommodationId);
            bookingAccommodation.Attributes["tc_hotelid"] = new EntityReference("tc_hotel", hotelId);
            bookingAccommodation.Attributes["tc_bookingid"] = new EntityReference("tc_booking", bookingId);

            var booking = new Entity("tc_booking", bookingId);
            booking.Attributes["tc_bookingid"] = bookingId;

            return new List<Entity>() { booking, bookingAccommodation, hotel, team, hotelTeam };

        }

        public List<Entity> InitialiseUserHotelTeam(Guid userId, Guid businessUnitId)
        {
            var teamId = Guid.NewGuid();
            var team = new Entity("team", teamId);
            team.Attributes["teamid"] = teamId;

            var hotelTeamId = Guid.NewGuid();
            var hotelTeam = new Entity("team", hotelTeamId);
            hotelTeam.Attributes["teamid"] = hotelTeamId;
            hotelTeam.Attributes["tc_hotelteamid"] = new EntityReference("team", teamId);
            hotelTeam.Attributes["businessunitid"] = new EntityReference("businessunit", businessUnitId);

            var hotelId = Guid.NewGuid();
            var hotel = new Entity("tc_hotel", hotelId);
            hotel.Attributes["tc_hotel"] = hotelId;
            hotel.Attributes["owningteam"] = new EntityReference("team", teamId);

            var NtoN = new Entity("tc_systemuser_tc_hotel", Guid.NewGuid());
            NtoN.Attributes["tc_hotelid"] = new EntityReference("tc_hotel", hotelId);
            NtoN.Attributes["systemuserid"] = new EntityReference("systemuser", userId);

            var user = new Entity("systemuser", userId);
            user.Attributes["systemuserid"] = userId;

            return new List<Entity>() { hotel, team, hotelTeam, user, NtoN };
        }
    }
}
