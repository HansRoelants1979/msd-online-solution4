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
    class DeallocateResortTeamService : IDeallocateResortTeamService
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
            logger.LogInformation("Executing GetBookingAllocations");
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

                if (bookingDeallocationResponse != null)
                {
                    logger.LogInformation("bookingDeallocationResponse.Count:" + bookingDeallocationResponse.Count);
                    IList<BookingDeallocationResortTeamRequest> bookingDeallocationResortTeamRequest = ProcessDeallocationResponse(bookingDeallocationResponse);
                    deAllocationService.ProcessBookingAllocations(bookingDeallocationResortTeamRequest);
                }
                
            }
            else
            {
                logger.LogInformation("No Gateways found to process");
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
            logger.LogInformation("ProcessDeallocationResponse - start");
            IList<BookingDeallocationResortTeamRequest> bookingDeallocationResortTeamRequest = null;
            if (bookingDeallocationResponse != null && bookingDeallocationResponse.Count > 0)
            {
                bookingDeallocationResortTeamRequest = new List<BookingDeallocationResortTeamRequest>();
                List<Guid> processedCustomer = new List<Guid>();
                var previousCustomerId = Guid.Empty;
                var currentCustomerId = Guid.Empty;
                var currentAccommodationEndDate = (DateTime?)null;
                var previousAccommodationEndDate = (DateTime?)null;
                var currentBookingId = Guid.Empty;
                var previousBookingId = Guid.Empty;
                bool differentBooking, sameBookingDifferentCustomer;
                for (int i = 0; i < bookingDeallocationResponse.Count; i++)
                {
                    if (bookingDeallocationResponse[i] != null)
                    {
                        var bookingResponse = bookingDeallocationResponse[i];
                        if (bookingResponse.Customer == null) continue;
                        if (bookingResponse != null)
                        {
                            currentBookingId = bookingResponse.BookingId;
                            currentCustomerId = bookingResponse.Customer.Id;
                            currentAccommodationEndDate = bookingResponse.AccommodationEndDate;

                            if (processedCustomer.Contains(currentCustomerId))
                            {
                                logger.LogWarning("Customer " + bookingResponse.Customer.Name + " was already processed.");
                                continue;
                            }
                            if (currentAccommodationEndDate == null)
                            {
                                logger.LogWarning("Accommodation EndDate is null");
                                continue;
                            }
                            

                            differentBooking = (previousBookingId != currentBookingId);
                            sameBookingDifferentCustomer = (previousBookingId == currentBookingId && previousAccommodationEndDate == currentAccommodationEndDate && previousCustomerId != currentCustomerId);

                           
                            if (differentBooking || sameBookingDifferentCustomer)
                            {
                                if (bookingResponse.BookingOwner.OwnerType == OwnerType.Team)
                                {
                                    WriteDeallocationResponseLog(bookingResponse);
                                    if (sameBookingDifferentCustomer)
                                    {
                                        //Add only Customer
                                        bookingDeallocationResortTeamRequest.Add(PrepareCustomerResortTeamRemovalRequest(bookingResponse));
                                    }
                                    else
                                    {
                                        //Add Booking, Customer
                                        AddResortTeamRemovalRequest(bookingResponse, bookingDeallocationResortTeamRequest);
                                    }
                                    previousCustomerId = currentCustomerId;
                                    previousBookingId = currentBookingId;
                                    previousAccommodationEndDate = currentAccommodationEndDate;
                                    processedCustomer.Add(currentCustomerId);
                                }
                            }
                        }
                    }
                }
            }
            logger.LogInformation("ProcessDeallocationResponse - end");
            return bookingDeallocationResortTeamRequest;
        }

        public void WriteDeallocationResponseLog(BookingDeallocationResponse bookingDeallocationResponse)
        {
            if (bookingDeallocationResponse != null)
            {
                StringBuilder information = new StringBuilder();
                if (bookingDeallocationResponse.BookingNumber != null)
                    information.AppendLine("****** Deallocating Booking: " + bookingDeallocationResponse.BookingNumber + " ******");
                if (bookingDeallocationResponse.Customer != null)
                    information.AppendLine("Customer: " + bookingDeallocationResponse.Customer.Name + " of Type " + bookingDeallocationResponse.Customer.CustomerType.ToString());
                if (bookingDeallocationResponse.AccommodationEndDate != null)
                    information.AppendLine("Accommodation End Date: " + bookingDeallocationResponse.AccommodationEndDate.Value.ToString());                

                logger.LogInformation(information.ToString());
            }

        }

        public void AddResortTeamRemovalRequest(BookingDeallocationResponse bookingResponse, IList<BookingDeallocationResortTeamRequest> bookingDeallocationResortTeamRequest)
        {
            logger.LogInformation("AddResortTeamRemovalRequest - start");
            BookingDeallocationResortTeamRequest bookingTeamRequest = PrepareResortTeamRemovalRequest(bookingResponse);
            if (bookingTeamRequest != null && bookingDeallocationResortTeamRequest != null)
                bookingDeallocationResortTeamRequest.Add(bookingTeamRequest);
            logger.LogInformation("AddResortTeamRemovalRequest - end");
        }

        public BookingDeallocationResortTeamRequest PrepareCustomerResortTeamRemovalRequest(BookingDeallocationResponse bookingResponse)
        {
            logger.LogInformation("PrepareCustomerResortTeamRemovalRequest - start");
            BookingDeallocationResortTeamRequest bookingDeallocationResortTeamRequest = null;
            if (bookingResponse != null && configurationService.DefaultUserId != null)
            {
                bookingDeallocationResortTeamRequest = new BookingDeallocationResortTeamRequest();               
                if (bookingResponse.Customer != null && bookingResponse.Customer.Id != null)
                {
                    bookingDeallocationResortTeamRequest.CustomerResortTeamRequest = new CustomerResortTeamRequest
                    {
                        Customer = bookingResponse.Customer,
                        Owner = new Owner { Id = Guid.Parse(configurationService.DefaultUserId), OwnerType = OwnerType.User }
                    };
                }
            }
            logger.LogInformation("PrepareCustomerResortTeamRemovalRequest - end");
            return bookingDeallocationResortTeamRequest;
        }

        public BookingDeallocationResortTeamRequest PrepareResortTeamRemovalRequest(BookingDeallocationResponse bookingResponse)
        {
            logger.LogInformation("PrepareResortTeamRemovalRequest - start");
            BookingDeallocationResortTeamRequest bookingDeallocationResortTeamRequest = null;
            if (bookingResponse != null && configurationService.DefaultUserId != null)
            {
                bookingDeallocationResortTeamRequest = new BookingDeallocationResortTeamRequest();
                if (bookingResponse.BookingId != null)
                {
                    bookingDeallocationResortTeamRequest.BookingResortTeamRequest = new BookingResortTeamRequest
                    {
                        Id = bookingResponse.BookingId,
                        Owner = new Owner { Id= Guid.Parse(configurationService.DefaultUserId),OwnerType=OwnerType.User }
                    };
                }
                if (bookingResponse.Customer != null && bookingResponse.Customer.Id != null)
                {
                    bookingDeallocationResortTeamRequest.CustomerResortTeamRequest = new CustomerResortTeamRequest
                    {
                        Customer = bookingResponse.Customer,
                        Owner = new Owner { Id = Guid.Parse(configurationService.DefaultUserId), OwnerType = OwnerType.User }
                    };
                }
            }
            logger.LogInformation("PrepareResortTeamRemovalRequest - end");
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
