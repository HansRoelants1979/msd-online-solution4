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
       

        public void GetBookingAllocations()
        {
            logger.LogInformation("Executing GetBookingAllocations");
            IList<Guid> destinationGateways = GetDestinationGateways();
            if (destinationGateways != null && destinationGateways.Count > 0)
            {
                logger.LogInformation("Processing for " + destinationGateways.Count.ToString() + " Destination Gateways");
                if (configurationService.DepartureDateinNextXDays != null)
                {
                    IList<BookingAllocationResponse> bookingAllocationResponse = allocationService.GetBookingAllocations(new
                                                                                    BookingAllocationRequest
                    {
                        DepartureDateinNextXDays = int.Parse(configurationService.DepartureDateinNextXDays),
                        DepartureDate = DateTime.Now.Date,
                        ReturnDate = DateTime.Now.Date,
                        Destination = destinationGateways
                    });

                    IList<BookingAllocationResortTeamRequest> bookingAllocationResortTeamRequest = ProcessAllocationResponse(bookingAllocationResponse);
                    allocationService.ProcessBookingAllocations(bookingAllocationResortTeamRequest);
                }
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

        public IList<BookingAllocationResortTeamRequest> ProcessAllocationResponse(IList<BookingAllocationResponse> bookingAllocationResponse)
        {
            IList<BookingAllocationResortTeamRequest> bookingAllocationResortTeamRequest = null;
            if (bookingAllocationResponse != null && bookingAllocationResponse.Count > 0)
            {
                bookingAllocationResortTeamRequest = new List<BookingAllocationResortTeamRequest>();
                var currentBookingId = Guid.Empty;
                var currentAccommodationStartDate = (DateTime?)null;
                var currentAccommodationEndDate = (DateTime?)null;
                for (int i = 0; i < bookingAllocationResponse.Count; i++)
                {
                    if (bookingAllocationResponse[i] != null)
                    {
                        var bookingResponse = bookingAllocationResponse[i];
                        if (bookingResponse != null)
                        {
                            var previousBookingId = currentBookingId;
                            var previousAccommodationStartDate = currentAccommodationStartDate;
                            var previousAccommodationEndDate = currentAccommodationEndDate;

                            currentBookingId = bookingResponse.BookingId;
                            currentAccommodationStartDate = bookingResponse.AccommodationStartDate;
                            currentAccommodationEndDate = bookingResponse.AccommodationEndDate;

                            if (previousBookingId != currentBookingId)
                            {
                                if (bookingResponse.BookingOwner.OwnerType == OwnerType.User)
                                {
                                    if (currentAccommodationEndDate != DateTime.Now.Date)
                                    {
                                        AddResortTeamRequest(bookingResponse, bookingAllocationResortTeamRequest);
                                    }
                                }
                            }

                            if (currentAccommodationStartDate == DateTime.Now.Date)
                            {
                                if (previousBookingId == currentBookingId && previousAccommodationStartDate != currentAccommodationStartDate)
                                {
                                    AddResortTeamRequest(bookingResponse, bookingAllocationResortTeamRequest);
                                }
                            }
                        }
                    }
                }
            }
            return bookingAllocationResortTeamRequest;
        }

        public void AddResortTeamRequest(BookingAllocationResponse bookingResponse, IList<BookingAllocationResortTeamRequest> bookingAllocationResortTeamRequest)
        {
            BookingAllocationResortTeamRequest bookingTeamRequest = PrepareResortTeamRequest(bookingResponse);
            if (bookingTeamRequest != null && bookingAllocationResortTeamRequest != null)
                bookingAllocationResortTeamRequest.Add(bookingTeamRequest);
        }

        public BookingAllocationResortTeamRequest PrepareResortTeamRequest(BookingAllocationResponse bookingResponse)
        {
            BookingAllocationResortTeamRequest bookingTeamRequest = null;
            if (bookingResponse != null)
            {
                bookingTeamRequest = new BookingAllocationResortTeamRequest();
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
            }
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
