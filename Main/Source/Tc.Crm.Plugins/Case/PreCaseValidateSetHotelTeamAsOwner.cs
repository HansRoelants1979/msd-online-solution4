using System;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;
using Tc.Crm.Plugins.Case.BusinessLogic;

namespace Tc.Crm.Plugins.Case
{
    public class PreCaseValidateSetHotelTeamAsOwner : IPlugin
    {
        /// <summary>
        /// Description: On case pre create set child hotel team as owner for case and customer record associated with case
        /// Message: Create
        /// Primary Entity: incident
        /// Run in user's context: Calling User
        /// Pipeline Stage: Pre-validation
        /// Execution Mode: Synchronous
        /// Deployment: Server
        /// </summary>
        /// <param name="serviceProvider"></param>
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);
            try
            {
                trace.Trace("Begin - PreCaseValidateSetHotelTeamAsOwner");
                AssignHotelTeamAsOwner ownerService = new AssignHotelTeamAsOwner(context, trace, service);
                ownerService.DoActions();
                trace.Trace("End - PreCaseValidateSetHotelTeamAsOwner");
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new InvalidPluginExecutionException(ex.ToString());
            }
            catch (TimeoutException ex)
            {
                throw new InvalidPluginExecutionException(ex.ToString());
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.ToString());
            }
        }
    }
}
