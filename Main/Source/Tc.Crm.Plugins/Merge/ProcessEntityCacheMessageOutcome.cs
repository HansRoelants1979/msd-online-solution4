using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Tc.Crm.Plugins.Merge.BusinessLogic;

namespace Tc.Crm.Plugins.Merge
{
    public class ProcessEntityCacheMessageOutcome : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var trace = (ITracingService) serviceProvider.GetService(typeof(ITracingService));
            var context = (IPluginExecutionContext) serviceProvider.GetService(typeof(IPluginExecutionContext));
            var factory = (IOrganizationServiceFactory) serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var orgService = factory.CreateOrganizationService(context.UserId);

            try
            {
                trace.Trace("Begin - UpdateResultingCustomerRecord");
                if (!IsContextValid(context))
                {
                    trace.Trace("Context is invalid");
                    return;
                }

                var service = new EntityCacheMessageOutcomeService(trace, orgService, new EntityMergeFactory());
                service.Run(context);
                trace.Trace("End - UpdateResultingCustomerRecord");
            }
            catch (FaultException<OrganizationServiceFault> exception)
            {
                trace.Trace(exception.ToString());
                throw new InvalidPluginExecutionException(exception.ToString());
            }
            catch (TimeoutException exception)
            {
                trace.Trace(exception.ToString());
                throw new InvalidPluginExecutionException(exception.ToString());
            }
            catch (Exception exception)
            {
                trace.Trace(exception.ToString());
                throw new InvalidPluginExecutionException(exception.ToString());
            }
        }

        private static bool IsContextValid(IPluginExecutionContext context)
        {
            return context.MessageName.Equals(Messages.Update, StringComparison.OrdinalIgnoreCase) ||
                   context.Stage == (int) PluginStage.Postoperation ||
                   context.InputParameters.Contains(InputParameters.Target) ||
                   context.InputParameters[InputParameters.Target] is Entity ||
                   context.Mode == (int) PluginMode.Asynchronous;
        }
    }
}