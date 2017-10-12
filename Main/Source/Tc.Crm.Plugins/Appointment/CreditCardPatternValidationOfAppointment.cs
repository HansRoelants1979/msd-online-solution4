using Tc.Crm.Plugins.MultipleEntities;
using Tc.Crm.Plugins.Appointment.BusinessLogic;
using Microsoft.Xrm.Sdk;
using System;
using Tc.Crm.Plugins.MultipleEntities.BusinessLogic;

namespace Tc.Crm.Plugins.Appointment
{
    public class CreditCardPatternValidationOfAppointment : CreditCardPatternValidation
    {       
        protected override string EntityName
        {
            get
            {
                return Entities.Appointment;
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
            return new CreditCardPatternValidationOfAppointmentService(trace, service);
        }
    }
}
