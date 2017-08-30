using System;
using System.Activities;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Tc.Crm.CustomWorkflowSteps.DistributeByRelated.Service;

namespace Tc.Crm.CustomWorkflowSteps.DistributeByRelated
{
    /// <summary>
    ///  DistributeManyToMany step.
    ///  Description: Distributes operation for many to many relationship
    /// </summary> 
    public sealed class DistributeManyToManyActivity : DistributeBaseActivity
    {
        #region Protected Methods

        /// <summary>
        /// Gather guid keys of many to many related records
        /// </summary>
        /// <param name="executionContext"></param>
        /// <returns>Collection of Guid keys</returns>
        protected override ICollection<Guid> GatherKeys(CodeActivityContext executionContext)
        {
            IWorkflowContext workflowContext = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.UserId);
            var relationship = DistributeByRelatedService.GetManyToManyRelationship(service, this.RelationshipName.Get(executionContext));
            var keys = DistributeByRelatedService.GatherManyToManyKeys(service, relationship, workflowContext.PrimaryEntityId, workflowContext.PrimaryEntityName);
            return keys;
        }

        #endregion

        /// <summary>
        /// Input parameter: Name of the relationship
        /// </summary>
        [Input("Relationship Name")]
        [RequiredArgument]
        public InArgument<string> RelationshipName { get; set; }

    }
}
