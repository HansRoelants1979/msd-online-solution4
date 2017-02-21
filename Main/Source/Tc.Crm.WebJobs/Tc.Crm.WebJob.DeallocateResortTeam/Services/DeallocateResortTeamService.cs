using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            GetBookingAllocations();
        }

        public void GetBookingAllocations()
        {
            logger.LogInformation("Executing GetBookingAllocations");
            IList<Guid> destinationGateways = GetDestinationGateways();
            if (destinationGateways != null && destinationGateways.Count > 0)
            {
                logger.LogInformation("Processing for " + destinationGateways.Count.ToString() + " Destination Gateways");
                
                IList<BookingDeallocationResponse> bookingDeallocationResponse = deAllocationService.GetBookingAllocations(new
                                                                                BookingDeallocationRequest
                { 
                    Destination = destinationGateways
                });

                IList<BookingDeallocationResortTeamRequest> bookingAllocationResortTeamRequest = ProcessDeallocationResponse(bookingDeallocationResponse);
                deAllocationService.ProcessBookingAllocations(bookingAllocationResortTeamRequest);
                
            }
            else
            {
                logger.LogInformation("No Gateways found to process");
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
            IList<BookingDeallocationResortTeamRequest> bookingDeallocationResortTeamRequest = null;
            if (bookingDeallocationResponse != null && bookingDeallocationResponse.Count > 0)
            {
                bookingDeallocationResortTeamRequest = new List<BookingDeallocationResortTeamRequest>();
                var currentBookingId = Guid.Empty;
                var currentAccommodationStartDate = (DateTime?)null;
                var currentAccommodationEndDate = (DateTime?)null;
                for (int i = 0; i < bookingDeallocationResponse.Count; i++)
                {
                    if (bookingDeallocationResponse[i] != null)
                    {
                        var bookingResponse = bookingDeallocationResponse[i];
                        if (bookingResponse != null)
                        {
                           
                        }
                    }
                }
            }
            return bookingDeallocationResortTeamRequest;
        }

        public void AddResortTeamRequest(BookingDeallocationResponse bookingResponse, IList<BookingDeallocationResortTeamRequest> bookingDeallocationResortTeamRequest)
        {
            BookingDeallocationResortTeamRequest bookingTeamRequest = PrepareResortTeamRequest(bookingResponse);
            if (bookingTeamRequest != null && bookingDeallocationResortTeamRequest != null)
                bookingDeallocationResortTeamRequest.Add(bookingTeamRequest);
        }

        public BookingDeallocationResortTeamRequest PrepareResortTeamRequest(BookingDeallocationResponse bookingResponse)
        {
            BookingDeallocationResortTeamRequest bookingTeamRequest = null;
            if (bookingResponse != null)
            {
                bookingTeamRequest = new BookingDeallocationResortTeamRequest();
                if (bookingResponse.BookingId != null)
                {
                    bookingTeamRequest.BookingResortTeamRequest = new BookingResortTeamRequest
                    {
                        Id = bookingResponse.BookingId,
                        Owner = new Owner { Id= Guid.Empty,OwnerType=OwnerType.User }
                    };
                }
                if (bookingResponse.Customer != null && bookingResponse.Customer.Id != null)
                {
                    bookingTeamRequest.CustomerResortTeamRequest = new CustomerResortTeamRequest
                    {
                        Customer = bookingResponse.Customer,
                        Owner = new Owner { Id = Guid.Empty, OwnerType = OwnerType.User }
                    };
                }
            }
            return bookingTeamRequest;
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
