using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.Common;
using Tc.Crm.Common.Models;
using Tc.Crm.Common.Services;
using Tc.Crm.WebJob.DeallocateResortTeam.Models;

namespace Tc.Crm.WebJob.DeallocateResortTeam.Services
{
    public class DeallocateResortTeamService : IDeallocateResortTeamService
    {
        ILogger logger;
        IDeallocationService deAllocationService;
        IConfigurationService configurationService;
        public DeallocateResortTeamService(ILogger logger, IDeallocationService deAllocationService, IConfigurationService configurationService)
        {
            this.logger = logger;
            this.deAllocationService = deAllocationService;
            this.configurationService = configurationService;
        }

        public void Run()
        {
            GetBookingDeallocations();
        }

        public void GetBookingDeallocations()
        {
            //logger.LogInformation("Executing GetBookingDeallocations");
            IList<Guid> destinationGateways = GetDestinationGateways();
            if (destinationGateways != null && destinationGateways.Count > 0)
            {

                logger.LogInformation("Processing for " + destinationGateways.Count.ToString() + " Destination Gateways");

                IList<BookingDeallocationResponse> bookingDeallocationResponse = deAllocationService.GetBookingDeallocations(new
                                                                                BookingDeallocationRequest
                {
                    AccommodationEndDate = DateTime.Now.Date,
                    Destination = destinationGateways
                });

                if (bookingDeallocationResponse == null) return;

                logger.LogInformation("Processing booking deallocations: " + bookingDeallocationResponse.Count);
                IList<BookingDeallocationResortTeamRequest> bookingDeallocationResortTeamRequest = ProcessDeallocationResponse(bookingDeallocationResponse);
                if(bookingDeallocationResortTeamRequest == null || bookingDeallocationResortTeamRequest.Count == 0)
                {
                    logger.LogInformation("No booking records matched the criteria to assign it to Default user");
                    return;
                }
                deAllocationService.ProcessBookingDeallocations(bookingDeallocationResortTeamRequest);
            }
            else
            {
                logger.LogWarning("No Gateways found to process");
                throw new InvalidOperationException("No Gateways found to process");
            }

        }

        public IList<Guid> GetDestinationGateways()
        {
            IList<Guid> destinationGateways = null;
            if (configurationService.DestinationGatewayIds != null)
            {
                var ids = configurationService.DestinationGatewayIds.Split(',');
                if (ids != null && ids.Length > 0)
                {
                    var guids = Array.ConvertAll(ids, delegate (string stringID) { return new Guid(stringID); });
                    destinationGateways = guids.ToList();
                }
            }
            return destinationGateways;
        }

        public IList<BookingDeallocationResortTeamRequest> ProcessDeallocationResponse(IList<BookingDeallocationResponse> bookingDeallocationResponse)
        {
            //logger.LogInformation("ProcessDeallocationResponse - start");
            if (bookingDeallocationResponse == null || bookingDeallocationResponse.Count == 0)
                return null;

            var bookingDeallocationResortTeamRequest = new List<BookingDeallocationResortTeamRequest>();
            var processedCustomers = new List<Guid>();
            var processedBookings = new List<Guid>();

            for (int i = 0; i < bookingDeallocationResponse.Count; i++)
            {
                var bookingResponse = bookingDeallocationResponse[i];
                var responseLog = WriteDeallocationResponseLog(bookingResponse);
                if (!ValidForProcessing(bookingResponse, processedCustomers, responseLog)) continue;

                var differentBooking = !processedBookings.Contains(bookingResponse.BookingId);

                var sameBookingDifferentCustomer = (processedBookings.Contains(bookingResponse.BookingId)
                                                    && !processedCustomers.Contains(bookingResponse.Customer.Id));


                if (!differentBooking && !sameBookingDifferentCustomer)
                {
                    logger.LogInformation(responseLog+"Not processing this record as the booking was already processed");
                    continue;
                }

               
                if (sameBookingDifferentCustomer)
                {
                    //Add only Customer
                    bookingDeallocationResortTeamRequest.Add(new BookingDeallocationResortTeamRequest()
                    {
                        CustomerResortTeamRequest = PrepareCustomerResortTeamRemovalRequest(bookingResponse)
                    });
                    logger.LogInformation(responseLog+"<<Processing only customer record as the booking was already processed>>");
                }
                else if (differentBooking)
                {
                    //Add Booking, Customer
                    AddResortTeamRemovalRequest(bookingResponse, bookingDeallocationResortTeamRequest);
                    logger.LogInformation(responseLog+"<<Processing both booking and customer records>>");
                }

                processedCustomers.Add(bookingResponse.Customer.Id);
                processedBookings.Add(bookingResponse.BookingId);
            }
            //logger.LogInformation("ProcessDeallocationResponse - end");
            return bookingDeallocationResortTeamRequest;
        }

