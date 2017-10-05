using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace Tc.Crm.CustomWorkflowSteps
{
    public class MergeCustomerActivity : CodeActivity
    {
        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService trace = executionContext.GetExtension<ITracingService>();
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                //EntityReference entityMerge = EntityMerge.Get<EntityReference>(executionContext);
                //(new MergeCustomerService()).MergeCustomers(
                //        EntityMerge.Get<EntityReference>(executionContext),
                //        service,
                //        trace);
            }            
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(OperationStatus.Failed, ex.ToString());
            }
        }
       

        [RequiredArgument]
        [Input("EntityMerge")]        
        [ReferenceTarget("tc_entitymerge")]
        public InArgument<EntityReference> EntityMerge { get; set; }
    }
}
