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

        public IList<BookingDeallocationResponse> GetBookingAllocations(BookingDeallocationRequest bookingDeallocationRequest)
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
                                                    <order descending='false' attribute='tc_name'/>
                                                    <filter type='and'>
                                                    <condition attribute='tc_destinationgatewayid' operator='in'>
                                                    {1}
                                                    </condition>
                                                    </filter>
                                                      <link-entity name='tc_bookingaccommodation' alias='accommodation' to='tc_bookingid' from='tc_bookingid'>
                                                        <attribute name='tc_enddateandtime'/>
                                                        <filter type='and'>
                                                          <condition attribute='tc_enddateandtime' value='{0}' operator='on'/>
                                                        </filter>
                                                        <link-entity name='tc_hotel' alias='hotel' to='tc_hotelid' from='tc_hotelid'>
                                                            <attribute name='tc_hotelid'/>
                                                        </link-entity>
                                                        </link-entity>
                                                        <link-entity name='tc_customerbookingrole' alias='role' to='tc_bookingid' from='tc_bookingid'>
                                                            <attribute name='tc_customer'/>
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
            IList<BookingDeallocationResponse> bookingAllocationResponse = null;
            if (bookingCollection != null && bookingCollection.Entities.Count > 0)
            {
                bookingAllocationResponse = new List<BookingDeallocationResponse>();
                for (int i = 0; i < bookingCollection.Entities.Count; i++)
                {
                    var booking = bookingCollection.Entities[i];
                    if (booking != null)
                    {
                        var response = new BookingDeallocationResponse();
                        if (booking.Contains(Booking.BookingId) && booking[Booking.BookingId] != null)
                            response.BookingId = Guid.Parse(booking[Booking.BookingId].ToString());

                        if (booking.Contains(AliasName.HotelAliasName + Hotel.HotelId) && booking[AliasName.HotelAliasName + Hotel.HotelId] != null)
                            response.HotelId = Guid.Parse(((AliasedValue)booking[AliasName.HotelAliasName + Hotel.HotelId]).Value.ToString());

                        if (booking.Contains(AliasName.AccommodationAliasName + BookingAccommodation.EndDateandTime) && booking[AliasName.AccommodationAliasName + BookingAccommodation.EndDateandTime] != null)
                            response.AccommodationEndDate = DateTime.Parse(((AliasedValue)booking[AliasName.AccommodationAliasName + BookingAccommodation.EndDateandTime]).Value.ToString());

                        if (booking.Contains(AliasName.RoleAliasName + CustomerBookingRole.Customer) && booking[AliasName.RoleAliasName + CustomerBookingRole.Customer] != null)
                        {
                            EntityReference customer = (EntityReference)((AliasedValue)booking[AliasName.RoleAliasName + CustomerBookingRole.Customer]).Value;
                            CustomerType customerType;

                            if (customer.LogicalName == EntityName.Contact)
                                customerType = CustomerType.Contact;
                            else
                                customerType = CustomerType.Account;

                            response.Customer = new Common.Models.Customer() { Id = customer.Id, Name = customer.Name, CustomerType = customerType };
                        }

                        bookingAllocationResponse.Add(response);
                    }

                }
            }
            logger.LogInformation("PrepareBookingDeallocation - end");
            return bookingAllocationResponse;
        }

        public void ProcessBookingAllocations(IList<BookingDeallocationResortTeamRequest> bookingDeallocationResortTeamRequest)
        {
            EntityCollection bookingTeamCollection = null;
            if (bookingDeallocationResortTeamRequest != null && bookingDeallocationResortTeamRequest.Count > 0)
            {
                var assignRequests = new Collection<AssignInformation>();

                for (int i = 0; i < bookingDeallocationResortTeamRequest.Count; i++)
                {
                    if (bookingDeallocationResortTeamRequest[i] == null) continue;

                    var bookingTeamRequest = bookingDeallocationResortTeamRequest[i];
                    var assignBookingRequest = new AssignInformation
                    {
                        EntityName = EntityName.Booking,
                        RecordId = bookingTeamRequest.BookingResortTeamRequest.Id,
                        RecordOwner = bookingTeamRequest.BookingResortTeamRequest.Owner
                    };
                    assignRequests.Add(assignBookingRequest);
                    var assignCustomerRequest = new AssignInformation
                    {
                        EntityName = bookingTeamRequest.CustomerResortTeamRequest.Customer.CustomerType.ToString(),
                        RecordId = bookingTeamRequest.CustomerResortTeamRequest.Customer.Id,
                        RecordOwner = bookingTeamRequest.CustomerResortTeamRequest.Owner
                    };
                    assignRequests.Add(assignCustomerRequest);
                    AddBookingResortTeamRequest(bookingTeamRequest.BookingResortTeamRequest, bookingTeamCollection);
                    AddCustomerResortTeamRequest(bookingTeamRequest.CustomerResortTeamRequest, bookingTeamCollection);
                }

                if (assignRequests != null && assignRequests.Count > 0)
                    crmService.BulkAssign(assignRequests);
            }
        }

        public void AddBookingResortTeamRequest(BookingResortTeamRequest bookingResortTeamRequest, EntityCollection bookingTeamCollection)
        {
            if (bookingResortTeamRequest != null && bookingResortTeamRequest.Id != null)
            {
                var bookingEntity = new Entity(EntityName.Booking, bookingResortTeamRequest.Id);
                if (bookingResortTeamRequest.Owner != null && bookingResortTeamRequest.Owner.Id != null)
                {
                    if (bookingResortTeamRequest.Owner.OwnerType == OwnerType.Team)
                        bookingEntity.Attributes[Booking.Owner] = new EntityReference(EntityName.Team, bookingResortTeamRequest.Owner.Id);
                    else
                        bookingEntity.Attributes[Booking.Owner] = new EntityReference(EntityName.User, bookingResortTeamRequest.Owner.Id);

                    if (bookingTeamCollection != null)
                        bookingTeamCollection.Entities.Add(bookingEntity);
                }
            }
        }

        public void AddCustomerResortTeamRequest(CustomerResortTeamRequest customerResortTeamRequest, EntityCollection bookingTeamCollection)
        {
            if (customerResortTeamRequest != null)
            {
                if (customerResortTeamRequest.Customer != null && customerResortTeamRequest.Customer.Id != null)
                {
                    Entity customerEntity = null;
                    if (customerResortTeamRequest.Customer.CustomerType == CustomerType.Account)
                        customerEntity = new Entity(EntityName.Account, customerResortTeamRequest.Customer.Id);
                    else if (customerResortTeamRequest.Customer.CustomerType == CustomerType.Contact)
                        customerEntity = new Entity(EntityName.Contact, customerResortTeamRequest.Customer.Id);

                    if (customerEntity != null)
                    {
                        if (customerResortTeamRequest.Owner != null && customerResortTeamRequest.Owner.Id != null)
                        {
                            if (customerResortTeamRequest.Owner.OwnerType == OwnerType.Team)
                                customerEntity.Attributes[Common.Constants.Attributes.Customer.Owner] = new EntityReference(EntityName.Team, customerResortTeamRequest.Owner.Id);
                            else
                                customerEntity.Attributes[Common.Constants.Attributes.Customer.Owner] = new EntityReference(EntityName.User, customerResortTeamRequest.Owner.Id);

                            if (bookingTeamCollection != null)
                                bookingTeamCollection.Entities.Add(customerEntity);
                        }
                    }

                }

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
