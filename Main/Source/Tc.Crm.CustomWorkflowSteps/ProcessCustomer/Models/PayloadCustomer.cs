using Microsoft.Xrm.Sdk;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;

namespace Tc.Crm.CustomWorkflowSteps.ProcessCustomer.Models
{
    public class PayloadCustomer
    {
        public PayloadCustomer(ITracingService trace, IOrganizationService crmService)
        {
            this.Trace = trace;
            this.CrmService = crmService;
        }

        public Customer Customer  { get; set; }        
        public IOrganizationService CrmService { get; set; }
        public ITracingService Trace { get; set; }
        public string CustomerId { get; set; }
        public XrmUpdateResponse Response { get; set; }
        public string OperationType { get; set; }

    }
}
