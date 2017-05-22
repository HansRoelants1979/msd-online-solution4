using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Tc.Crm.WebJob.DeallocateResortTeam.Models;
using Tc.Crm.Common;
using Tc.Crm.Common.Services;
using Tc.Crm.Common.Models;
using Attributes = Tc.Crm.Common.Constants.Attributes;
using EntityRecords = Tc.Crm.Common.Constants.EntityRecords;
using Tc.Crm.Common.Constants;


namespace Tc.Crm.WebJob.DeallocateResortTeam.Services
{
    public class DeallocationService : IDeallocationService
    {
        ICrmService crmService;
        ILogger logger;

        public DeallocationService(ILogger logger, ICrmService crmService)
        {
            this.logger = logger;
            this.crmService = crmService;
        }

        public DeallocationExecutionRequest FetchBookingsForDeallocation(DeallocationRequest bookingDeallocationRequest)
        {
            //logger.LogInformation("GetBookingAllocations - start");

            // don't process invalid request
            if (bookingDeallocationRequest == null || bookingDeallocationRequest.Destination == null ||
                bookingDeallocationRequest.Destination.Count == 0 ||
                bookingDeallocationRequest.Date == null || bookingDeallocationRequest.Date == DateTime.MinValue || bookingDeallocationRequest.Date == DateTime.MaxValue)
                return null;

            var destinationGateWays = GetLookupConditions(bookingDeallocationRequest.Destination);
            // get ids of bookings, filtered by return date and destination gateway, ordered by booking id, whose owner is hotel team
            // via customerbookingrole ordered by customer:
            // - get ids of contacts of booking and ids of their active incidents
            // - get ids of accounts of booking and ids of their active incidents
            // via country to business unit get business unit name and get business unit default team id
            var query = string.Format(
                @"<fetch distinct='true' mapping='logical' output-format='xml-platform' version='1.0'>
                    <entity name='tc_booking'>
                    <attribute name='tc_bookingid' />
                    <filter type='and'>
                        <condition attribute='tc_returndate' operator='on' value='{0}' />
                        <condition attribute='tc_destinationgatewayid' operator='in'>
                        {1}
                        </condition>
                    </filter>
                    <order attribute='tc_bookingid' />
                    <link-entity name='team' from='teamid' to='owningteam' link-type='inner'>
                        <filter type='and'>
                        <condition attribute='tc_hotelteam' operator='eq' value='1' />
                        </filter>
                    </link-entity>
                    <link-entity name='tc_customerbookingrole' from='tc_bookingid' to='tc_bookingid' link-type='outer'>
                        <order attribute='tc_customer' />
                        <link-entity name='contact' from='contactid' to='tc_customer' link-type='outer' alias='contact' visible='true'>
                            <attribute name='contactid' />
                            <link-entity name='incident' from='customerid' to='contactid' link-type='outer' alias='contactincident' visible='true'>
                                <attribute name='incidentid' />
                                <attribute name='ownerid' />
                                <filter type='and'>
                                    <condition attribute='statecode' operator='eq' value='0' />
                                </filter>
                            </link-entity>
                        </link-entity>
                        <link-entity name='account' from='accountid' to='tc_customer' link-type='outer' alias='account'>
                            <attribute name='accountid' />
                            <link-entity name='incident' from='customerid' to='accountid' link-type='outer' alias='accountincident' visible='true'>
                                <attribute name='incidentid' />
                                <attribute name='ownerid' />
                                <filter type='and'>
                                    <condition attribute='statecode' operator='eq' value='0' />
                                </filter>
                            </link-entity>
                        </link-entity>
                    </link-entity>
                    <link-entity name='tc_country' from='tc_countryid' to='tc_sourcemarketid' link-type='inner'>
                        <link-entity name='businessunit' from='businessunitid' to='tc_sourcemarketbusinessunitid' link-type='inner' alias='businessunit' visible='true'>
                            <attribute name='name' />
                            <link-entity name='team' from='businessunitid' to='businessunitid' link-type='inner' alias='team' visible='true'>
                                <attribute name='teamid' />
                                <filter>
                                    <condition attribute='isdefault' operator='eq' value='1' />
                                </filter>                                
                            </link-entity>
                        </link-entity>
                    </link-entity>
                    </entity>
                </fetch>",
                new object[] { bookingDeallocationRequest.Date.ToString("yyyy-MM-dd"),
                destinationGateWays.ToString() });

            EntityCollection bookingCollection = crmService.RetrieveMultipleRecordsFetchXml(query);           
            var caseOwnersandDefaultTeams = GetCaseOwnersandDefaultTeams(bookingCollection);
            var customerRelationUsers = GetUsersBySecurityRole(caseOwnersandDefaultTeams, bookingDeallocationRequest.UserRolesToAssignCase);
            var customerRelationTeams = GetTeamsBySecurityRole(caseOwnersandDefaultTeams, bookingDeallocationRequest.TeamRolesToAssignCase);
            var result = ConvertCrmResponse(bookingCollection, customerRelationUsers, customerRelationTeams);
            return result;
        }

