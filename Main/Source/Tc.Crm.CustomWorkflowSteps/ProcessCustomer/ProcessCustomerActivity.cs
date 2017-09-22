using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.ServiceModel;
using Tc.Crm.CustomWorkflowSteps.ProcessCustomer.Models;
using Tc.Crm.CustomWorkflowSteps.ProcessCustomer.Services;

namespace Tc.Crm.CustomWorkflowSteps.ProcessCustomer
{
    public class ProcessCustomerActivity : CodeActivity
    {
        [Input("Customer Info")]
        [RequiredArgument]
        public InArgument<string> CustomerInfo { get; set; }

        [Input("Operation Type")]
        [RequiredArgument]
        public InArgument<string> OperationType { get; set; }

        [Output("Response")]
        [RequiredArgument]
        public OutArgument<string> Response { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {                
                var customerJson = CustomerInfo.Get<string>(executionContext);                
                var operationType = OperationType.Get<string>(executionContext);                
                PayloadCustomer payloadCustomer = new PayloadCustomer(tracingService, service);
                payloadCustomer.Customer = JsonHelper.DeserializeCustomerJson(customerJson, tracingService);
                payloadCustomer.OperationType = operationType;                
                ProcessCustomerService process = new ProcessCustomerService(payloadCustomer);                
                Response.Set(executionContext, process.processPayload());                
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                tracingService.Trace(ex.ToString());
                throw new InvalidPluginExecutionException(ex.ToString());
            }
            catch (InvalidPluginExecutionException ex)
            {
                tracingService.Trace(ex.ToString());
                throw new InvalidPluginExecutionException(ex.ToString());
            }
            catch (TimeoutException ex)
            {
                tracingService.Trace(ex.ToString());
                throw new InvalidPluginExecutionException(ex.ToString());
            }
            catch (Exception ex)
            {
                tracingService.Trace(ex.ToString());
                throw new InvalidPluginExecutionException(ex.ToString());
            }
        }       
    }
}

