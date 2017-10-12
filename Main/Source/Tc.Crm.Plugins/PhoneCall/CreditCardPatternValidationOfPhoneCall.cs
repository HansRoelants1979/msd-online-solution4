using System;
using Microsoft.Xrm.Sdk;
using Tc.Crm.Plugins.MultipleEntities;
using Tc.Crm.Plugins.MultipleEntities.BusinessLogic;
using Tc.Crm.Plugins.PhoneCall.BusinessLogic;

namespace Tc.Crm.Plugins.PhoneCall
{
    public class CreditCardPatternValidationOfPhoneCall : CreditCardPatternValidation
    {

        protected override string EntityName
        {
            get
            {
                return Entities.Phonecall;
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
            return new CreditCardPatternValidationOfPhoneCallService(trace, service);
        }

       
    }
}
