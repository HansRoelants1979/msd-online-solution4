using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.CustomWorkflowSteps.GetTeamDefaultQueue.Service;

namespace Tc.Crm.CustomWorkflowSteps.GetTeamDefaultQueue
{
   public class GetTeamDefaultQueueActivity : CodeActivity
    {

        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService trace = executionContext.GetExtension<ITracingService>();
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            var sourceMarketId = SourceMarket.Get<EntityReference>(executionContext);

            var teamDefaultQueueService = new GetTeamDefaultQueueService();

            trace.Trace("getting team default queue by Source Market.");
            if (sourceMarketId != null)
            {
                var response = teamDefaultQueueService.GetTeamDefaultQueue(sourceMarketId, service, trace);
                if (response != null)
                {
                    executionContext.SetValue<EntityReference>(Queue, response);
                }
                else
                    trace.Trace("response is null");

                return;
            }
        }

        [Input("SourceMarket")]
        [ReferenceTarget("tc_country")]
        public InArgument<EntityReference> SourceMarket { get; set; }

        [Output("Queue")]
        [ReferenceTarget("queue")]
        public OutArgument<EntityReference> Queue { get; set; }

    }
}
