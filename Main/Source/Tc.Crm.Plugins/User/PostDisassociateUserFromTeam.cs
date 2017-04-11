using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Tc.Crm.Plugins.User.BusinessLogic;

namespace Tc.Crm.Plugins.User
{
    public class PostDisassociateUserFromTeam : IPlugin
    {        
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);
            try
            {
                trace.Trace("Begin - AssociateUserToTeamService");
                DeassociateUserFromTeamService deassociateUserFromTeamService = new DeassociateUserFromTeamService(context, trace, service);
                deassociateUserFromTeamService.DoActionsOnUserDisassociate();
                trace.Trace("End - AssociateUserToTeamService");

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
