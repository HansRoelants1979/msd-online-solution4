using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.CustomWorkflowSteps.RetrieveConfigurationEntity.Service;


namespace Tc.Crm.CustomWorkflowSteps.RetrieveConfigurationEntity
{
    public class RetrieveConfigurationEntityActivity : CodeActivity
    {
        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService trace = executionContext.GetExtension<ITracingService>();
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                var Name = string.Empty;
                Name = ConfigurationEntityName.Get<string>(executionContext);
                var configurationEntityValue = RetrieveConfigurationEntityValue.RetrieveValue(Name, trace, service);
                if (configurationEntityValue != null)
                {
                    ConfigurationEntityValue.Set(executionContext, configurationEntityValue);
                }
                else
                {
                    trace.Trace("Configuration Entity Value is Null");
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

        [RequiredArgument]
        [Input("ConfigurationEntityName")]
        public InArgument<string> ConfigurationEntityName { get; set; }

        [Output("ConfigurationEntityValue")]
        public OutArgument<string> ConfigurationEntityValue { get; set; }
    }
}
