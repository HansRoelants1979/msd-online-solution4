using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Tc.Crm.Plugins.MultipleEntities.BusinessLogic;

namespace Tc.Crm.Plugins.Task.BusinessLogic
{
    public class CreditCardPatternValidationOfTaskService : CreditCardPatternValidationService
    {

        public CreditCardPatternValidationOfTaskService()
        {

        }

        public CreditCardPatternValidationOfTaskService(ITracingService trace, IOrganizationService service) : base(trace, service)
        {

        }
       
    }
}
