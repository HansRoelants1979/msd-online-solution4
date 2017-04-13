using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;
using Tc.Crm.Common.Models;
using Tc.Crm.Common.Services;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
using Tc.Crm.Common.Constants;
using Attributes = Tc.Crm.Common.Constants.Attributes;
using System.Reflection;
using Tc.Crm.Common.Constants.EntityRecords;

namespace Tc.Crm.WebJob.DeallocateResortTeamTests
{
    public class TestCrmService : ICrmService
    {

        #region Properties 

        public XrmFakedContext Context { get; private set; }
        //private ICrmService Service { get; set; }
        public Collection<Guid> ValidGatewayIds { get; private set; }
        public Entity SystemUser { get; private set; }
        public Entity BusinessUnitUK { get; private set; }
        public Entity BusinessUnitNonUK { get; private set; }
        public Entity SourceMarketCountryUK{ get; private set; }
        public Entity SourceMarketCountryNonUK { get; private set; }
        public Entity DefaultTeamUK { get; private set; }
        public Entity DefaultTeamNonUK { get; private set; }

        #endregion

        public TestCrmService()
        {
            // XrmFakedContext does not parses deallocation fetch xml properly
            // Context = new XrmFakedContext { ProxyTypesAssembly = Assembly.GetExecutingAssembly() };
            //Service = new CrmService(new TestConfigurationService(), new TestLogger(), Context.GetFakedOrganizationService());

            Context = new XrmFakedContext();
            ValidGatewayIds = new Collection<Guid>();
            PrepareData();
        }

        #region Test helpers

        public void PrepareData()
        {
            Context.Data.Clear();
            // initialize entities
            Context.Data.Add(EntityName.Booking, new Dictionary<Guid, Entity>());
            Context.Data.Add(EntityName.Country, new Dictionary<Guid, Entity>());
            Context.Data.Add(EntityName.BusinessUnit, new Dictionary<Guid, Entity>());
            Context.Data.Add(EntityName.Team, new Dictionary<Guid, Entity>());
            Context.Data.Add(EntityName.CustomerBookingRole, new Dictionary<Guid, Entity>());
            Context.Data.Add(EntityName.Contact, new Dictionary<Guid, Entity>());
            Context.Data.Add(EntityName.Account, new Dictionary<Guid, Entity>());
            Context.Data.Add(EntityName.Case, new Dictionary<Guid, Entity>());
            Context.Data.Add(EntityName.User, new Dictionary<Guid, Entity>());
            AddConfiguredGatewaysToContext();

            // add default data
            var userId = Guid.NewGuid();
            SystemUser = new Entity(EntityName.User, userId);
            SystemUser[Attributes.User.UserId] = userId;
            Context.Data[EntityName.User].Add(userId, SystemUser);

            BusinessUnitUK = AddBusinessUnit(BusinessUnit.GB);
            BusinessUnitNonUK = AddBusinessUnit("DE");

            DefaultTeamUK = AddTeam(CreateReference(EntityName.BusinessUnit, Attributes.BusinessUnit.BusinessUnitId, BusinessUnitUK.Id), false, true);
            DefaultTeamNonUK = AddTeam(CreateReference(EntityName.BusinessUnit, Attributes.Team.BusinessUnitId, BusinessUnitNonUK.Id), false, true);

            SourceMarketCountryUK = AddCountry(CreateReference(EntityName.BusinessUnit, Attributes.Country.BusinessUnitId, BusinessUnitUK.Id));
            SourceMarketCountryNonUK = AddCountry(CreateReference(EntityName.BusinessUnit, Attributes.Country.BusinessUnitId, BusinessUnitNonUK.Id));
        }

        public Entity AddBusinessUnit(string name)
        {
            var id = Guid.NewGuid();
            var businessUnit = new Entity(EntityName.BusinessUnit, id);
            businessUnit[Attributes.BusinessUnit.BusinessUnitId] = id;
            businessUnit[Attributes.BusinessUnit.Name] = name;

            Context.Data[EntityName.BusinessUnit].Add(id, businessUnit);
            return businessUnit;
        }

