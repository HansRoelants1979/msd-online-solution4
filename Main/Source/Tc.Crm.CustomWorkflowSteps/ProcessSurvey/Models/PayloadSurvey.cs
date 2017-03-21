using Microsoft.Xrm.Sdk;

namespace Tc.Crm.CustomWorkflowSteps.ProcessSurvey.Models
{
    public class PayloadSurvey
    {
        public PayloadSurvey(ITracingService trace, IOrganizationService crmService)
        {
            this.Trace = trace;
            this.CrmService = crmService;
        }

        public ITracingService Trace { get; set; }
        public IOrganizationService CrmService { get; set; }
        public Survey SurveyResponse { get; set; }
    }
}
