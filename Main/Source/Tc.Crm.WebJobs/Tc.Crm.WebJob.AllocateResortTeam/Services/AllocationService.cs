using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using Tc.Crm.Common.Constants.Attributes;
using Tc.Crm.Common.Constants;
using Tc.Crm.Common.Services;
using Tc.Crm.Common;
using Tc.Crm.Common.Models;
using Tc.Crm.WebJob.AllocateResortTeam.Models;
using Microsoft.Xrm.Sdk;

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetBookingAllocations")]
        public IList<BookingAllocationResponse> GetBookingAllocations(BookingAllocationRequest bookingAllocationRequest)
        {
            //logger.LogInformation("GetBookingAllocations - start");
            if (bookingAllocationRequest == null) throw new ArgumentNullException("bookingAllocationRequest");

            if (bookingAllocationRequest.Destination == null) throw new ArgumentNullException("bookingAllocationRequest.Destination");

            var destinationGateWays = GetDestinationGateways(bookingAllocationRequest.Destination);
            if (bookingAllocationRequest.DepartureDate == null || bookingAllocationRequest.ReturnDate == null)
                throw new ArgumentNullException("bookingAllocationRequest.DepartureDate and bookingAllocationRequest.ReturnDate");

            var query = string.Format(@"<fetch version='1.0' output-format='xml-platform' mapping='logical'>
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
                                                        <order attribute='tc_startdateandtime' descending='false'/>
                                                            <link-entity name='tc_hotel' alias='hotel' from='tc_hotelid' to='tc_hotelid'>
                                                                <attribute name='ownerid'/>
                                                            </link-entity>
                                                    </link-entity>
                                                    <link-entity name='tc_customerbookingrole' alias='role' from='tc_bookingid' to='tc_bookingid'>
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
                                         new object[] { bookingAllocationRequest.DepartureDateInNextXDays,
                                                     bookingAllocationRequest.DepartureDate.ToString("yyyy-MM-dd"),
                                                     bookingAllocationRequest.ReturnDate.ToString("yyyy-MM-dd"),
                                                     destinationGateWays.ToString() });

            EntityCollection bookingCollection = crmService.RetrieveMultipleRecordsFetchXml(query);

            //logger.LogInformation("GetBookingAllocations - end");
            return PrepareBookingAllocation(bookingCollection);
        }

        public StringBuilder GetDestinationGateways(IList<Guid> destinationGateways)
        {
            //logger.LogInformation("GetDestinationGateways - start");
            StringBuilder gateways = new StringBuilder();
            if (destinationGateways == null || destinationGateways.Count == 0)
                throw new ArgumentNullException("destinationGateways");

            for (int i = 0; i < destinationGateways.Count; i++)
            {
                gateways.Append("<value>" + destinationGateways[i].ToString() + "</value>");
            }
            //logger.LogInformation("GetDestinationGateways - end");
            return gateways;
        }

        public IList<BookingAllocationResponse> PrepareBookingAllocation(EntityCollection bookingCollection)
        {
            //logger.LogInformation("PrepareBookingAllocation - start");
            if (bookingCollection == null || bookingCollection.Entities.Count == 0)
            {
                logger.LogWarning("No booking records found to process in CRM for the schedule.");
                return null;
            }

            var bookingAllocationResponse = new List<BookingAllocationResponse>();
            for (int i = 0; i < bookingCollection.Entities.Count; i++)
            {
                var booking = bookingCollection.Entities[i];
                if (booking == null) continue;

                var response = new BookingAllocationResponse();
                if (booking.Contains(Booking.BookingId) && booking[Booking.BookingId] != null)
                    response.BookingId = Guid.Parse(booking.Attributes[Booking.BookingId].ToString());
                if (booking.Contains(Booking.Name) && booking.Attributes[Booking.Name] != null)
                    response.BookingNumber = booking.Attributes[Booking.Name].ToString();
               
                    response.BookingOwner = GetOwner(booking, Booking.Owner, false);               

                var fieldStartDate = AliasName.AccommodationAliasName + BookingAccommodation.StartDateandTime;
                var fieldEndDate = AliasName.AccommodationAliasName + BookingAccommodation.EndDateandTime;
                var fieldOwner = AliasName.HotelAliasName + Hotel.Owner;
                var fieldCustomer = AliasName.RoleAliasName + CustomerBookingRole.Customer;
                var fieldAccountOwner = AliasName.AccountAliasName + Common.Constants.Attributes.Customer.Owner;
                var fieldContactOwner = AliasName.ContactAliasName + Common.Constants.Attributes.Customer.Owner;

                if (booking.Contains(fieldStartDate) && booking[fieldStartDate] != null)
                    response.AccommodationStartDate = DateTime.Parse(((AliasedValue)booking[fieldStartDate]).Value.ToString());
                if (booking.Contains(fieldEndDate) && booking[fieldEndDate] != null)
                    response.AccommodationEndDate = DateTime.Parse(((AliasedValue)booking[fieldEndDate]).Value.ToString());
                 
                    response.HotelOwner = GetOwner(booking, fieldOwner, true);
                
                if (booking.Contains(fieldCustomer) && booking[fieldCustomer] != null)
                {
                    var customer = (EntityReference)((AliasedValue)booking[fieldCustomer]).Value;
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
                    

                    response.Customer = new Common.Models.Customer() { Id = customer.Id, Name = customer.Name, CustomerType = customerType,Owner= owner };
                }


                bookingAllocationResponse.Add(response);


            }
            //logger.LogInformation("PrepareBookingAllocation - end");
            return bookingAllocationResponse;
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


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ProcessBookingAllocations")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Tc.Crm.WebJob.AllocateResortTeam.Services.ILogger.LogInformation(System.String)")]
        public void ProcessBookingAllocations(IList<BookingAllocationResortTeamRequest> bookingAllocationResortTeamRequests)
        {
            //logger.LogInformation("ProcessBookingAllocations - start");
            if (bookingAllocationResortTeamRequests == null || bookingAllocationResortTeamRequests.Count == 0)
                return;

            var assignRequests = new Collection<AssignInformation>();

            for (int i = 0; i < bookingAllocationResortTeamRequests.Count; i++)
            {
                if (bookingAllocationResortTeamRequests[i] == null) continue;

                var bookingTeamRequest = bookingAllocationResortTeamRequests[i];
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

            //logger.LogInformation("ProcessBookingAllocations - end");
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
