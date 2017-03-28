using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Tc.Crm.Plugins
{
    public class CreateSharePointDocumentLocation : IPlugin
    {
        void IPlugin.Execute(IServiceProvider serviceProvider)
        {
            Microsoft.Xrm.Sdk.IPluginExecutionContext localContext = (Microsoft.Xrm.Sdk.IPluginExecutionContext)
                serviceProvider.GetService(typeof(Microsoft.Xrm.Sdk.IPluginExecutionContext));

            ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(localContext.UserId);

            Entity note = localContext.InputParameters["Target"] as Entity;
            string fileName = note.GetAttributeValue<string>("filename");
            if (!string.IsNullOrEmpty(fileName))
            {                
                EntityReference regarding = note.GetAttributeValue<EntityReference>("objectid");
                if (regarding != null && regarding.LogicalName == "incident")
                {
                    QueryByAttribute query = new QueryByAttribute("sharepointdocumentlocation");
                    query.ColumnSet = new ColumnSet("name");                    
                    query. AddAttributeValue("regardingobjectid", regarding.Id);
                    EntityCollection sharepoinDocumentLocations = service.RetrieveMultiple(query);
                    if (sharepoinDocumentLocations.Entities.Count == 0)
                    {
                        Entity incident = service.Retrieve(regarding.LogicalName, regarding.Id,
                            new ColumnSet("ticketnumber"));
                        if (incident != null)
                        {
                            QueryByAttribute queryForDocumentLocationsRelativeToIncident = new QueryByAttribute("sharepointdocumentlocation");
                            queryForDocumentLocationsRelativeToIncident.ColumnSet = new ColumnSet("name");
                            queryForDocumentLocationsRelativeToIncident.AddAttributeValue("relativeurl", "incident");
                            EntityCollection sharepoinDocumentLocationsRelativeToIncident = service.
                                RetrieveMultiple(queryForDocumentLocationsRelativeToIncident);
                            if (sharepoinDocumentLocationsRelativeToIncident != null &&
                                sharepoinDocumentLocationsRelativeToIncident.Entities.Count == 1)
                            {
                                string ticketNumber = incident.GetAttributeValue<string>("ticketnumber");
                                Entity sharepointDocumentLocation = new Entity("sharepointdocumentlocation");
                                sharepointDocumentLocation.Attributes.Add("name", ticketNumber);
                                sharepointDocumentLocation.Attributes.Add("parentsiteorlocation", new EntityReference("sharepointdocumentlocation",
                                    sharepoinDocumentLocationsRelativeToIncident.Entities[0].Id));
                                sharepointDocumentLocation.Attributes.Add("relativeurl", ticketNumber);
                                sharepointDocumentLocation.Attributes.Add("regardingobjectid", regarding);
                                service.Create(sharepointDocumentLocation);
                            }
                            else
                                throw new InvalidPluginExecutionException("Unable to find base document location incident for incidents");
                        }
                    }
                }
            }
        }
    }
}
