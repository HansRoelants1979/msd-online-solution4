using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.CustomWorkflowSteps.ExecutingUserInDepartment.Service;


namespace Tc.Crm.CustomWorkflowSteps.ExecutingUserInDepartment
{
    public class ExecutingUserInDepartmentActivity : CodeActivity
    {

        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService trace = executionContext.GetExtension<ITracingService>();
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                var SecurityRoleName = string.Empty;

                SecurityRoleName = deptBaseSecurityRoleName.Get<string>(executionContext);
                var UserId = context.UserId;

                if (UserId != null)
                {
                    var response = RetrieveSecurityRoles.GetSecurityRoles(SecurityRoleName,UserId,service);
                    if (response != false)
                        Bool.Set(executionContext, response);
                }
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
        [Input("deptBaseSecurityRoleName")]
        public InArgument<string> deptBaseSecurityRoleName { get; set; }

        [Output("Bool")]
        [Default("false")]
        public OutArgument<bool>Bool { get; set; }
        


    }
}
