using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.Common;
using Tc.Crm.Common.Services;
using Tc.Crm.WebJob.AllocateResortTeam.Models;

namespace Tc.Crm.WebJob.AllocateResortTeam.Services
{
    public class AllocateResortTeamService : IAllocateResortTeamService
    {

        ILogger logger;
        IAllocationService allocationService;
        IConfigurationService configurationService;
        public AllocateResortTeamService(ILogger logger, IAllocationService allocationService, IConfigurationService configurationService)
        {
            this.logger = logger;
            this.allocationService = allocationService;
            this.configurationService = configurationService;
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetBookingAllocations")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Tc.Crm.WebJob.AllocateResortTeam.Services.ILogger.LogInformation(System.String)")]
        public void GetBookingAllocations()
        {
            logger.LogInformation("Executing GetBookingAllocations");
            IList<Guid> destinationGateways = GetDestinationGateways();
            if (destinationGateways == null || destinationGateways.Count == 0)
            {
                logger.LogInformation("No Gateways found to process");
                throw new InvalidOperationException("No Gateways found to process");
            }

            logger.LogInformation("Processing for " + destinationGateways.Count.ToString() + " Destination Gateways");
            var departureDateinNextXDays = configurationService.DepartureDateInNextXDays;

            IList<BookingAllocationResponse> bookingAllocationResponses
                = allocationService.GetBookingAllocations(new
                                                                BookingAllocationRequest
                {
                    DepartureDateInNextXDays = configurationService.DepartureDateInNextXDays,
                    DepartureDate = DateTime.Now.Date,
                    ReturnDate = DateTime.Now.Date,
                    Destination = destinationGateways
                });
            if (bookingAllocationResponses != null)
            {
                logger.LogInformation("bookingAllocationResponses.Count:" + bookingAllocationResponses.Count);
                if (bookingAllocationResponses.Count == 0)
                    return;
                IList<BookingAllocationResortTeamRequest> bookingAllocationResortTeamRequests = ProcessAllocationResponse(bookingAllocationResponses);
                if (bookingAllocationResortTeamRequests.Count == 0)
                    return;
                allocationService.ProcessBookingAllocations(bookingAllocationResortTeamRequests);
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

        public IList<BookingAllocationResortTeamRequest> ProcessAllocationResponse(IList<BookingAllocationResponse> bookingAllocationResponses)
        {
            logger.LogInformation("ProcessAllocationResponse - start");
            if (bookingAllocationResponses == null || bookingAllocationResponses.Count == 0) return null;

            var bookingAllocationResortTeamRequest = new List<BookingAllocationResortTeamRequest>();
            var currentBookingId = Guid.Empty;
            var currentAccommodationStartDate = (DateTime?)null;
            var previousAccommodationStartDate = (DateTime?)null;
            var previousBookingId = Guid.Empty;
            var previousCustomerId = Guid.Empty;
            var currentCustomerId = Guid.Empty;
            bool differentBooking, sameBookingDifferentCustomer;
            List<Guid> processedCustomer = new List<Guid>();
            for (int i = 0; i < bookingAllocationResponses.Count; i++)
            {
                if (bookingAllocationResponses[i] == null) continue;
                var bookingResponse = bookingAllocationResponses[i];
                if (bookingResponse.Customer == null) continue; 
                currentBookingId = bookingResponse.BookingId;
                currentCustomerId = bookingResponse.Customer.Id;
                currentAccommodationStartDate = bookingResponse.AccommodationStartDate;
              

                if (processedCustomer.Contains(currentCustomerId))
                {
                    logger.LogWarning("Customer " + bookingResponse.Customer.Name + " was already processed.");
                    continue;
                }
                if (currentAccommodationStartDate == null)
                {
                    logger.LogWarning("Accommodation StartDate is null");
                    continue;
                }

                differentBooking = (previousBookingId != currentBookingId);
                sameBookingDifferentCustomer = (previousBookingId == currentBookingId && previousAccommodationStartDate == currentAccommodationStartDate && previousCustomerId != currentCustomerId);

                if (differentBooking || sameBookingDifferentCustomer)
                {
                    if (bookingResponse.BookingOwner.OwnerType == OwnerType.User)
                    {
                        if (currentAccommodationStartDate.Value.Date >= DateTime.Now.Date)
                        {                            
                            WriteAllocationResponseLog(bookingResponse);
                            if (sameBookingDifferentCustomer)
                            { 
                                //Add only Customer
                                bookingAllocationResortTeamRequest.Add(PrepareCustomerResortTeamRequest(bookingResponse));
                            }
                            else
                            {
                                //Add Booking, Customer
                                AddResortTeamRequest(bookingResponse, bookingAllocationResortTeamRequest);
                            }
                            previousBookingId = currentBookingId;
                            previousCustomerId = currentCustomerId;
                            previousAccommodationStartDate = currentAccommodationStartDate;
                            processedCustomer.Add(currentCustomerId);
                        }
                    }
                } 
            }
            logger.LogInformation("ProcessAllocationResponse - end");
            return bookingAllocationResortTeamRequest;
        }

        public void WriteAllocationResponseLog(BookingAllocationResponse bookingAllocationResponse)
        {
            if (bookingAllocationResponse != null)
            {
                StringBuilder information = new StringBuilder();
                if (bookingAllocationResponse.BookingNumber != null)
                    information.AppendLine("****** Allocating Booking: " + bookingAllocationResponse.BookingNumber + " ******");
                if (bookingAllocationResponse.Customer != null)
                    information.AppendLine("Customer: " + bookingAllocationResponse.Customer.Name + " of Type " + bookingAllocationResponse.Customer.CustomerType.ToString());
                if (bookingAllocationResponse.AccommodationStartDate != null)
                    information.AppendLine("Accommodation Start Date: " + bookingAllocationResponse.AccommodationStartDate.Value.ToString());
                if (bookingAllocationResponse.BookingOwner != null)
                    information.AppendLine("Booking Owner: "+ bookingAllocationResponse.BookingOwner.Name + " of Type " + bookingAllocationResponse.BookingOwner.OwnerType.ToString());
                if (bookingAllocationResponse.HotelOwner != null)
                    information.AppendLine("Hotel Owner: " + bookingAllocationResponse.HotelOwner.Name + " of Type " + bookingAllocationResponse.HotelOwner.OwnerType.ToString());                

                logger.LogInformation(information.ToString());
            }

        }

        public void AddResortTeamRequest(BookingAllocationResponse bookingResponse, IList<BookingAllocationResortTeamRequest> bookingAllocationResortTeamRequests)
        {
            logger.LogInformation("AddResortTeamRequest - start");
            if (bookingAllocationResortTeamRequests == null)
                throw new ArgumentNullException("bookingAllocationResortTeamRequests");
            if (bookingResponse == null)
                throw new ArgumentNullException("bookingResponse");

            BookingAllocationResortTeamRequest bookingTeamRequest = PrepareResortTeamRequest(bookingResponse);
            if (bookingTeamRequest == null) return;

            bookingAllocationResortTeamRequests.Add(bookingTeamRequest);

            logger.LogInformation("AddResortTeamRequest - end");
        }

        public BookingAllocationResortTeamRequest PrepareResortTeamRequest(BookingAllocationResponse bookingResponse)
        {
            logger.LogInformation("PrepareResortTeamRequest - start");

            if (bookingResponse == null) throw new ArgumentNullException("bookingResponse");
            
            var bookingTeamRequest = new BookingAllocationResortTeamRequest();

            if (bookingResponse.BookingId != null && bookingResponse.HotelOwner != null)
            {
                bookingTeamRequest.BookingResortTeamRequest = new BookingResortTeamRequest
                {
                    Id = bookingResponse.BookingId,
                    Owner = bookingResponse.HotelOwner
                };
            }
            if (bookingResponse.Customer != null && bookingResponse.Customer.Id != null && bookingResponse.HotelOwner != null)
            {
                bookingTeamRequest.CustomerResortTeamRequest = new CustomerResortTeamRequest
                {
                    Customer = bookingResponse.Customer,
                    Owner = bookingResponse.HotelOwner
                };
            }
            logger.LogInformation("PrepareResortTeamRequest - end");
            return bookingTeamRequest;
        }

        public BookingAllocationResortTeamRequest PrepareCustomerResortTeamRequest(BookingAllocationResponse bookingResponse)
        {
            logger.LogInformation("PrepareCustomerResortTeamRequest - start");

            if (bookingResponse == null) throw new ArgumentNullException("bookingResponse");

            var bookingTeamRequest = new BookingAllocationResortTeamRequest();
          
            if (bookingResponse.Customer != null && bookingResponse.Customer.Id != null && bookingResponse.HotelOwner != null)
            {
                bookingTeamRequest.CustomerResortTeamRequest = new CustomerResortTeamRequest
                {
                    Customer = bookingResponse.Customer,
                    Owner = bookingResponse.HotelOwner
                };
            }
            logger.LogInformation("PrepareCustomerResortTeamRequest - end");
            return bookingTeamRequest;
        }

        public void Run()
        {
            GetBookingAllocations();
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
                DisposeObject(allocationService);
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
