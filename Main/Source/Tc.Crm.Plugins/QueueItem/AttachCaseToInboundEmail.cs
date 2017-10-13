using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;
using Tc.Crm.Plugins.QueueItem.BusinessLogic;

namespace Tc.Crm.Plugins.QueueItem
{
    public class AttachCaseToInboundEmail : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);

            try
            {
                trace.Trace("Begin - AttachCaseToInboundEmail");
                AttachCaseToInboundEmailService attachCaseToInboundEmailService = new AttachCaseToInboundEmailService(context, trace, service);
                attachCaseToInboundEmailService.AttachCaseToInboundEmail();
                trace.Trace("End - AttachCaseToInboundEmail");

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

