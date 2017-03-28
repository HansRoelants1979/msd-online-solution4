using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Tc.Crm.Plugins
{
    public class CreateHotelOwner : IPlugin
    {
        void IPlugin.Execute(IServiceProvider serviceProvider)
        {
            Microsoft.Xrm.Sdk.IPluginExecutionContext localContext = (Microsoft.Xrm.Sdk.IPluginExecutionContext)
                serviceProvider.GetService(typeof(Microsoft.Xrm.Sdk.IPluginExecutionContext));

            ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            tracingService.Trace("Begin - CreateHotelOwner");

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(localContext.UserId);

            Entity hotel = localContext.InputParameters["Target"] as Entity;
            string hotelName = hotel.GetAttributeValue<string>("tc_name");
            
            if (!string.IsNullOrEmpty(hotelName))
            {
                string teamName = string.Format("Hotel Team: {0}", hotelName);
                tracingService.Trace("Output - Creating Team with Name: {0}", teamName);
                Entity team = new Entity("team");
                team.Attributes.Add("name", teamName);
                team.Attributes.Add("tc_hotelteam", true);
                team.Attributes.Add("businessunitid", new EntityReference("businessunit",
                    localContext.BusinessUnitId));                    
                Guid teamId = service.Create(team);
                tracingService.Trace("Output - Created Team with Id: {0}", teamId);

                QueryExpression queryForSecurityRole = new QueryExpression("role")
                {
                    Distinct = false,
                    Criteria =
                    {
                        Filters =
                        {
                            new FilterExpression
                            {
                                FilterOperator = LogicalOperator.And,
                                Conditions =
                                {
                                    new ConditionExpression("businessunitid", ConditionOperator.Equal, localContext.BusinessUnitId),
                                    new ConditionExpression("name", ConditionOperator.Equal, "Tc.Ids.Base")
                                },
                            },
                        }
                    }
                };

                DataCollection<Entity> entityCollection = service.RetrieveMultiple(queryForSecurityRole).Entities;
                tracingService.Trace("Output - Query for security roles");
                if (entityCollection.Count == 1)
                {
                    tracingService.Trace("Output - One security role with found with matching name and BU");
                    service.Associate(
                        "team",
                        teamId,
                        new Relationship("teamroles_association"),
                        new EntityReferenceCollection()
                        {
                            new EntityReference("role", entityCollection[0].Id)
                        });
                    tracingService.Trace("Output - Associated security role with team");
                    hotel["ownerid"] = new EntityReference("team", teamId);
                    tracingService.Trace("Output - Update owner of hotel");
                }
                else                    
                    throw new InvalidPluginExecutionException("The role Tc.Ids.Base does not exist in the required business unit");

                tracingService.Trace("End - CreateHotelOwner");
            }
        }
    }
}
