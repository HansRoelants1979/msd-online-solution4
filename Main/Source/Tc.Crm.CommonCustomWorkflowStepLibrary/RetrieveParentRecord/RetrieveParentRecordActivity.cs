using System;
using System.Activities;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System.ServiceModel;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using System.Collections.Generic;
using Tc.Crm.CommonCustomWorkflowStepLibrary.RetrieveParentRecord.Services;

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

                
                var expression = Expression.Get<string>(executionContext);
                ReturnValue.Set(executionContext, RetrieveRecordProcessHelper.RetrieveParentRecord(expression, service,context));

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

        [Input("String Expression")]
        public InArgument<string> Expression { get; set; }

        //entity reference - string
        //optionset - string

        [Output("String ReturnValue")]
        public OutArgument<string> ReturnValue { get; set; }




    }
}
