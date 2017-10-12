using Microsoft.Xrm.Sdk;
using Tc.Crm.Plugins.MultipleEntities.BusinessLogic;

namespace Tc.Crm.Plugins.Appointment.BusinessLogic
{
    public class CreditCardPatternValidationOfAppointmentService : CreditCardPatternValidationService
    {
        public CreditCardPatternValidationOfAppointmentService()
        {

        }

        public CreditCardPatternValidationOfAppointmentService(ITracingService trace, IOrganizationService service) : base(trace, service)
        {

        }

    }
}
