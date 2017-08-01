using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;


namespace Tc.Crm.Plugins.Case.BusinessLogic
{
    public class AssignHotelTeamAsOwner
    {
        public IPluginExecutionContext context = null;
        public IOrganizationService service = null;
        public ITracingService trace = null;        

        public AssignHotelTeamAsOwner(IPluginExecutionContext context, ITracingService trace, IOrganizationService service)
        {
            this.context = context;
            this.service = service;
            this.trace = trace;
        }

        /// <summary>
        /// To check whether the context is valid to execute or not
        /// </summary>
        /// <returns></returns>
        private bool IsContextValid()
        {
            if (!context.MessageName.Equals(Messages.Create, StringComparison.OrdinalIgnoreCase) && !context.MessageName.Equals(Messages.Update, StringComparison.OrdinalIgnoreCase)) return false;
            if (context.Stage != (int)PluginStage.Prevalidation && context.Stage != (int)PluginStage.Postoperation) return false;
            if (context.PrimaryEntityName != Entities.Case) return false;
            return true;
        }

        /// <summary>
        /// Actions to do while creating a customer or case
        /// </summary>
        public void DoActions()
        {
            trace.Trace("DoActions - Start");
            if (!IsContextValid()) return;
            trace.Trace("Context is valid");
            if (!context.InputParameters.Contains(InputParameters.Target) || !(context.InputParameters[InputParameters.Target] is Entity)) return;                        
            var isRepUser = IsLoggedInUserisRepUser(context.InitiatingUserId);
            if (!isRepUser) return;
            AssignToChildTeam();
            trace.Trace("DoActions - End");
        }

        /// <summary>
        /// To set child hotel team as owner for case and customer records
        /// </summary>
        private void AssignToChildTeam()
        {
            trace.Trace("AssignToChildTeam - Start");
            var incident = (Entity)context.InputParameters[InputParameters.Target];
            var businessUnitId = GetSourceMarketBusinessUnit(incident);
            if (businessUnitId == Guid.Empty) return;           
            var bookingId = GetBookingId();
            var teamId = GetHotelTeamIdByBooking(businessUnitId, bookingId);
            if (teamId == Guid.Empty)
                teamId = GetHotelTeamIdByUser(context.InitiatingUserId, businessUnitId);
            if (teamId == Guid.Empty) return;
            if (context.MessageName.Equals(Messages.Create, StringComparison.OrdinalIgnoreCase))
            {
                SetCaseOwner(incident, teamId);
                UpdateCustomer(incident, teamId);
            }
            else
            {
                UpdateCustomer(incident, teamId);
            }
            trace.Trace("AssignToChildTeam - End");
        }

        /// <summary>
        /// To get bookingid from case record
        /// </summary>
        /// <returns></returns>
        private Guid GetBookingId()
        {
            trace.Trace("GetBookingId - Start");
            var bookingId = Guid.Empty;
            var incident = (Entity)context.InputParameters[InputParameters.Target];
            if (incident.Attributes.Contains(Attributes.Case.BookingId) && incident.Attributes[Attributes.Case.BookingId] != null)
            {
                bookingId = ((EntityReference)incident.Attributes[Attributes.Case.BookingId]).Id;
            }
            else if (context.PreEntityImages.Contains(PreEntityImages.CasePreImage) && context.PreEntityImages[PreEntityImages.CasePreImage] != null)
            {
                var preImageCase = context.PreEntityImages[PreEntityImages.CasePreImage];
                if (preImageCase.Attributes.Contains(Attributes.Case.BookingId) && preImageCase.Attributes[Attributes.Case.BookingId] != null)
                {
                    bookingId = ((EntityReference)preImageCase.Attributes[Attributes.Case.BookingId]).Id;
                }
            }
            trace.Trace("GetBookingId - End");
            return bookingId;
        }

        /// <summary>
        /// To update customer record
        /// </summary>
        /// <param name="incident"></param>
        /// <param name="teamId"></param>
        private void UpdateCustomer(Entity incident, Guid teamId)
        {
            trace.Trace("UpdateCustomer - Start");
            if (!incident.Attributes.Contains(Attributes.Case.CustomerId) || incident.Attributes[Attributes.Case.CustomerId] == null) return;
            var customer = (EntityReference)incident.Attributes[Attributes.Case.CustomerId];
            var customerToUpdate = new Entity(Entities.Customer, customer.Id);
            UpdateCustomerOwner(customerToUpdate, teamId);
            trace.Trace("UpdateCustomer - End");
        }

