using Tc.Crm.Plugins.MultipleEntities;
using Tc.Crm.Plugins.AssistanceRequest.BusinessLogic;
using Microsoft.Xrm.Sdk;
using System;
using Tc.Crm.Plugins.MultipleEntities.BusinessLogic;

namespace Tc.Crm.Plugins.AssistanceRequest
{
    public class CreditCardPatternValidationOfAssistanceRequest : CreditCardPatternValidation
    {
       
        protected override string EntityName
        {
            get
            {
                return Entities.AssistanceRequest;
            }
        }

        protected override string PluginName
        {
            get
            {
                return this.GetType().Name;
            }
        }

        protected override CreditCardPatternValidationService GetBusinessLogic(ITracingService trace, IOrganizationService service)
        {
            return new CreditCardPatternValidationOfAssistanceRequestService(trace, service);
        }
    }
}
