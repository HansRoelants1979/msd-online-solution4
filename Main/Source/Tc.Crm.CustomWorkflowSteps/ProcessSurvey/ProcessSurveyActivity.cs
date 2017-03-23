using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System.ServiceModel;
using Tc.Crm.CustomWorkflowSteps.ProcessSurvey.Models;
using Tc.Crm.CustomWorkflowSteps.ProcessSurvey.Services;

namespace Tc.Crm.CustomWorkflowSteps.ProcessSurvey
{
    public class ProcessSurveyActivity : CodeActivity
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
                var surveyJson = SurveyResponseInfo.Get<string>(executionContext);
                PayloadSurvey payloadSurvey = new PayloadSurvey(tracingService, service);
                payloadSurvey.SurveyResponse = JsonHelper.DeSerializeSurveyJson(surveyJson, tracingService);
                ProcessSurveyService processSurveyService = new ProcessSurveyService(payloadSurvey);
                Response.Set(executionContext, processSurveyService.ProcessSurveyResponse());

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


        [Input("String SurveyResponseInfo")]
        public InArgument<string> SurveyResponseInfo { get; set; }

        [Output("String Response")]
        public OutArgument<string> Response { get; set; }
    }
}
