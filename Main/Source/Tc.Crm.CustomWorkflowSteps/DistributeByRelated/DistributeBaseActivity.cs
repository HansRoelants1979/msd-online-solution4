using System;
using System.Activities;
using System.Collections.Generic;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace Tc.Crm.CustomWorkflowSteps.DistributeByRelated
{

    /// <summary>
    ///  DistributeBase step.
    ///  Description: Abstract class to structure distributing operations for entity records
    /// </summary> 
    public abstract class DistributeBaseActivity : CodeActivity
    {
        #region CodeActivity
        protected override void Execute(CodeActivityContext executionContext)
        {
            this.Distribute(executionContext);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Launch workflows for dependent entities.
        /// </summary>
        protected void Distribute(CodeActivityContext executionContext)
        {
            var workflowId = this.Workflow.Get(executionContext).Id;
            var keyList = this.GatherKeys(executionContext);
            IWorkflowContext workflowContext = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.UserId);
            
            foreach (Guid key in keyList)
            {
                ExecuteWorkflowRequest workflowRequest = new ExecuteWorkflowRequest();
                workflowRequest.EntityId = key;
                workflowRequest.WorkflowId = workflowId;
                service.Execute(workflowRequest);
            }
        }

        /// <summary>
        /// Get IDs of dependent or fetched entities
        /// </summary>

        protected abstract ICollection<Guid> GatherKeys(CodeActivityContext executionContext);

        #endregion

        #region Workflow Parameters
        [Input("Distributed Workflow")]
        [ReferenceTarget("workflow")]
        [RequiredArgument]
        public InArgument<EntityReference> Workflow { get; set; }

        #endregion
    }
}
