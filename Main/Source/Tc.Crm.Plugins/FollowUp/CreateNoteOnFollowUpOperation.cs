using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;
using Tc.Crm.Plugins.FollowUp.BusinessLogic;

namespace Tc.Crm.Plugins.FollowUp
{
    public class CreateNoteOnFollowUpOperation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);

            try
            {
                trace.Trace("Begin - CreateNote");
                CreateNoteOnFollowUpOperationService followUpOperationService = new CreateNoteOnFollowUpOperationService(context, trace, service);
                followUpOperationService.PrePareNoteFromFolloWup();
                trace.Trace("End - CreateNote");

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
