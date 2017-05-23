using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;
using Tc.Crm.Plugins.Email.BusinessLogic;

namespace Tc.Crm.Plugins.Email
{
    public class PreoperationUpdateEmailBody : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);

            try
            {
                trace.Trace("Begin - SendEmail");
                UpdateEmailBodyService sendEmailService = new UpdateEmailBodyService(context, trace, service);
                sendEmailService.UpdateEmailBodyWithHeadersandFooters();
                trace.Trace("End - SendEmail");

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
