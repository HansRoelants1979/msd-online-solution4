using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;

namespace Tc.Crm.Plugins
{
    public class RemoveUserFromHotelTeam : IPlugin
    {        
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext localContext = (IPluginExecutionContext)
               serviceProvider.GetService(typeof(IPluginExecutionContext));

            ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(localContext.UserId);
            try
            {
                if (localContext.Depth > 1)
                    return;

                if (localContext.MessageName == Messages.Disassociate)
                {
                    tracingService.Trace("Begin - PluginExecution");
                    string relationshipName = string.Empty;
                    if (localContext.InputParameters.Contains(InputParameters.Relationship))
                    {
                        relationshipName = localContext.InputParameters[InputParameters.Relationship].ToString();
                        tracingService.Trace("Relationship " + relationshipName);
                    }

                    if (relationshipName != Relationships.TeamMembershipAssociation + ".")
                        return;

                    ProcessRemoveUserFromHotelTeam processRemoveUser = new ProcessRemoveUserFromHotelTeam(service, localContext, tracingService);
                    processRemoveUser.ProcessDisassociateUserFromHotelTeam();
                   
                    tracingService.Trace("End - PluginExecution");
                }

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
