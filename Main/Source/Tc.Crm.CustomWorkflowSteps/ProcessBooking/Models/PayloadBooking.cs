using Microsoft.Xrm.Sdk;

namespace Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models
{
    public class PayloadBooking
    {
        public PayloadBooking(ITracingService trace,IOrganizationService crmService)
        {
            this.Trace = trace;
            this.CrmService = crmService;
        }
        public Booking BookingInfo { get; set; }
        public IOrganizationService CrmService { get; set; }
        public ITracingService Trace { get; set; }
        public BookingResponse Response { get; set; }
        

        public bool DeleteBookingRole { get; set; }

        public bool DeleteAccommodationOrTransportOrRemarks { get; set; }

        public string BookingId { get; set; }

        public string CustomerId { get; set; }
    }

}



