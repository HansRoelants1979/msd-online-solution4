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
            //logger.LogInformation("Executing GetBookingAllocations");
            IList<Guid> destinationGateways = GetDestinationGateways();
            if (destinationGateways == null || destinationGateways.Count == 0)
            {
                logger.LogWarning("No Gateways found to process");
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
                logger.LogInformation("Processing booking allocations :: " + bookingAllocationResponses.Count);
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
            //logger.LogInformation("ProcessAllocationResponse - start");
            if (bookingAllocationResponses == null || bookingAllocationResponses.Count == 0) return null;

            var bookingAllocationResortTeamRequest = new List<BookingAllocationResortTeamRequest>();

            var processedCustomers = new List<Guid>();
            var processedBookings = new List<BookingAllocationResponse>();           
            for (int i = 0; i < bookingAllocationResponses.Count; i++)
            {
                var bookingResponse = bookingAllocationResponses[i];
                WriteAllocationResponseLog(bookingResponse);

                if (!ValidForProcessing(bookingResponse, processedCustomers)) continue;

                if (bookingResponse.AccommodationStartDate.Value.Date < DateTime.Now.Date)
                {
                    logger.LogInformation("Not processing this record as accommodation start date is less than current date");
                    continue;
                }

                var differentBooking = processedBookings.Find(b => b.BookingId == bookingResponse.BookingId) == null;
                var sameBookingDifferentCustomer = processedBookings.Find(b => b.BookingId == bookingResponse.BookingId
                                                                            && b.AccommodationStartDate == bookingResponse.AccommodationStartDate
                                                                            && b.Customer.Id != bookingResponse.Customer.Id) != null;


                if (!differentBooking && !sameBookingDifferentCustomer)
                {
                    logger.LogInformation("Not processing this record as the booking was already processed");
                    continue;
                }

                
                if (sameBookingDifferentCustomer)
                {
                    //Add only Customer
                    bookingAllocationResortTeamRequest.Add(new BookingAllocationResortTeamRequest()
                    {
                        CustomerResortTeamRequest = PrepareCustomerResortTeamRequest(bookingResponse)
                    });
                    logger.LogInformation("<<Processing only customer record as the booking was already processed>>");                    
                }
                else if (differentBooking)
                {
                    //Add Booking, Customer
                    AddResortTeamRequest(bookingResponse, bookingAllocationResortTeamRequest);
                    logger.LogInformation("<<Processing both booking and customer records>>");                    
                }
                processedCustomers.Add(bookingResponse.Customer.Id);
                processedBookings.Add(bookingResponse);
            }
           
            return bookingAllocationResortTeamRequest;
        }

        private bool ValidForProcessing(BookingAllocationResponse bookingResponse, List<Guid> processedCustomers)
        {
            if (bookingResponse == null) return false;
            if (bookingResponse.Customer == null)
            {
                logger.LogWarning("Not processing this record as no customer exists");
                return false;
            }
            if (bookingResponse.BookingOwner.OwnerType == OwnerType.Team)
            {
                logger.LogInformation("Not processing this record as the booking owner type is team");
                return false;
            }
            if (bookingResponse.Customer.Owner == null) return false;
            if (bookingResponse.Customer.Owner.OwnerType == OwnerType.Team)
            {
                logger.LogInformation("Not processing this record as the customer owner type is team");
                return false;
            }

            //customer already allocated
            if (processedCustomers.Contains(bookingResponse.Customer.Id))
            {
                logger.LogWarning("Not processing this record as customer: " + bookingResponse.Customer.Name + " was already got processed.");
                return false;
            }
            //accommodation start date is not provided
            if (bookingResponse.AccommodationStartDate == null)
            {
                logger.LogWarning("Not processing this record as accommodation startdate is null");
                return false;
            }
            return true;
        }

        public void WriteAllocationResponseLog(BookingAllocationResponse bookingAllocationResponse)
        {
            if (bookingAllocationResponse != null)
            {
                StringBuilder information = new StringBuilder();
                information.AppendLine();
                if (bookingAllocationResponse.BookingNumber != null)
                    information.AppendLine("Processing Booking: " + bookingAllocationResponse.BookingNumber);
                if (bookingAllocationResponse.BookingOwner != null)
                    information.AppendLine("Booking Owner: " + bookingAllocationResponse.BookingOwner.Name + " of Type " + bookingAllocationResponse.BookingOwner.OwnerType.ToString());
                if (bookingAllocationResponse.AccommodationStartDate != null)
                    information.AppendLine("Accommodation Start Date: " + bookingAllocationResponse.AccommodationStartDate.Value.ToString());
                if (bookingAllocationResponse.Customer != null)
                {
                    information.AppendLine("Booking Customer: " + bookingAllocationResponse.Customer.Name + " of type " + bookingAllocationResponse.Customer.CustomerType.ToString());
                    information.AppendLine("Customer Owner: " + bookingAllocationResponse.Customer.Owner.Name + " of type " + bookingAllocationResponse.Customer.Owner.OwnerType.ToString());
                }                
                if (bookingAllocationResponse.HotelOwner != null)
                    information.AppendLine("Hotel Owner: " + bookingAllocationResponse.HotelOwner.Name + " of Type " + bookingAllocationResponse.HotelOwner.OwnerType.ToString());

                logger.LogInformation(information.ToString());
            }

        }

        public void AddResortTeamRequest(BookingAllocationResponse bookingResponse, IList<BookingAllocationResortTeamRequest> bookingAllocationResortTeamRequests)
        {
            //logger.LogInformation("AddResortTeamRequest - start");
            if (bookingAllocationResortTeamRequests == null)
                throw new ArgumentNullException("bookingAllocationResortTeamRequests");
            if (bookingResponse == null)
                throw new ArgumentNullException("bookingResponse");

            var bookingTeamRequest = PrepareResortTeamRequest(bookingResponse);
            bookingAllocationResortTeamRequests.Add(bookingTeamRequest);

            //logger.LogInformation("AddResortTeamRequest - end");
        }

        public BookingAllocationResortTeamRequest PrepareResortTeamRequest(BookingAllocationResponse bookingResponse)
        {
            //logger.LogInformation("PrepareResortTeamRequest - start");
            var bookingTeamRequest = new BookingAllocationResortTeamRequest()
            {
                BookingResortTeamRequest = PrepareBookingResortTeamRequest(bookingResponse),
                CustomerResortTeamRequest = PrepareCustomerResortTeamRequest(bookingResponse)
            };
            //logger.LogInformation("PrepareResortTeamRequest - end");
            return bookingTeamRequest;
        }

        public CustomerResortTeamRequest PrepareCustomerResortTeamRequest(BookingAllocationResponse bookingResponse)
        {
            //logger.LogInformation("PrepareCustomerResortTeamRequest - start");
            if (bookingResponse == null) throw new ArgumentNullException("bookingResponse");
            if (bookingResponse.Customer == null || bookingResponse.Customer.Id == Guid.Empty) return null;
            if (bookingResponse.HotelOwner == null) return null;
            var customerResortTeamRequest = new CustomerResortTeamRequest
            {
                Customer = bookingResponse.Customer,
                Owner = bookingResponse.HotelOwner
            };

            //logger.LogInformation("PrepareCustomerResortTeamRequest - end");
            return customerResortTeamRequest;
        }

        public BookingResortTeamRequest PrepareBookingResortTeamRequest(BookingAllocationResponse bookingResponse)
        {
            //logger.LogInformation("PrepareBookingResortTeamRequest - start");
            if (bookingResponse == null) throw new ArgumentNullException("bookingResponse");

            if (bookingResponse.BookingId == null || bookingResponse.BookingId == Guid.Empty) return null;
            if (bookingResponse.HotelOwner == null) return null;

            var bookingResortTeamRequest = new BookingResortTeamRequest
            {
                Id = bookingResponse.BookingId,
                Name = bookingResponse.BookingNumber,
                Owner = bookingResponse.HotelOwner
            };

            //logger.LogInformation("PrepareBookingResortTeamRequest - end");
            return bookingResortTeamRequest;
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
