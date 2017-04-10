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

        #region Properties 

        public DataSwitch Switch { get; set; }
        public XrmFakedContext context;
        public IOrganizationService orgService;

        public List<Guid> ValidGatewayIds { get; private set; }
        public Entity SystemUser { get; private set; }
        public Entity BusinessUnit1 { get; private set; }
        public Entity BusinessUnit2 { get; private set; }
        public Entity SourceMarketCountry1 { get; private set; }
        public Entity SourceMarketCountry2 { get; private set; }
        public Entity DefaultTeam1 { get; private set; }
        public Entity DefaultTeam2 { get; private set; }

        #endregion

        public TestCrmService()
        {
            context = new XrmFakedContext();
            orgService = GetOrganizationService();
            ValidGatewayIds = new List<Guid>();
            PrepareData();
        }

        #region Test helpers

        public void PrepareData()
        {
            context.Data.Clear();
            // initialize entities
            context.Data.Add(EntityName.Booking, new Dictionary<Guid, Entity>());            
            context.Data.Add(EntityName.Country, new Dictionary<Guid, Entity>());
            context.Data.Add(EntityName.BusinessUnit, new Dictionary<Guid, Entity>());
            context.Data.Add(EntityName.Team, new Dictionary<Guid, Entity>());
            context.Data.Add(EntityName.CustomerBookingRole, new Dictionary<Guid, Entity>());            
            context.Data.Add(EntityName.Contact, new Dictionary<Guid, Entity>());
            context.Data.Add(EntityName.Account, new Dictionary<Guid, Entity>());
            context.Data.Add(EntityName.Case, new Dictionary<Guid, Entity>());
            context.Data.Add(EntityName.User, new Dictionary<Guid, Entity>());
            AddConfiguredGatewaysToContext();

            // add default data
            var userId = Guid.NewGuid();
            SystemUser = new Entity(EntityName.User, userId);
            SystemUser[Attributes.User.UserId] = userId;
            context.Data[EntityName.User].Add(userId, SystemUser);

            BusinessUnit1 = AddBusinessUnit();
            BusinessUnit2 = AddBusinessUnit();

            DefaultTeam1 = AddTeam(CreateReference(EntityName.BusinessUnit, Attributes.BusinessUnit.BusinessUnitId, BusinessUnit1.Id), false, true);
            DefaultTeam2 = AddTeam(CreateReference(EntityName.BusinessUnit, Attributes.Team.BusinessUnitId, BusinessUnit1.Id), false, true);

            SourceMarketCountry1 = AddCountry(CreateReference(EntityName.BusinessUnit, Attributes.Country.BusinessUnitId, BusinessUnit1.Id));
            SourceMarketCountry2 = AddCountry(CreateReference(EntityName.BusinessUnit, Attributes.Country.BusinessUnitId, BusinessUnit2.Id));
            
            
        }

        public Entity AddBusinessUnit()
        {
            var id = Guid.NewGuid();
            var businessUnit = new Entity(EntityName.BusinessUnit, id);
            businessUnit[Attributes.BusinessUnit.BusinessUnitId] = id;

            context.Data[EntityName.BusinessUnit].Add(id, businessUnit);
            return businessUnit;
        }

        public Entity AddGateway()
        {
            var id = Guid.NewGuid();
            var gateway = new Entity(EntityName.Gateway, id);
            gateway[Attributes.Gateway.GatewayId] = id;
            context.Data[EntityName.Gateway].Add(id, gateway);
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

            context.Data[EntityName.Team].Add(id, team);
            return team;
        }

        public Entity AddCountry(EntityReference businessUnit)
        {
            var id = Guid.NewGuid();
            var country = new Entity(EntityName.Country, id);
            country[Attributes.Country.CountryId] = id;
            country[Attributes.Country.BusinessUnitId] = businessUnit;

            context.Data[EntityName.Country].Add(id, country);
            return country;
        }

        public Entity AddCase(EntityReference customer, EntityReference owningTeam)
        {
            var id = Guid.NewGuid();
            var _case = new Entity(EntityName.Case, id);
            _case[Attributes.Case.CaseId] = id;
            _case[Attributes.Case.CustomerId] = customer;
            _case[Attributes.Case.Owner] = owningTeam ?? CreateReference(EntityName.User, Attributes.Booking.Owner, SystemUser.Id);

            context.Data[EntityName.Case].Add(id, _case);
            return _case;
        }

        public Entity AddContact(EntityReference owningTeam)
        {
            var id = Guid.NewGuid();

            var contact = new Entity(EntityName.Contact, id);
            contact[Attributes.Contact.ContactId] = id;
            contact[Attributes.Booking.Owner] = owningTeam ?? CreateReference(EntityName.User, Attributes.Booking.Owner, SystemUser.Id);

            context.Data[EntityName.Contact].Add(id, contact);
            return contact;
        }

        public Entity AddAccount(EntityReference owningTeam)
        {
            var id = Guid.NewGuid();
            var account = new Entity(EntityName.Account, id);
            account[Attributes.Account.AccountId] = id;
            account[Attributes.Booking.Owner] = owningTeam ?? CreateReference(EntityName.User, Attributes.Booking.Owner, SystemUser.Id);

            context.Data[EntityName.Account].Add(id, account);
            return account;
        }

        public Entity AddCustomerBookingRole(EntityReference booking, EntityReference customer)
        {
            var id = Guid.NewGuid();
            var role = new Entity(EntityName.CustomerBookingRole, id);
            role[Attributes.CustomerBookingRole.CustomerBookingRoleId] = id;
            role[Attributes.CustomerBookingRole.Customer] = customer;
            role[Attributes.CustomerBookingRole.BookingId] = booking;

            context.Data[EntityName.CustomerBookingRole].Add(id, role);
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

            context.Data[EntityName.Booking].Add(id, booking);
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
            context.Data.Add(EntityName.Gateway, gatewayEntityCollection);
        }

        #endregion

        #region ICrmService

        public IOrganizationService GetOrganizationService()
        {

            return context.GetFakedOrganizationService();
        }

        public void BulkAssign(Collection<AssignInformation> assignRequests)
        {
            foreach (var ar in assignRequests)
            {
                string owner = ar.RecordOwner.OwnerType == Common.OwnerType.Team ? EntityName.Team : EntityName.User; 
                context.Data[ar.EntityName][ar.RecordId]["ownerid"] = new EntityReference(owner, ar.RecordOwner.Id);
            }
        }

        public EntityCollection RetrieveMultipleRecordsFetchXml(string query)
        {
            switch (Switch)
            {
                case DataSwitch.No_Records_Returned:
                    return null;
                case DataSwitch.Collection_With_No_Records_Returned:
                    return new Microsoft.Xrm.Sdk.EntityCollection();
                case DataSwitch.Returns_Data:
                    return GetBookings(
                        context.Data[EntityName.Booking].Values.ToList(),
                        context.Data[EntityName.Gateway].Values.ToList(),
                        context.Data[EntityName.CustomerBookingRole].Values.ToList(),
                        context.Data[EntityName.Contact].Values.ToList(),
                        context.Data[EntityName.Account].Values.ToList(),
                        context.Data[EntityName.Case].Values.ToList(),
                        context.Data[EntityName.Country].Values.ToList(),
                        context.Data[EntityName.BusinessUnit].Values.ToList(),
                        context.Data[EntityName.Team].Values.ToList());
            }
            return null;
        }

        #region Not Implemented

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

        public EntityCollection GetRecordsUsingQuery(Microsoft.Xrm.Sdk.Query.QueryExpression queryExpression)
        {
            throw new NotImplementedException();
        }

        public EntityCollection RetrieveMultipleRecords(string entityName, string[] columns, string[] filterKeys, string[] filterValues)
        {
            return null;
        }

        public void ExecuteBulkAssignRequests(ExecuteMultipleRequest request)
        {
            throw new NotImplementedException();
        }

        public string FormatFaultException(AssignRequest assignRequest, OrganizationServiceFault fault)
        {
            throw new NotImplementedException();
        }

        #endregion

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
                                join team in teams on ((EntityReference)b[Attributes.Booking.OwningTeam]).Id equals team.Id
                                join country in countries on ((EntityReference)b[Attributes.Booking.SourceMarketId]).Id equals country.Id
                                join bu in businessUnits on ((EntityReference)country[Attributes.Country.BusinessUnitId]).Id equals bu.Id
                                join defaultTeam in teams on bu.Id equals ((EntityReference)defaultTeam[Attributes.Team.BusinessUnitId]).Id
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
                                orderby b.Id ascending, role.Id ascending
                             select new
                             {
                                 BookingId = b.Id,
                                 ContactId = contact != null ? contact.Id : Guid.Empty,
                                 AccountId = account != null ? account.Id : Guid.Empty,
                                 ContactCaseId = contactCase != null ? contactCase.Id : Guid.Empty,
                                 AccountCaseId = accountCase != null ? accountCase.Id: Guid.Empty,
                                 DefaultTeamId = defaultTeam.Id
                             }).GroupBy(r => new { r.BookingId, r.ContactId, r.AccountId, r.ContactCaseId, r.AccountCaseId, r.DefaultTeamId }).Select(r => r.First());

            foreach (var item in filteredData)
            {
                var b = new Entity(EntityName.Booking, item.BookingId);
                b[Attributes.Booking.BookingId] = item.BookingId;
                b["defaultTeam.teamid"] = new AliasedValue(EntityName.Team, Attributes.Team.TeamId, item.DefaultTeamId);
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

        private EntityReference CreateReference(string entity, string key, Guid id)
        {
            return new EntityReference(entity, key, id) { Id = id };
        }
    }
}
