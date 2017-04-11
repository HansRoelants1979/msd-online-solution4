using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.CustomWorkflowSteps.QueueIdentifier.Service;

namespace Tc.Crm.CustomWorkflowSteps.QueueIdentifier
{
    public class QueueIdentifierActivity : CodeActivity
    {
        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService trace = executionContext.GetExtension<ITracingService>();
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            var queueName = QueueName.Get<string>(executionContext);
            var caseId = Case.Get<EntityReference>(executionContext);

            var queueIdentifierService = new QueueIdentifierService();
            if (!string.IsNullOrWhiteSpace(queueName))
            {
                trace.Trace("getting queue by queue name.");
                var response = queueIdentifierService.GetQueueBy(queueName, service, trace);
                if (response != null)
                    executionContext.SetValue<EntityReference>(Queue, response);
                return;
            }

            trace.Trace("getting queue by case id.");
            if (caseId != null)
            {
                var response = queueIdentifierService.GetQueueFor(caseId, service, trace);
                if (response != null)
                    executionContext.SetValue<EntityReference>(Queue, response);
                return;
            }

            trace.Trace("queueName and caseId inputs - both are null;");
        }
        [Input("Queue Name")]
        public InArgument<string> QueueName { get; set; }

        [Input("Case")]
        [ReferenceTarget("incident")]
        public InArgument<EntityReference> Case { get; set; }

        [Output("Queue")]
        [ReferenceTarget("queue")]
        public OutArgument<EntityReference> Queue { get; set; }
    }
}