        public bool ValidForProcessing(BookingDeallocationResponse bookingResponse, List<Guid> processedCustomers, string responseLog)
        {
            if (bookingResponse == null) return false;
            if (bookingResponse.Customer == null)
            {
                logger.LogWarning(responseLog+"Not processing this record as no customer exists");
                return false;
            }
            if (bookingResponse.BookingOwner.OwnerType == OwnerType.User)
            {
                logger.LogInformation(responseLog+"Not processing this record as the booking owner type is user");
                return false;
            }
            if (bookingResponse.Customer.Owner == null) return false;
            if (bookingResponse.Customer.Owner.OwnerType == OwnerType.User)
            {
                logger.LogInformation(responseLog+"Not processing this record as the customer owner type is user");
                return false;
            }

            //customer already deallocated
            if (processedCustomers.Contains(bookingResponse.Customer.Id))
            {
                logger.LogWarning(responseLog+"Not processing this record as customer: " + bookingResponse.Customer.Name + " was already got processed.");
                return false;
            }
            //accommodation end date is not provided
            if (bookingResponse.AccommodationEndDate == null)
            {
                logger.LogWarning(responseLog+"Not processing this record as accommodation enddate is null");
                return false;
            }
            return true;
        }

        public string WriteDeallocationResponseLog(BookingDeallocationResponse bookingDeallocationResponse)
        {
            var information = new StringBuilder();
            if (bookingDeallocationResponse != null)
            {
               
                information.AppendLine();
                information.AppendLine();
                if (bookingDeallocationResponse.BookingNumber != null)
                    information.AppendLine("Processing Booking: " + bookingDeallocationResponse.BookingNumber);
                if (bookingDeallocationResponse.BookingOwner != null)
                    information.AppendLine("Booking Owner: " + bookingDeallocationResponse.BookingOwner.Name + " of Type " + bookingDeallocationResponse.BookingOwner.OwnerType.ToString());
                if (bookingDeallocationResponse.AccommodationEndDate != null)
                    information.AppendLine("Accommodation End Date: " + bookingDeallocationResponse.AccommodationEndDate.Value.ToString());
                if (bookingDeallocationResponse.Customer != null)
                {
                    information.AppendLine("Booking Customer: " + bookingDeallocationResponse.Customer.Name + " of type " + bookingDeallocationResponse.Customer.CustomerType.ToString());
                    information.AppendLine("Customer Owner: " + bookingDeallocationResponse.Customer.Owner.Name + " of type " + bookingDeallocationResponse.Customer.Owner.OwnerType.ToString());
                }
            }
            return information.ToString();
        }

        public void AddResortTeamRemovalRequest(BookingDeallocationResponse bookingResponse, IList<BookingDeallocationResortTeamRequest> bookingDeallocationResortTeamRequest)
        {
            //logger.LogInformation("AddResortTeamRemovalRequest - start");
            var bookingTeamRequest = PrepareResortTeamRemovalRequest(bookingResponse);
            if (bookingTeamRequest != null && bookingDeallocationResortTeamRequest != null)
                bookingDeallocationResortTeamRequest.Add(bookingTeamRequest);
            //logger.LogInformation("AddResortTeamRemovalRequest - end");
        }

        public CustomerResortTeamRequest PrepareCustomerResortTeamRemovalRequest(BookingDeallocationResponse bookingResponse)
        {
            //logger.LogInformation("PrepareCustomerResortTeamRemovalRequest - start");
            //guard clause
            if (bookingResponse == null) return null;
            if (bookingResponse.Customer == null || bookingResponse.Customer.Id == Guid.Empty) return null;
            var customerResortTeamRequest = new CustomerResortTeamRequest
            {
                Customer = bookingResponse.Customer,
                Owner = new Owner { Id = configurationService.DefaultUserId, OwnerType = OwnerType.User, Name=configurationService.DefaultUserName }
            };
            //logger.LogInformation("PrepareCustomerResortTeamRemovalRequest - end");
            return customerResortTeamRequest;
        }

        public BookingResortTeamRequest PrepareBookingResortTeamRemovalRequest(BookingDeallocationResponse bookingResponse)
        {
            //logger.LogInformation("PrepareCustomerResortTeamRemovalRequest - start");
            if (bookingResponse == null) return null;
            if (bookingResponse.BookingId == null || bookingResponse.BookingId == Guid.Empty) return null;

            var bookingResortTeamRequest = new BookingResortTeamRequest
            {
                Id = bookingResponse.BookingId,
                Name = bookingResponse.BookingNumber,
                Owner = new Owner { Id = configurationService.DefaultUserId, OwnerType = OwnerType.User, Name = configurationService.DefaultUserName }
            };

            //logger.LogInformation("PrepareCustomerResortTeamRemovalRequest - end");
            return bookingResortTeamRequest;
        }

        public BookingDeallocationResortTeamRequest PrepareResortTeamRemovalRequest(BookingDeallocationResponse bookingResponse)
        {
            //logger.LogInformation("PrepareResortTeamRemovalRequest - start");
            var bookingDeallocationResortTeamRequest = new BookingDeallocationResortTeamRequest()
            {
                CustomerResortTeamRequest = PrepareCustomerResortTeamRemovalRequest(bookingResponse),
                BookingResortTeamRequest = PrepareBookingResortTeamRemovalRequest(bookingResponse)
            };
            //logger.LogInformation("PrepareResortTeamRemovalRequest - end");
            return bookingDeallocationResortTeamRequest;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                DisposeObject(deAllocationService);
                DisposeObject(logger);
                DisposeObject(configurationService);
            }

            disposed = true;
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
    }
}
