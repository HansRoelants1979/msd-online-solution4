using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Tc.Crm.Plugins.MultipleEntities.BusinessLogic;

namespace Tc.Crm.Plugins.PhoneCall.BusinessLogic
{
    public class CreditCardPatternValidationOfPhoneCallService : CreditCardPatternValidationService
    {

        public CreditCardPatternValidationOfPhoneCallService()
        {

        }

        public CreditCardPatternValidationOfPhoneCallService(ITracingService trace, IOrganizationService service):base(trace,service)
        {

        }
       
    }
}
