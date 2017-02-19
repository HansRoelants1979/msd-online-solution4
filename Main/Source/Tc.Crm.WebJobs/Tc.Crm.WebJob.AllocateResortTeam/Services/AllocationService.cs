using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Tc.Crm.WebJob.AllocateResortTeam.Models;

namespace Tc.Crm.WebJob.AllocateResortTeam.Services
{
    public class AllocationService : IAllocationService
    {
        ICrmService crmService;
        ILogger logger;
        public AllocationService(ILogger logger, ICrmService crmService)
        {
            this.logger = logger;
            this.crmService = crmService;
        }

        public IList<BookingAllocationResponse> GetBookingAllocations(BookingAllocationRequest bookingAllocationRequest)
        {
            IList <BookingAllocationResponse> bookingAllocationResponse = null;
            if (bookingAllocationRequest != null)
            {
                if (bookingAllocationRequest.Destination != null)
                {
                    var destinationGateWays = GetDestinationGateways(bookingAllocationRequest.Destination);

                    var query = string.Format(@"<fetch distinct='true' version='1.0' output-format='xml-platform' mapping='logical'>
                                                 <entity name='tc_booking'>
                                                    <attribute name='tc_bookingid'/>
                                                    <attribute name='tc_name'/>
                                                    <attribute name='ownerid'/>
                                                    <order attribute='tc_name' descending='false'/>
                                                    <filter type='and'>
                                                        <filter type='and'>
                                                                <filter type='or'>
                                                                <condition attribute='tc_departuredate' operator='next-x-days' value='{0}'/>
                                                                    <filter type='and'>
                                                                        <condition attribute='tc_departuredate' operator='on-or-before' value='{1}'/>
                                                                        <condition attribute='tc_returndate' operator='on-or-after' value='{2}'/>
                                                                    </filter>
                                                                </filter>
                                                            <condition attribute='tc_destinationgatewayid' operator='in'>
                                                            {3}
                                                            </condition>
                                                        </filter>
                                                    </filter>
                                                    <link-entity name='tc_bookingaccommodation' alias='accommodation' from='tc_bookingid' to='tc_bookingid'>
                                                        <attribute name='tc_startdateandtime'/>
                                                        <attribute name='tc_enddateandtime'/>
                                                            <link-entity name='tc_hotel' alias='hotel' from='tc_hotelid' to='tc_hotelid'>
                                                                <attribute name='ownerid'/>
                                                            </link-entity>
                                                    </link-entity>
                                                    <link-entity name='tc_customerbookingrole' alias='role' from='tc_bookingid' to='tc_bookingid'>
                                                        <attribute name='tc_customer'/>
                                                    </link-entity>
                                                 </entity>
                                                 </fetch>", 
                                                 new object[] { bookingAllocationRequest.DepartureDateinNextXDays,
                                                     bookingAllocationRequest.DepartureDate,
                                                     bookingAllocationRequest.ReturnDate,
                                                     destinationGateWays.ToString() });

                    EntityCollection bookings = crmService.RetrieveMultipleRecordsFetchXml(query);

                    bookingAllocationResponse = PrepareBookingAllocation(bookings);
                }
            }
            return bookingAllocationResponse;
        }

        public StringBuilder GetDestinationGateways(IList<Guid> destinationGateways)
        {
            StringBuilder gateways = new StringBuilder();
            if (destinationGateways != null && destinationGateways.Count > 0)
            {                            
                for (int i = 0; i < destinationGateways.Count; i++)
                {
                    gateways.Append("<value>" + destinationGateways[i].ToString() + "</value>");
                }                
            }
            return gateways;
        }

        public IList<BookingAllocationResponse> PrepareBookingAllocation(EntityCollection bookingCollection)
        {
            string accommodationAliasName = "accommodation.";
            string hotelAliasName = "hotel.";
            string roleAliasName = "role.";
            IList<BookingAllocationResponse> bookingAllocationResponse = null;
            if (bookingCollection != null && bookingCollection.Entities.Count > 0)
            {
                bookingAllocationResponse = new List<BookingAllocationResponse>();
                for (int i=0; i<bookingCollection.Entities.Count; i++)
                {
                    var booking = bookingCollection.Entities[i];
                    var response = new BookingAllocationResponse();
                    if (booking.Attributes.Contains(Attributes.Booking.BookingId) && booking.Attributes[Attributes.Booking.BookingId] != null)
                        response.BookingId = Guid.Parse(booking.Attributes[Attributes.Booking.BookingId].ToString());
                    if (booking.Attributes.Contains(accommodationAliasName + Attributes.BookingAccommodation.StartDateandTime) && booking.Attributes[accommodationAliasName + Attributes.BookingAccommodation.StartDateandTime] != null)
                        response.AccommodationStartDate = DateTime.Parse(((AliasedValue)booking.Attributes[accommodationAliasName + Attributes.BookingAccommodation.StartDateandTime]).Value.ToString());
                    if (booking.Attributes.Contains(accommodationAliasName + Attributes.BookingAccommodation.EndDateandTime) && booking.Attributes[accommodationAliasName + Attributes.BookingAccommodation.EndDateandTime] != null)
                        response.AccommodationEndDate = DateTime.Parse(((AliasedValue)booking.Attributes[accommodationAliasName + Attributes.BookingAccommodation.EndDateandTime]).Value.ToString());
                    if (booking.Attributes.Contains(hotelAliasName + Attributes.Hotel.Owner) && booking.Attributes[hotelAliasName + Attributes.Hotel.Owner] != null)
                    {
                        EntityReference owner = (EntityReference)((AliasedValue)booking.Attributes[hotelAliasName + Attributes.Hotel.Owner]).Value;
                        OwnerType ownerType;

                        if (owner.LogicalName == EntityName.User)
                            ownerType = OwnerType.User;
                        else
                            ownerType = OwnerType.Team;

                        response.OwnerId = new Owner() { Id = owner.Id, Name = owner.Name, OwnerType = ownerType };
                    }
                    if (booking.Attributes.Contains(roleAliasName + Attributes.CustomerBookingRole.Customer) && booking.Attributes[roleAliasName + Attributes.CustomerBookingRole.Customer] != null)
                    {
                        EntityReference customer = (EntityReference)((AliasedValue)booking.Attributes[roleAliasName + Attributes.CustomerBookingRole.Customer]).Value;
                        CustomerType customerType;

                        if (customer.LogicalName == EntityName.Contact)
                            customerType = CustomerType.Contact;
                        else
                            customerType = CustomerType.Account;

                        response.CustomerId = new Customer() { Id = customer.Id, Name = customer.Name, CustomerType = customerType };
                    }

                    bookingAllocationResponse.Add(response);
                }
            }

            return bookingAllocationResponse;
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
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AllocateService() {
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