        /// <summary>
        /// To set hotel Team as owner
        /// </summary>
        /// <param name="customer"></param>
        private void SetCaseOwner(Entity incident, Guid teamId)
        {
            trace.Trace("SetOwner - Start");           
            incident.Attributes[Attributes.Case.Owner] = new EntityReference(Entities.Team, teamId);
            trace.Trace("SetOwner - End");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="teamId"></param>
        private void UpdateCustomerOwner(Entity customer, Guid teamId)
        {
            trace.Trace("UpdateCustomerOwner - Start");
            customer.Attributes[Attributes.Customer.Owner] = new EntityReference(Entities.Team, teamId);
            service.Update(customer);
            trace.Trace("UpdateCustomerOwner - end");
        }

        /// <summary>
        /// To check whether the user has Tc.Ids.Rep role
        /// </summary>
        /// <param name="systemUserId"></param>
        /// <returns></returns>
        private bool IsLoggedInUserisRepUser(Guid systemUserId)
        {
            trace.Trace("IsLoggedInUserisRepUser - Start");
            var query = $@"<fetch distinct='true' mapping='logical' output-format='xml-platform' version='1.0'>
                           <entity name='systemuser'>                           
                            <filter type='and' >
                             <condition attribute='systemuserid' operator='eq' value='{systemUserId}' />
                            </filter>
                           <link-entity name='systemuserroles' intersect='true' visible='false' to='systemuserid' from='systemuserid'>
                            <link-entity name='role' to='roleid' from='roleid' alias='role'>
                             <filter type='and'>
                              <condition attribute='name' operator='eq' value='{General.RoleTcIdRep}' />
                             </filter>
                            </link-entity>
                           </link-entity>
                           </entity>
                           </fetch>";
            var repUser = ExecuteFetchXml(query);
            trace.Trace("IsLoggedInUserisRepUser - End");
            return (repUser != null && repUser.Entities.Count > 0);
        }

              
        /// <summary>
        /// To get business unit of source market based on customer associated with case record
        /// </summary>
        /// <param name="incident"></param>
        /// <returns></returns>
        private Guid GetSourceMarketBusinessUnit(Entity incident)
        {
            trace.Trace("GetSourceMarketBusinessUnit - Start");
            var businessUnitId = Guid.Empty;
            if (incident == null) return businessUnitId;
            if (!incident.Attributes.Contains(Attributes.Case.CustomerId) || incident.Attributes[Attributes.Case.CustomerId] == null) return businessUnitId;           
            var fieldSourceMarketBusinessUnitId = "sourcemarket.tc_sourcemarketbusinessunitid";
            var customerId = (EntityReference)incident.Attributes[Attributes.Case.CustomerId];
            if (customerId.LogicalName.ToLower() != Entities.Customer) return businessUnitId;
            var query = $@"<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0'>
                            <entity name='contact'>                                
                             <filter type='and'>
                              <condition attribute='contactid' value='{customerId.Id.ToString()}' operator='eq'/>
                             </filter>
                            <link-entity name='tc_country' alias='sourcemarket' to='tc_sourcemarketid' from='tc_countryid'>
                            <attribute name='tc_sourcemarketbusinessunitid'/>
                            </link-entity>
                            </entity>
                            </fetch>";
            var customers = ExecuteFetchXml(query);
            if (customers == null || customers.Entities.Count == 0) return businessUnitId;
            var customer = customers.Entities[0];
            if (!customer.Attributes.Contains(fieldSourceMarketBusinessUnitId) || customer.Attributes[fieldSourceMarketBusinessUnitId] == null) return businessUnitId;
            businessUnitId = ((EntityReference)((AliasedValue)customer.Attributes[fieldSourceMarketBusinessUnitId]).Value).Id;            
            trace.Trace("GetSourceMarketBusinessUnit - End");
            return businessUnitId;
        }

        

        /// <summary>
        /// To get child hotel team based on source market business unit and hotel allocated to user 
        /// </summary>
        /// <param name="systemUserId"></param>
        /// <param name="businessUnitId"></param>
        /// <returns></returns>
        private Guid GetHotelTeamIdByUser(Guid systemUserId, Guid businessUnitId)
        {
            trace.Trace("GetHotelTeamIdByUser - Start");
            var teamId = Guid.Empty;
            var fieldChildTeamId = "childteam.teamid";
            var query = $@"<fetch distinct='true' mapping='logical' output-format='xml-platform' version='1.0'>
                            <entity name='tc_hotel' >                            
                            <order descending='false' attribute='tc_name' />
                            <link-entity name='tc_systemuser_tc_hotel' intersect='true' visible='false' to='tc_hotelid' from='tc_hotelid' >
                             <link-entity name='systemuser' to='systemuserid' from='systemuserid' alias='user' >
                              <filter type='and'>
                               <condition attribute='systemuserid' value='{systemUserId}' operator='eq' />
                              </filter>
                             </link-entity>
                            </link-entity>
                            <link-entity name='team' to='owningteam' from='teamid' alias='owningteam'>
                             <link-entity name='team' to='teamid' from='tc_hotelteamid' alias='childteam'>
                             <attribute name='teamid'/>
                              <filter type='and'>
                               <condition attribute='businessunitid' value='{businessUnitId}' operator='eq'/>
                              </filter>
                             </link-entity>
                            </link-entity>
                            </entity>
                            </fetch>";
            var hotels = ExecuteFetchXml(query);
            if (hotels == null || hotels.Entities.Count == 0) return teamId;
            var hotel = hotels.Entities[0];
            if(!hotel.Attributes.Contains(fieldChildTeamId) || hotel.Attributes[fieldChildTeamId] == null) return teamId;
            teamId = (Guid)((AliasedValue)hotel.Attributes[fieldChildTeamId]).Value;
            trace.Trace("GetHotelTeamIdByUser - End");
            return teamId;
        }

        /// <summary>
        /// To get child hotel team based on booking record associated for the case
        /// </summary>
        /// <param name="businessUnitId"></param>
        /// <returns></returns>
        private Guid GetHotelTeamIdByBooking(Guid businessUnitId, Guid bookingId)
        {
            trace.Trace("GetHotelTeamIdByBooking - Start");            
            var teamId = Guid.Empty;
            if (bookingId == Guid.Empty) return teamId;
            var fieldChildTeamId = "childteam.teamid";
            var query = $@"<fetch distinct='true' mapping='logical' output-format='xml-platform' version='1.0'>
                           <entity name='tc_booking'>
                            <link-entity name='tc_bookingaccommodation' alias='bookingaccommodation' to='tc_bookingid' from='tc_bookingid'>
                             <link-entity name='tc_hotel' alias='hotel' to='tc_hotelid' from='tc_hotelid'>
                              <link-entity name='team' alias='owningteam' to='owningteam' from='teamid'>
                               <link-entity name='team' alias='childteam' to='teamid' from='tc_hotelteamid'>
                               <attribute name='teamid'/>
                                <filter type='and'>
                                 <condition attribute='businessunitid' value='{businessUnitId}' operator='eq'/>
                                </filter>
                               </link-entity>
                              </link-entity>
                             </link-entity>
                            </link-entity>
                           <filter type='and'>
                            <condition attribute='tc_bookingid' value='{bookingId}' operator= 'eq' />
                           </filter>
                           </entity>
                           </fetch>";
            var hotelTeams = ExecuteFetchXml(query);
            if (hotelTeams == null || hotelTeams.Entities.Count == 0) return teamId;
            var hotelTeam = hotelTeams.Entities[0];
            if (!hotelTeam.Attributes.Contains(fieldChildTeamId) || hotelTeam.Attributes[fieldChildTeamId] == null) return teamId;
            teamId = (Guid)((AliasedValue)hotelTeam.Attributes[fieldChildTeamId]).Value;
            trace.Trace("GetHotelTeamIdByBooking - End");
            return teamId;
        }

        /// <summary>
        /// To execute fetchxml query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private EntityCollection ExecuteFetchXml(string query)
        {
            var fetch = new FetchExpression(query);
            return service.RetrieveMultiple(fetch);
        }
    }
}
