using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.CustomWorkflowSteps.Utility
{
    public class SimpleValueFetchActivity:CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            ITracingService trace = context.GetExtension<ITracingService>();
            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.UserId);

            var fetchXml = FetchXml.Get<string>(context);
            var returnType = FetchXml.Get<string>(context);
            var returnAttribute = FetchXml.Get<string>(context);

            if (string.IsNullOrWhiteSpace(fetchXml))
            {
                trace.Trace("fetch xml is null");
                return;
            }
            if (string.IsNullOrWhiteSpace(returnType))
            {
                trace.Trace("return type is null");
                return;
            }
            if (string.IsNullOrWhiteSpace(returnAttribute))
            {
                trace.Trace("return attribute is null.");
                return;
            }
            var query = new FetchExpression(fetchXml);
            var response = service.RetrieveMultiple(query);

            if (response == null || response.Entities == null || response.Entities.Count == 0)
            {
                trace.Trace("response is null");
                return;
            }

            if (!response.Entities[0].Contains(returnAttribute))
            {
                trace.Trace("response doesnt have the requested attribute.");
                return;
            }

            if (response.Entities[0][returnAttribute] == null)
            {
                trace.Trace("returned object is null");
                return;
            }

            if (returnType.Equals("bool", StringComparison.OrdinalIgnoreCase))
            {
                trace.Trace("setting boolean value.");
                ReturnValueBool.Set(context, (bool)response.Entities[0][returnAttribute]);
            }
            if (returnType.Equals("int", StringComparison.OrdinalIgnoreCase))
            {
                trace.Trace("setting integer value.");
                ReturnValueInt.Set(context, (int)response.Entities[0][returnAttribute]);
            }
            if (returnType.Equals("string", StringComparison.OrdinalIgnoreCase))
            {
                trace.Trace("setting string value.");
                ReturnValueString.Set(context, response.Entities[0][returnAttribute].ToString());
            }
        }

        [Input("Fetch Xml")]
        public InArgument<string> FetchXml { get; set; }

        [Input("Return Type")]
        public InArgument<string> ReturnType { get; set; }

        [Input("Return Attribute")]
        public InArgument<string> ReturnAttribute { get; set; }

        [Output("Return Value (Bool)")]
        [ReferenceTarget("queue")]
        public OutArgument<bool> ReturnValueBool { get; set; }

        [Output("Return Value (Int)")]
        [ReferenceTarget("queue")]
        public OutArgument<bool> ReturnValueInt { get; set; }

        [Output("Return Value (string)")]
        [ReferenceTarget("queue")]
        public OutArgument<bool> ReturnValueString { get; set; }
    }

    
}
