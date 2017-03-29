using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.ServiceModel;


namespace Tc.Crm.Plugins
{
    public class AddUserToHotelTeam : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            Microsoft.Xrm.Sdk.IPluginExecutionContext localContext = (Microsoft.Xrm.Sdk.IPluginExecutionContext)
               serviceProvider.GetService(typeof(Microsoft.Xrm.Sdk.IPluginExecutionContext));

            ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(localContext.UserId);
            try
            {
                tracingService.Trace("Begin - CreateTeam");
                if (localContext.MessageName == "Associate")
                {
                    string relationshipName = string.Empty;
                    if (localContext.InputParameters.Contains("Relationship"))
                    {
                        relationshipName = localContext.InputParameters["Relationship"].ToString();
                        tracingService.Trace("Relationship " + relationshipName);
                    }
                    if (relationshipName != "teammembership_association.")
                    {
                        return;
                    }
                    CreateTeam(localContext, tracingService);
                    tracingService.Trace("End - CreateTeam");
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

        private void CreateTeam(IPluginExecutionContext context, ITracingService trace)
        {
            EntityReference targetEntity = null;
            string relationshipName = string.Empty;
            EntityReferenceCollection relatedEntities = null;
            EntityReference relatedEntity = null;

            if (context.InputParameters.Contains("Target"))
            {
                trace.Trace("contains target");
                if (context.InputParameters["Target"] is EntityReference)
                {
                    targetEntity = (EntityReference)context.InputParameters["Target"];
                    trace.Trace("Target " + targetEntity.LogicalName + " " + targetEntity.Name);
                }
            }

            if (context.InputParameters.Contains("RelatedEntities"))
            {
                trace.Trace("contains related entities");
                if (context.InputParameters["RelatedEntities"] is EntityReferenceCollection)
                {
                    relatedEntities = context.InputParameters["RelatedEntities"] as EntityReferenceCollection;
                    relatedEntity = relatedEntities[0];
                    trace.Trace("Related Entities " + relatedEntity.LogicalName + " " + relatedEntity.Name);
                }
            }
                
            

        }
    }
}
