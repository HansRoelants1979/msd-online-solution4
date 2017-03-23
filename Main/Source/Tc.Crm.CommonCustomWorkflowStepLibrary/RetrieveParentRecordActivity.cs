using System;
using System.Activities;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System.ServiceModel;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using System.Collections.Generic;

namespace Tc.Crm.CommonCustomWorkflowStepLibrary
{
    public class RetrieveParentRecordActivity : CodeActivity
    {

        protected override void Execute(CodeActivityContext executionContext)
        {
            //Create the tracing service
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            //Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            try
            {
                //var sourceEntity = SourceEntity.Get<EntityReference>(executionContext);
                //var targetEntity = TargetEntity.Get<string>(executionContext);
                //var relationshipName = RelationshipName.Get<string>(executionContext);
                //ParentRecord.Set(executionContext, RetrieveParentRecord(sourceEntity, targetEntity, relationshipName, service));
                
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

        
        //starting entity should be the entity on which the workflow runs
        //case||customerid;contact||parentaccountid;account||companyname
        [Input("Expression")]
        public string Expression { get; set; }

        //entity reference - string
        //optionset - string
        [Output("ReturnValue")]
        public string ReturnValue { get; set; }





    }
}
