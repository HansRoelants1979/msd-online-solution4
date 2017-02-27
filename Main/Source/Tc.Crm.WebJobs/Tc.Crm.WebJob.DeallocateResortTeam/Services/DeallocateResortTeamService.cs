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
                var currentAccommodationEndDate = (DateTime?)null;
                List<Guid> processedCustomer = new List<Guid>();
                for (int i = 0; i < bookingDeallocationResponse.Count; i++)
                {
                    if (bookingDeallocationResponse[i] != null)
                    {
                        var bookingResponse = bookingDeallocationResponse[i];
                        if (bookingResponse != null)
                        {
                            currentAccommodationEndDate = bookingResponse.AccommodationEndDate;
                            if (currentAccommodationEndDate.Value.Date == DateTime.Now.Date)
                            {
                                if (bookingResponse.Customer != null && bookingResponse.Customer.Id != null && !processedCustomer.Contains(bookingResponse.Customer.Id))
                                {
                                    AddResortTeamRemovalRequest(bookingResponse, bookingDeallocationResortTeamRequest);
                                    processedCustomer.Add(bookingResponse.Customer.Id);
                                }                             
                            }
                        }
                    }
                }
            }
            logger.LogInformation("ProcessDeallocationResponse - end");
            return bookingDeallocationResortTeamRequest;
        }

        public void AddResortTeamRemovalRequest(BookingDeallocationResponse bookingResponse, IList<BookingDeallocationResortTeamRequest> bookingDeallocationResortTeamRequest)
        {
            logger.LogInformation("AddResortTeamRequest - start");
            BookingDeallocationResortTeamRequest bookingTeamRequest = PrepareResortTeamRemovalRequest(bookingResponse);
            if (bookingTeamRequest != null && bookingDeallocationResortTeamRequest != null)
                bookingDeallocationResortTeamRequest.Add(bookingTeamRequest);
            logger.LogInformation("AddResortTeamRequest - end");
        }

        public BookingDeallocationResortTeamRequest PrepareResortTeamRemovalRequest(BookingDeallocationResponse bookingResponse)
        {
            logger.LogInformation("PrepareResortTeamRequest - start");
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
            logger.LogInformation("PrepareResortTeamRequest - end");
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
