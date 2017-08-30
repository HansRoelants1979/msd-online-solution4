using System;
using System.Activities;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Tc.Crm.CustomWorkflowSteps.DistributeByRelated.Service;

namespace Tc.Crm.CustomWorkflowSteps.DistributeByRelated
{
    /// <summary>
    ///  DistributeOneToMany step.
    ///  Description: Distributes operation for one to many relationship
    /// </summary> 
    public sealed class DistributeOneToManyActivity : DistributeBaseActivity
    {
        #region Protected Methods
        /// <summary>
        /// Gather guid keys of one to many related records
        /// </summary>
        /// <param name="executionContext"></param>
        /// <returns>ICollection Guid keys values</returns>
        protected override ICollection<Guid> GatherKeys(CodeActivityContext executionContext)
        {

            IWorkflowContext workflowContext = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.UserId);
            var relationship = DistributeByRelatedService.GetOneToManyRelationship(service, this.RelationshipName.Get(executionContext));
            var keyList = DistributeByRelatedService.GatherOneToManyKeys(service, relationship, workflowContext.PrimaryEntityId);
            return keyList;
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
