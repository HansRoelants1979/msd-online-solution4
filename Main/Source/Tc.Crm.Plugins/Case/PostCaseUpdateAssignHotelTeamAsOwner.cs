using System;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;
using Tc.Crm.Plugins.Case.BusinessLogic;

namespace Tc.Crm.Plugins.Case
{
    public class PostCaseUpdateAssignHotelTeamAsOwner : IPlugin
    {
        /// <summary>
        /// Description: On case post update, update child hotel team as owner for customer record associated with case
        /// Message: Update
        /// Primary Entity: incident
        /// Filtering Attributes: customerid
        /// Run in user's context: Calling User
        /// Pipeline Stage: Post-operation
        /// Execution Mode: Synchronous
        /// Deployment: Server
        /// Pre Image: CasePreImage (tc_bookingid)
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
                trace.Trace("Begin - PostCaseUpdateAssignHotelTeamAsOwner");
                AssignHotelTeamAsOwner ownerService = new AssignHotelTeamAsOwner(context, trace, service);
                ownerService.DoActions();
                trace.Trace("End - PostCaseUpdateAssignHotelTeamAsOwner");
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
