using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Microsoft.Xrm.Sdk;

namespace Tc.Crm.CustomWorkflowSteps
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

        public const string Seperator = ",";

        public const string NextLine = "\r\n";

        public const string Account = "B";

        public const string Contact = "P";

        public const string Booked = "B";

        public const string Cancelled = "C";      

        public bool DeleteBookingRole { get; set; }

        public bool DeleteAccomodationOrTransportOrRemarks { get; set; }

        public string BookingId { get; set; }

        public string CustomerId { get; set; }
    }

}



