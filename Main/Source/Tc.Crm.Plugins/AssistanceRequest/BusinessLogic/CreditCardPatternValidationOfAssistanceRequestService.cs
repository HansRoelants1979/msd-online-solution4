using Microsoft.Xrm.Sdk;
using Tc.Crm.Plugins.MultipleEntities.BusinessLogic;


namespace Tc.Crm.Plugins.AssistanceRequest.BusinessLogic
{
    public class CreditCardPatternValidationOfAssistanceRequestService : CreditCardPatternValidationService
    {
        public CreditCardPatternValidationOfAssistanceRequestService()
        {

        }

        public CreditCardPatternValidationOfAssistanceRequestService(ITracingService trace, IOrganizationService service) : base(trace, service)
        {

        }

    }
}