        public Entity AddGateway()
        {
            var id = Guid.NewGuid();
            var gateway = new Entity(EntityName.Gateway, id);
            gateway[Attributes.Gateway.GatewayId] = id;
            Context.Data[EntityName.Gateway].Add(id, gateway);
            return gateway;
        }

        public Entity AddTeam(EntityReference businessUnit, bool isHotelTeam, bool isDefaultTeam = false)
        {
            var id = Guid.NewGuid();
            var team = new Entity(EntityName.Team, id);
            team[Attributes.Team.TeamId] = id;
            team[Attributes.Team.BusinessUnitId] = businessUnit;
            team[Attributes.Team.IsDefaultTeam] = isDefaultTeam;
            team[Attributes.Team.IsHotelTeam] = isHotelTeam;

            Context.Data[EntityName.Team].Add(id, team);
            return team;
        }

        public Entity AddCountry(EntityReference businessUnit)
        {
            var id = Guid.NewGuid();
            var country = new Entity(EntityName.Country, id);
            country[Attributes.Country.CountryId] = id;
            country[Attributes.Country.BusinessUnitId] = businessUnit;

            Context.Data[EntityName.Country].Add(id, country);
            return country;
        }

        public Entity AddCase(EntityReference customer, EntityReference owningTeam)
        {
            var id = Guid.NewGuid();
            var _case = new Entity(EntityName.Case, id);
            _case[Attributes.Case.CaseId] = id;
            _case[Attributes.Case.CustomerId] = customer;
            _case[Attributes.Case.Owner] = owningTeam ?? CreateReference(EntityName.User, Attributes.Booking.Owner, SystemUser.Id);

            Context.Data[EntityName.Case].Add(id, _case);
            return _case;
        }

        public Entity AddContact(EntityReference owningTeam)
        {
            var id = Guid.NewGuid();

            var contact = new Entity(EntityName.Contact, id);
            contact[Attributes.Contact.ContactId] = id;
            contact[Attributes.Booking.Owner] = owningTeam ?? CreateReference(EntityName.User, Attributes.Booking.Owner, SystemUser.Id);

            Context.Data[EntityName.Contact].Add(id, contact);
            return contact;
        }

        public Entity AddAccount(EntityReference owningTeam)
        {
            var id = Guid.NewGuid();
            var account = new Entity(EntityName.Account, id);
            account[Attributes.Account.AccountId] = id;
            account[Attributes.Booking.Owner] = owningTeam ?? CreateReference(EntityName.User, Attributes.Booking.Owner, SystemUser.Id);

            Context.Data[EntityName.Account].Add(id, account);
            return account;
        }

        public Entity AddCustomerBookingRole(EntityReference booking, EntityReference customer)
        {
            var id = Guid.NewGuid();
            var role = new Entity(EntityName.CustomerBookingRole, id);
            role[Attributes.CustomerBookingRole.CustomerBookingRoleId] = id;
            role[Attributes.CustomerBookingRole.Customer] = customer;
            role[Attributes.CustomerBookingRole.BookingId] = booking;

            Context.Data[EntityName.CustomerBookingRole].Add(id, role);
            return role;
        }

        public Entity AddBooking(DateTime returnDate, EntityReference owningTeam, EntityReference gateway, EntityReference sourceMarket)
        {
            var id = Guid.NewGuid();
            var booking = new Entity(EntityName.Booking, id);

            booking[Attributes.Booking.BookingId] = id;
            booking[Attributes.Booking.ReturnDate] = returnDate;
            booking[Attributes.Booking.DestinationGatewayId] = gateway;
            booking[Attributes.Booking.SourceMarketId] = sourceMarket;
            booking[Attributes.Booking.OwningTeam] = owningTeam;
            booking[Attributes.Booking.Owner] = owningTeam ?? CreateReference(EntityName.User, Attributes.Booking.Owner, SystemUser.Id);

            Context.Data[EntityName.Booking].Add(id, booking);
            return booking;
        }

