using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;


namespace Tc.Crm.CustomWorkflowSteps
{
    public class ProcessBooking : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            
            throw new NotImplementedException();
        }


        [Input("String BookingInfo")]
        public InArgument<String> BookingInfo { get; set; }

        [Output("String Response")]
        public OutArgument<String> Response { get; set; }
    }

   

}
