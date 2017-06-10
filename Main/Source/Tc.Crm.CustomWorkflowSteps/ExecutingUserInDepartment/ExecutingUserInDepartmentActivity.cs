using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.ServiceModel;
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

                SecurityRoleName = DeptBaseSecurityRoleName.Get<string>(executionContext);
                if (SecurityRoleName == null || SecurityRoleName == "")
                {
                    throw new InvalidPluginExecutionException("SecurityRoleName is null");
                }
                else
                {
                    SecurityRoleName = SecurityRoleName.ToLower();
                }
                var UserId = context.InitiatingUserId;

                
                    trace.Trace("Checking executing User is in role or not");
                    var response = RetrieveSecurityRoles.GetSecurityRoles(SecurityRoleName,UserId,service,trace);
                    if (response != false)
                    {
                        IsInRole.Set(executionContext, response);
                    }
                    else
                    {
                        trace.Trace("Executing User is not in Role");
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

        [RequiredArgument]
        [Input("DeptBaseSecurityRoleName")]
        public InArgument<string> DeptBaseSecurityRoleName { get; set; }

        [Output("IsInRole")]
        [Default("false")]
        public OutArgument<bool> IsInRole { get; set; }
        


    }
}