        private void AddConfiguredGatewaysToContext()
        {
            var gatewayEntityCollection = new Dictionary<Guid, Entity>();

            var configurationService = new TestConfigurationService();
            var gatewayIdsString = configurationService.DestinationGatewayIds;
            var gatewayIds = gatewayIdsString.Split(',');
            for (int i = 0; i < gatewayIds.Length; i++)
            {
                var gatewayId = new Guid(gatewayIds[i]);
                var gateway = new Entity(EntityName.Gateway, gatewayId);
                gateway[Attributes.Gateway.GatewayId] = gatewayId;
                gatewayEntityCollection.Add(gatewayId, gateway);
                ValidGatewayIds.Add(gatewayId);
            }
            Context.Data.Add(EntityName.Gateway, gatewayEntityCollection);
        }

        #endregion

        #region ICrmService        

        public void BulkAssign(Collection<AssignInformation> assignRequests)
        {
            throw new NotImplementedException();
        }

        public void BulkUpdate(IEnumerable<Entity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException("BulkUpdate: entities are null");
            // Service.BulkUpdate(entities);
            foreach (var entity in entities)
            {
                var owner = (EntityReference)entity[Attributes.Entity.Owner];
                Context.Data[entity.LogicalName][entity.Id][Attributes.Entity.Owner] = new EntityReference(owner.LogicalName, owner.Id);
                if (entity.LogicalName == EntityName.Case)
                {
                    Context.Data[entity.LogicalName][entity.Id][Attributes.Case.StatusReason] = entity[Attributes.Case.StatusReason];
                    Context.Data[entity.LogicalName][entity.Id][Attributes.Case.State] = entity[Attributes.Case.State];
                }
            }
        }

        public EntityCollection RetrieveMultipleRecordsFetchXml(string query)
        {
            //return Service.RetrieveMultipleRecordsFetchXml(query);

            return GetBookings(
                Context.Data[EntityName.Booking].Values.ToList(),
                Context.Data[EntityName.Gateway].Values.ToList(),
                Context.Data[EntityName.CustomerBookingRole].Values.ToList(),
                Context.Data[EntityName.Contact].Values.ToList(),
                Context.Data[EntityName.Account].Values.ToList(),
                Context.Data[EntityName.Case].Values.ToList(),
                Context.Data[EntityName.Country].Values.ToList(),
                Context.Data[EntityName.BusinessUnit].Values.ToList(),
                Context.Data[EntityName.Team].Values.ToList());
        }

        #endregion