        public Dictionary<Guid, OwnerType> GetCaseOwnersandDefaultTeams(EntityCollection caseCollection)
        {
            var caseOwnersandDefaultTeams = new Dictionary<Guid, OwnerType>();
            if (caseCollection == null || caseCollection.Entities.Count == 0) return caseOwnersandDefaultTeams;
            var fieldTeamId = AliasName.TeamAliasName + Attributes.Team.TeamId;
            var fieldContactCaseOwner = AliasName.ContactCaseAliasName + Attributes.CommonAttribute.Owner;
            var fieldAccountCaseOwner = AliasName.AccountCaseAliasName + Attributes.CommonAttribute.Owner;
            for (int i = 0; i < caseCollection.Entities.Count; i++)
            {
                var incident = caseCollection.Entities[i];
                Guid teamId = Guid.Empty;
                if (incident.Attributes.Contains(fieldTeamId))
                {
                    teamId = (Guid)((AliasedValue)incident.Attributes[fieldTeamId]).Value;
                    if (!caseOwnersandDefaultTeams.ContainsKey(teamId))
                        caseOwnersandDefaultTeams.Add(teamId, OwnerType.Team);
                }

                EntityReference owner = null;
                if (incident.Attributes.Contains(fieldContactCaseOwner))
                    owner = (EntityReference)((AliasedValue)incident.Attributes[fieldContactCaseOwner]).Value;
                else if (incident.Attributes.Contains(fieldAccountCaseOwner))
                    owner = (EntityReference)((AliasedValue)incident.Attributes[fieldAccountCaseOwner]).Value;

                if (owner != null && owner.LogicalName == EntityName.User)
                {
                    if (!caseOwnersandDefaultTeams.ContainsKey(owner.Id))
                        caseOwnersandDefaultTeams.Add(owner.Id, OwnerType.User);
                }
            }
            return caseOwnersandDefaultTeams;
        }

      
        public Collection<Guid> GetUsersBySecurityRole(Dictionary<Guid,OwnerType> caseOwnersandDefaultTeams, string securityRole)
        {
            var customerRelationUsers = new Collection<Guid>();
            if (caseOwnersandDefaultTeams == null || caseOwnersandDefaultTeams.Count == 0) return customerRelationUsers;
            var userCondition = GetLookupConditions(caseOwnersandDefaultTeams.Where(u => u.Value == OwnerType.User).ToDictionary(u => u.Key, u => u.Value).Keys);
            if(string.IsNullOrWhiteSpace(userCondition)) return customerRelationUsers;
            var query = $@"<fetch mapping='logical' output-format='xml-platform' version='1.0'>
                            <entity name='role'>
                            <filter type='and'>
                                <condition attribute='name' value='{securityRole}' operator='eq'/>
                            </filter>
                            <link-entity name='systemuserroles' intersect='true' visible='false' to='roleid' from='roleid'>
                                <link-entity name='systemuser' to='systemuserid' from='systemuserid' alias='systemuser'>
                                <attribute name='systemuserid'/>
                                    <filter type='and'>
                                        <condition attribute='systemuserid' operator='in'>
                                            {userCondition}
                                        </condition>
                                    </filter>
                                </link-entity>
                            </link-entity>
                            </entity>
                            </fetch>";
            EntityCollection roleCollection = crmService.RetrieveMultipleRecordsFetchXml(query);
            return GetCollectionByAttribute(roleCollection, AliasName.UserAliasName + Attributes.User.UserId);           
        }

