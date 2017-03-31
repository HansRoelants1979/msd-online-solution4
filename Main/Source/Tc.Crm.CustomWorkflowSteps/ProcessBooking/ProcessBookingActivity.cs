using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System.ServiceModel;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Services;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;

namespace Tc.Crm.CustomWorkflowSteps.ProcessBooking
{
    public class ProcessBookingActivity : CodeActivity
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
                var bookingJson = BookingInfo.Get<string>(executionContext);
                PayloadBooking payloadBooking = new PayloadBooking(tracingService, service);
                payloadBooking.BookingInfo = JsonHelper.DeSerializeJson(bookingJson,tracingService);
                ProcessBookingService process = new ProcessBookingService(payloadBooking);
                Response.Set(executionContext, process.ProcessPayload());

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


        [Input("String BookingInfo")]        
        public InArgument<string> BookingInfo { get; set; }

        [Output("String Response")]
        public OutArgument<string> Response { get; set; }
    }

   

}
