using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Tc.Crm.WebJob.DeallocateResortTeam.Models;
using Tc.Crm.Common;
using Tc.Crm.Common.Services;
using Tc.Crm.Common.Constants;
using Tc.Crm.Common.Constants.Attributes;
using Tc.Crm.Common.Models;
using System.Collections.ObjectModel;

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

        public IList<BookingDeallocationResponse> GetBookingDeallocations(BookingDeallocationRequest bookingDeallocationRequest)
        {
            logger.LogInformation("GetBookingAllocations - start");
            IList<BookingDeallocationResponse> bookingAllocationResponse = null;
            if (bookingDeallocationRequest != null)
            {
                if (bookingDeallocationRequest.Destination != null)
                {
                    var destinationGateWays = GetDestinationGateways(bookingDeallocationRequest.Destination);
                    if (bookingDeallocationRequest.AccommodationEndDate != null)
                    {
                        var query = string.Format(@"<fetch distinct='true' mapping='logical' output-format='xml-platform' version='1.0'>
                                                 <entity name='tc_booking'>
                                                    <attribute name='tc_bookingid'/>
                                                    <attribute name='tc_name'/>
                                                    <attribute name='ownerid'/>
                                                    <order descending='false' attribute='tc_name'/>
                                                    <filter type='and'>
                                                    <condition attribute='tc_destinationgatewayid' operator='in'>
                                                    {1}
                                                    </condition>
                                                    </filter>
                                                      <link-entity name='tc_bookingaccommodation' alias='accommodation' to='tc_bookingid' from='tc_bookingid'>
                                                        <attribute name='tc_enddateandtime'/>
                                                        <order descending='false' attribute='tc_enddateandtime'/>
                                                        <filter type='and'>
                                                          <condition attribute='tc_enddateandtime' value='{0}' operator='on'/>
                                                        </filter>
                                                        <link-entity name='tc_hotel' alias='hotel' to='tc_hotelid' from='tc_hotelid'>
                                                            <attribute name='tc_hotelid'/>
                                                        </link-entity>
                                                        </link-entity>
                                                        <link-entity name='tc_customerbookingrole' alias='role' to='tc_bookingid' from='tc_bookingid'>
                                                            <attribute name='tc_customer'/>
                                                             <link-entity link-type='outer' name='account' alias='account' from='accountid' to='tc_customer'>
                                                                <attribute name='ownerid'/>
                                                             </link-entity>
                                                             <link-entity link-type='outer' name='contact' alias='contact' from='contactid' to='tc_customer'>
                                                                <attribute name='ownerid'/>
                                                             </link-entity>
                                                        </link-entity>                                                    
                                                    </entity>
                                                    </fetch>",
                                                     new object[] { bookingDeallocationRequest.AccommodationEndDate.ToString("yyyy-MM-dd"),
                                                     destinationGateWays.ToString() });

                        EntityCollection bookingCollection = crmService.RetrieveMultipleRecordsFetchXml(query);
                        logger.LogInformation("GetBookingAllocations - end");
                        bookingAllocationResponse = PrepareBookingDeallocation(bookingCollection);
                    }
                }
            }
            return bookingAllocationResponse;
        }

        public StringBuilder GetDestinationGateways(IList<Guid> destinationGateways)
        {
            logger.LogInformation("GetDestinationGateways - start");
            StringBuilder gateways = new StringBuilder();
            if (destinationGateways != null && destinationGateways.Count > 0)
            {
                for (int i = 0; i < destinationGateways.Count; i++)
                {
                    gateways.Append("<value>" + destinationGateways[i].ToString() + "</value>");
                }
            }
            logger.LogInformation("GetDestinationGateways - end");
            return gateways;
        }


        public IList<BookingDeallocationResponse> PrepareBookingDeallocation(EntityCollection bookingCollection)
        {
            logger.LogInformation("PrepareBookingDeallocation - start");
            IList<BookingDeallocationResponse> bookingDeallocationResponse = null;
            if (bookingCollection != null && bookingCollection.Entities.Count > 0)
            {
                bookingDeallocationResponse = new List<BookingDeallocationResponse>();
                for (int i = 0; i < bookingCollection.Entities.Count; i++)
                {
                    var booking = bookingCollection.Entities[i];
                    if (booking != null)
                    {
                        var response = new BookingDeallocationResponse();
                        if (booking.Contains(Booking.BookingId) && booking[Booking.BookingId] != null)
                            response.BookingId = Guid.Parse(booking[Booking.BookingId].ToString());

                        if (booking.Contains(Booking.Name) && booking[Booking.Name] != null)
                            response.BookingNumber = booking[Booking.Name].ToString();

                        var fieldHotelId = AliasName.HotelAliasName + Hotel.HotelId;
                        var fieldEndDate = AliasName.AccommodationAliasName + BookingAccommodation.EndDateandTime;
                        var fieldCustomer = AliasName.RoleAliasName + CustomerBookingRole.Customer;
                        var fieldAccountOwner = AliasName.AccountAliasName + Common.Constants.Attributes.Customer.Owner;
                        var fieldContactOwner = AliasName.ContactAliasName + Common.Constants.Attributes.Customer.Owner;

                        if (booking.Contains(fieldHotelId) && booking[fieldHotelId] != null)
                            response.HotelId = Guid.Parse(((AliasedValue)booking[fieldHotelId]).Value.ToString());

                        if (booking.Contains(fieldEndDate) && booking[fieldEndDate] != null)
                            response.AccommodationEndDate = DateTime.Parse(((AliasedValue)booking[fieldEndDate]).Value.ToString());

                        if (booking.Contains(fieldCustomer) && booking[fieldCustomer] != null)
                        {
                            EntityReference customer = (EntityReference)((AliasedValue)booking[fieldCustomer]).Value;
                            CustomerType customerType;
                            Owner owner;

                            if (customer.LogicalName == EntityName.Contact)
                                customerType = CustomerType.Contact;
                            else
                                customerType = CustomerType.Account;

                            if (booking.Contains(fieldContactOwner) && booking[fieldContactOwner] != null)
                                owner = GetOwner(booking, fieldContactOwner, true);
                            else
                                owner = GetOwner(booking, fieldAccountOwner, true);

                            response.Customer = new Common.Models.Customer() { Id = customer.Id, Name = customer.Name, CustomerType = customerType,Owner=owner };
                        }

                            response.BookingOwner = GetOwner(booking, Booking.Owner, false);                        

                        bookingDeallocationResponse.Add(response);
                    }

                }
            }
            logger.LogInformation("PrepareBookingDeallocation - end");
            return bookingDeallocationResponse;
        }

        public OwnerType GetOwnerType(EntityReference owner)
        {
            if (owner.LogicalName == EntityName.User)
                return OwnerType.User;
            else
                return OwnerType.Team;
        }

        public Owner GetOwner(Entity entity, string attributeName, bool isAliasedValue)
        {
            if (entity == null) return null;
            if (!entity.Contains(attributeName)) return null;
            if (entity[attributeName] == null) return null;
            EntityReference owner;
            if (isAliasedValue)
                owner = (EntityReference)((AliasedValue)entity[attributeName]).Value;
            else
                owner = (EntityReference)entity.Attributes[attributeName];

            var ownerType = GetOwnerType(owner);

            return new Owner() { Id = owner.Id, Name = owner.Name, OwnerType = ownerType };
        }

        public void ProcessBookingAllocations(IList<BookingDeallocationResortTeamRequest> bookingDeallocationResortTeamRequest)
        {
            
            if (bookingDeallocationResortTeamRequest != null && bookingDeallocationResortTeamRequest.Count > 0)
            {
                var assignRequests = new Collection<AssignInformation>();

                for (int i = 0; i < bookingDeallocationResortTeamRequest.Count; i++)
                {
                    if (bookingDeallocationResortTeamRequest[i] == null) continue;

                    var bookingTeamRequest = bookingDeallocationResortTeamRequest[i];
                    if (bookingTeamRequest.BookingResortTeamRequest != null)
                    {
                        var assignBookingRequest = new AssignInformation
                        {
                            EntityName = EntityName.Booking,
                            RecordId = bookingTeamRequest.BookingResortTeamRequest.Id,
                            RecordOwner = bookingTeamRequest.BookingResortTeamRequest.Owner
                        };
                        assignRequests.Add(assignBookingRequest);
                    }
                    if (bookingTeamRequest.CustomerResortTeamRequest != null)
                    {
                        var assignCustomerRequest = new AssignInformation
                        {
                            EntityName = bookingTeamRequest.CustomerResortTeamRequest.Customer.CustomerType.ToString(),
                            RecordId = bookingTeamRequest.CustomerResortTeamRequest.Customer.Id,
                            RecordOwner = bookingTeamRequest.CustomerResortTeamRequest.Owner
                        };
                        assignRequests.Add(assignCustomerRequest);
                    }                 
                }

                if (assignRequests != null && assignRequests.Count > 0)
                    crmService.BulkAssign(assignRequests);
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