        public Collection<Guid> GetTeamsBySecurityRole(Dictionary<Guid, OwnerType> caseOwnersandDefaultTeams, string securityRole)
        {
            var customerRelationTeams = new Collection<Guid>();
            if (caseOwnersandDefaultTeams == null || caseOwnersandDefaultTeams.Count == 0) return customerRelationTeams;
            var teamCondition = GetLookupConditions(caseOwnersandDefaultTeams.Where(t => t.Value == OwnerType.Team).ToDictionary(t => t.Key, t => t.Value).Keys);
            if (string.IsNullOrWhiteSpace(teamCondition)) return customerRelationTeams;
            var query = $@"<fetch mapping='logical' output-format='xml-platform' version='1.0'>
                            <entity name='role'>
                            <filter type='and'>
                                <condition attribute='name' value='{securityRole}' operator='eq'/>
                            </filter>
                            <link-entity name='teamroles' intersect='true' visible='false' to='roleid' from='roleid'>
                               <link-entity name='team' to='teamid' from='teamid' alias='team'>
                               <attribute name='teamid'/>
                                    <filter type='and'>
                                       <condition attribute='teamid' operator='in'>
                                            {teamCondition}
                                       </condition>
                                    </filter>
                                </link-entity>
                            </link-entity>
                            </entity>
                            </fetch>";
            EntityCollection roleCollection = crmService.RetrieveMultipleRecordsFetchXml(query);
            return GetCollectionByAttribute(roleCollection, AliasName.TeamAliasName + Attributes.Team.TeamId);       
        }

        public Collection<Guid> GetCollectionByAttribute(EntityCollection entityCollection, string attributeName)
        {
            var collection = new Collection<Guid>();
            if (entityCollection == null || entityCollection.Entities.Count == 0 || string.IsNullOrWhiteSpace(attributeName)) return collection;
            for (int i = 0; i < entityCollection.Entities.Count; i++)
            {
                var entity = entityCollection.Entities[i];
                if (!entity.Attributes.Contains(attributeName)) continue;
                var recordId = (Guid)((AliasedValue)entity.Attributes[attributeName]).Value;
                if (!collection.Contains(recordId))
                    collection.Add(recordId);
            }
            return collection;
        }


        public void DeallocateEntities(DeallocationExecutionRequest request)
        {
            var requests = new Collection<Entity>();
            // bookings
            CreateUpdateRequests(requests, request.Bookings);
            // customer
            CreateUpdateRequests(requests, request.Customers);
            // cases
            CreateUpdateRequests(requests, request.Cases);
            // assign
            if (requests.Count > 0)
                crmService.BulkUpdate(requests);
        }

        public string GetLookupConditions(IEnumerable<Guid> guids)
        {
            StringBuilder values = new StringBuilder();
            foreach (var guid in guids)
            {
                values.Append("<value>" + guid.ToString() + "</value>");
            }
            return values.ToString();
        }

