using System;
using Microsoft.Xrm.Sdk;
using Tc.Crm.Plugins.MultipleEntities;
using Tc.Crm.Plugins.MultipleEntities.BusinessLogic;
using Tc.Crm.Plugins.Task.BusinessLogic;

namespace Tc.Crm.Plugins.Task
{
    public class CreditCardPatternValidationOfTask : CreditCardPatternValidation
    {

        protected override string EntityName
        {
            get
            {
                return Entities.Task;
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
            return new CreditCardPatternValidationOfTaskService(trace, service);
        }
    }
}
