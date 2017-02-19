using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.WebJob.AllocateResortTeam.Models;

namespace Tc.Crm.WebJob.AllocateResortTeam.Services
{
    public class AllocateResortTeamService : IAllocateResortTeamService
    {
        
        ILogger logger;
        IAllocationService allocationService;
        IConfigurationService configurationService;
        public AllocateResortTeamService(ILogger logger, IAllocationService allocationService,IConfigurationService configurationService)
        {
            this.logger = logger;
            this.allocationService = allocationService;
            this.configurationService = configurationService;
        }
        public void AllocateBookingToResortTeam(IList<BookingAllocationResortTeamRequest> bookingAllocationResortTeamRequest)
        {
            
        }

        public void GetBookingAllocations()
        {
            logger.LogInformation("Executing GetBookingAllocations");
            IList<BookingAllocationResponse> bookingAllocationResponse = allocationService.GetBookingAllocations(new
                                                                            BookingAllocationRequest
            {
                DepartureDateinNextXDays = int.Parse(configurationService.DepartureDateinNextXDays),
                DepartureDate = DateTime.Now.Date,
                ReturnDate = DateTime.Now.Date,
                Destination = GetDestinationGateways()
            });
                                                                            

            if (bookingAllocationResponse != null && bookingAllocationResponse.Count > 0)
            {
                ProcessAllocationResponse(bookingAllocationResponse);
            }
        }

        public IList<Guid> GetDestinationGateways()
        {
            IList<Guid> destinationGateways = new List<Guid>();
            if (configurationService.DestinationGatewayIds != null)
            {
                var ids = configurationService.DestinationGatewayIds.Split(',');  
                var guids = Array.ConvertAll(ids, delegate (string stringID) { return new Guid(stringID); });
                destinationGateways = guids.ToList();
            }
            return destinationGateways;
        }

        public IList<BookingAllocationResortTeamRequest> ProcessAllocationResponse(IList<BookingAllocationResponse> bookingAllocationResponse)
        {
            IList<BookingAllocationResortTeamRequest> bookingAllocationResortTeamRequest = new List<BookingAllocationResortTeamRequest>();
            if (bookingAllocationResponse != null && bookingAllocationResponse.Count > 0)
            {
                var currentBookingId = Guid.Empty;
                var currentAccommodationStartDate = (DateTime?)null;
                var currentAccommodationEndDate = (DateTime?)null;
                for (int i = 0; i < bookingAllocationResponse.Count; i++)
                {
                    if (bookingAllocationResponse[i] != null)
                    {
                        var bookingResponse = bookingAllocationResponse[i];

                        var previousBookingId = currentBookingId;
                        var previousAccommodationStartDate = currentAccommodationStartDate;
                        var previousAccommodationEndDate = currentAccommodationEndDate;

                        currentBookingId = bookingResponse.BookingId;
                        currentAccommodationStartDate = bookingResponse.AccommodationStartDate;
                        currentAccommodationEndDate = bookingResponse.AccommodationEndDate;

                        if (previousBookingId != currentBookingId)
                        {
                            if (bookingResponse.OwnerId.OwnerType == OwnerType.User)
                            {
                                if (currentAccommodationEndDate != DateTime.Now.Date)
                                {
                                    bookingAllocationResortTeamRequest.Add(PrepareResortTeamRequest(bookingResponse));
                                }
                            }
                        }

                        if (currentAccommodationStartDate == DateTime.Now.Date)
                        {
                            if (previousBookingId == currentBookingId && previousAccommodationStartDate != currentAccommodationStartDate)
                            {
                                bookingAllocationResortTeamRequest.Add(PrepareResortTeamRequest(bookingResponse));
                            }
                        }
                    }
                }
            }
            return bookingAllocationResortTeamRequest;
        }

        public BookingAllocationResortTeamRequest PrepareResortTeamRequest(BookingAllocationResponse bookingResponse)
        {
            BookingAllocationResortTeamRequest bookingTeamRequest = new BookingAllocationResortTeamRequest();
            bookingTeamRequest.BookingResortTeamRequest = new BookingResortTeamRequest { Id = bookingResponse.BookingId };
            bookingTeamRequest.CustomerResortTeamRequest = new CustomerResortTeamRequest { Id = bookingResponse.CustomerId.Id, Customer= bookingResponse.CustomerId };
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