        /// <summary>
        /// Covert CRM search results to collections of entities
        /// Implementation is based on assumption, that there cannot be one customer in 2 on-going bookings
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public DeallocationExecutionRequest ConvertCrmResponse(EntityCollection collection,Collection<Guid> customerRelationUsers, Collection<Guid> customerRelationTeams)
        {
            if (collection == null || collection.Entities.Count == 0)
                return null;

            var result = new DeallocationExecutionRequest
            {
                Bookings = new HashSet<Booking>(),
                Customers = new HashSet<Customer>(),
                Cases = new HashSet<Case>()
            };

            var currentBookingId = Guid.Empty;
            var currentCustomerId = Guid.Empty;
            var fieldTeamId = AliasName.TeamAliasName + Attributes.Team.TeamId;
            var fieldContactId = AliasName.ContactAliasName + Attributes.Contact.ContactId;
            var fieldAccountId = AliasName.AccountAliasName + Attributes.Account.AccountId;
            var fieldContactCaseOwner = AliasName.ContactCaseAliasName + Attributes.CommonAttribute.Owner;
            var fieldAccountCaseOwner = AliasName.AccountCaseAliasName + Attributes.CommonAttribute.Owner;
            var fieldContactCaseId = AliasName.ContactCaseAliasName + Attributes.Case.CaseId;
            var fieldAccountCaseId = AliasName.AccountCaseAliasName + Attributes.Case.CaseId;
            var fieldBusinessUnitName = AliasName.BusinessUnitAliasName + Attributes.BusinessUnit.Name;
            foreach (var searchRecord in collection.Entities)
            {
                var owner = new Owner {
                    Id = Guid.Parse(((AliasedValue)searchRecord[fieldTeamId]).Value.ToString()),
                    OwnerType = OwnerType.Team
                };

                var bookingId = Guid.Parse(searchRecord[Attributes.Booking.BookingId].ToString());
                if (currentBookingId != bookingId)
                {
                    // add booking entity
                    var booking = new Booking
                    {
                        Id = bookingId,
                        Owner = owner
                    };                    

                    currentBookingId = bookingId;
                    currentCustomerId = Guid.Empty;
                    if (!result.Bookings.Contains(booking))
                        result.Bookings.Add(booking);
                }
                if (searchRecord.Contains(fieldContactId) || searchRecord.Contains(fieldAccountId))
                {
                    var isContact = searchRecord.Contains(fieldContactId);
                    var customerId = Guid.Parse(((AliasedValue)searchRecord[isContact ? fieldContactId : fieldAccountId]).Value.ToString());
                    if (customerId != currentCustomerId)
                    {
                        // add customer entity
                        var customer = new Customer
                        {
                            Id = customerId,
                            Owner = owner,
                            CustomerType = isContact ? CustomerType.Contact : CustomerType.Account
                        };
                        currentCustomerId = customerId;
                        if (!result.Customers.Contains(customer))
                            result.Customers.Add(customer);
                    }
                }
                //Do this for UK only
                if (EntityRecords.BusinessUnit.GB.Equals(((AliasedValue)searchRecord[fieldBusinessUnitName]).Value.ToString(), StringComparison.InvariantCultureIgnoreCase) && (searchRecord.Contains(fieldContactCaseId) || searchRecord.Contains(fieldAccountCaseId)))
                {
                    var isContact = searchRecord.Contains(fieldContactCaseId);
                    var incidentId = Guid.Parse(((AliasedValue)searchRecord[isContact ? fieldContactCaseId : fieldAccountCaseId]).Value.ToString());
                    var caseOwner = (EntityReference)((AliasedValue)searchRecord[isContact ? fieldContactCaseOwner : fieldAccountCaseOwner]).Value;
                    // add case entity
                    var incident = new Case
                    {
                        Id = incidentId,                      
                        StatusCode = CaseStatusCode.AssignedToLocalSourceMarket,
                        State = CaseState.Active
                    };
                    if (!customerRelationUsers.Contains(caseOwner.Id) && customerRelationTeams.Contains(owner.Id) && caseOwner.Id != owner.Id)
                        incident.Owner = owner;
                    if (!result.Cases.Contains(incident))
                    {
                        result.Cases.Add(incident);
                    }
                }
            }

            return result;
        }
        

        public void CreateUpdateRequests(Collection<Entity> requets, IEnumerable<EntityModel> entities)
        {
            foreach (var entity in entities)
            {
                var updateEntity = new Entity(entity.EntityName, entity.Id) { EntityState = EntityState.Changed };
                // set owner   
                if (entity.Owner != null)
                    updateEntity[Attributes.CommonAttribute.Owner] = new EntityReference(entity.Owner.OwnerEntityName, entity.Owner.Id);
                if (entity is Case)
                {
                    var _case = (Case)entity;
                    updateEntity[Attributes.Case.State] = new OptionSetValue((int)_case.State);
                    updateEntity[Attributes.Case.StatusReason] = new OptionSetValue((int)((Case)entity).StatusCode);
                }
                requets.Add(updateEntity);
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DisposeObject(crmService);
                    DisposeObject(logger);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DeallocateService() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }

        void DisposeObject(Object obj)
        {
            if (obj != null)
            {
                if (obj is IDisposable)
                    ((IDisposable)obj).Dispose();
                else
                    obj = null;
            }

        }

       

        #endregion
    }
}