        private EntityCollection GetBookings(
            List<Entity> bookings,
            List<Entity> gateways,
            List<Entity> customerBookingRoles,
            List<Entity> contacts,
            List<Entity> accounts,
            List<Entity> cases,
            List<Entity> countries,
            List<Entity> businessUnits,
            List<Entity> teams)
        {
            var result = new List<Entity>();

            // fetch xml filter
            var filteredData = (from b in bookings
                                join g in gateways on ((EntityReference)b[Attributes.Booking.DestinationGatewayId]).Id equals g.Id
                                join team in teams on
                                    new { Id = b[Attributes.Booking.OwningTeam] != null ? ((EntityReference)b[Attributes.Booking.OwningTeam]).Id : Guid.Empty }
                                    equals new { Id = team.Id }
                                join country in countries on ((EntityReference)b[Attributes.Booking.SourceMarketId]).Id equals country.Id
                                join businessUnit in businessUnits on ((EntityReference)country[Attributes.Country.BusinessUnitId]).Id equals businessUnit.Id
                                join defaultTeam in teams on businessUnit.Id equals ((EntityReference)defaultTeam[Attributes.Team.BusinessUnitId]).Id
                                join role in customerBookingRoles on b.Id equals ((EntityReference)role[Attributes.CustomerBookingRole.BookingId]).Id into cbr
                                from role in cbr.DefaultIfEmpty()
                                join contact in contacts on
                                    new { Id = role != null ? ((EntityReference)role[Attributes.CustomerBookingRole.Customer]).Id : Guid.Empty }
                                    equals new { Id = contact.Id } into c
                                from contact in c.DefaultIfEmpty()
                                join contactCase in cases on
                                    new { Id = contact != null ? contact.Id : Guid.Empty }
                                    equals new { Id = ((EntityReference)contactCase[Attributes.Case.CustomerId]).Id } into cc
                                from contactCase in cc.DefaultIfEmpty()
                                join account in accounts on
                                    new { Id = role != null ? ((EntityReference)role[Attributes.CustomerBookingRole.Customer]).Id : Guid.Empty }
                                    equals new { Id = account.Id } into a
                                from account in a.DefaultIfEmpty()
                                join accountCase in cases on
                                    new { Id = account != null ? account.Id : Guid.Empty }
                                    equals
                                    new { Id = ((EntityReference)accountCase[Attributes.Case.CustomerId]).Id }  into ac
                                from accountCase in ac.DefaultIfEmpty()
                                where ((DateTime)b[Attributes.Booking.ReturnDate]).Date == DateTime.Now.Date.AddDays(-2)
                                && ValidGatewayIds.Contains(((EntityReference)b[Attributes.Booking.DestinationGatewayId]).Id)
                                && (bool)defaultTeam[Attributes.Team.IsDefaultTeam] == true
                                && (bool)team[Attributes.Team.IsHotelTeam] == true                              
                             select new
                             {
                                 BookingId = b.Id,
                                 ContactId = contact != null ? contact.Id : Guid.Empty,
                                 AccountId = account != null ? account.Id : Guid.Empty,
                                 ContactCaseId = contactCase != null ? contactCase.Id : Guid.Empty,
                                 AccountCaseId = accountCase != null ? accountCase.Id: Guid.Empty,
                                 DefaultTeamId = defaultTeam.Id,
                                 BusinessUnitName = businessUnit[Attributes.BusinessUnit.Name]
                             }).GroupBy(r => new { r.BookingId, r.ContactId, r.AccountId, r.ContactCaseId, r.AccountCaseId, r.DefaultTeamId }).
                             Select(r => r.First()).OrderBy(r => r.BookingId).ThenBy(r => r.ContactId).ThenBy(r => r.AccountId);

            foreach (var item in filteredData)
            {
                var b = new Entity(EntityName.Booking, item.BookingId);
                b[Attributes.Booking.BookingId] = item.BookingId;
                b["defaultTeam.teamid"] = new AliasedValue(EntityName.Team, Attributes.Team.TeamId, item.DefaultTeamId);
                b["businessUnit.name"] = new AliasedValue(EntityName.BusinessUnit, Attributes.BusinessUnit.Name, item.BusinessUnitName);
                if (item.ContactId != Guid.Empty)
                {
                    b["contact.contactid"] = new AliasedValue(EntityName.Contact, Attributes.Contact.ContactId, item.ContactId);
                }
                if (item.AccountId != Guid.Empty)
                {
                    b["account.accountid"] = new AliasedValue(EntityName.Account, Attributes.Account.AccountId, item.AccountId);
                }
                if (item.ContactCaseId != Guid.Empty)
                {
                    b["contactIncident.incidentid"] = new AliasedValue(EntityName.Case, Attributes.Case.CaseId, item.ContactCaseId);
                }
                if (item.AccountCaseId != Guid.Empty)
                {
                    b["accountIncident.incidentid"] = new AliasedValue(EntityName.Case, Attributes.Case.CaseId, item.AccountCaseId);
                }
                result.Add(b);
            }

            return new EntityCollection(result);

        }

        private static EntityReference CreateReference(string entity, string key, Guid id)
        {
            return new EntityReference(entity, key, id) { Id = id };
        }
    }
}
