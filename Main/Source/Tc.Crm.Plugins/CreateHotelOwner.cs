using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.ServiceModel;

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

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(localContext.UserId);
            try
            {
                tracingService.Trace("Begin - CreateHotelOwner");
                CreateTeam(service, localContext, tracingService);
                tracingService.Trace("End - CreateHotelOwner");
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

        private void CreateTeam(IOrganizationService service, IPluginExecutionContext context, ITracingService trace)
        {
            
            if (context.InputParameters.Contains(InputParameters.Target) && context.InputParameters[InputParameters.Target] is Entity)
            {
                Entity hotel = context.InputParameters["Target"] as Entity;
                string hotelName = hotel.GetAttributeValue<string>("tc_name");

                if (!string.IsNullOrEmpty(hotelName))
                {
                    string teamName = string.Format("Hotel Team: {0}", hotelName);
                    trace.Trace("Output - Creating Team with Name: {0}", teamName);
                    Entity team = new Entity(Entities.Team);
                    team.Attributes.Add(Attributes.Team.Name, teamName);
                    team.Attributes.Add(Attributes.Team.HotelTeam, true);
                    team.Attributes.Add(Attributes.Team.BusinessUnitId, new EntityReference(Entities.BusinessUnit,
                        context.BusinessUnitId));
                    Guid teamId = service.Create(team);
                    if (teamId != Guid.Empty)
                    {
                        trace.Trace("Output - Created Team with Id: {0}", teamId);
                        AssociateSecurityRole(service, context, trace, teamId);
                        hotel.Attributes[Attributes.Hotel.Owner] = new EntityReference(Entities.Team, teamId);
                        trace.Trace("Output - Update owner of hotel");
                    }
                }
            }
            
        }

        private void AssociateSecurityRole(IOrganizationService service, IPluginExecutionContext context, ITracingService trace, Guid teamId)
        {
            QueryExpression queryForSecurityRole = new QueryExpression(Entities.Role)
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
                                    new ConditionExpression(Attributes.Role.BusinessUnitId, ConditionOperator.Equal, context.BusinessUnitId),
                                    new ConditionExpression(Attributes.Role.Name, ConditionOperator.Equal, General.TeamRoleName)
                                },
                            },
                        }
                    }
            };

            DataCollection<Entity> entityCollection = service.RetrieveMultiple(queryForSecurityRole).Entities;
            trace.Trace("Output - Query for security roles");
            if (entityCollection.Count == 1)
            {
                trace.Trace("Output - One security role with found with matching name and BU");
                service.Associate(
                    Entities.Team,
                    teamId,
                    new Relationship(Relationships.TeamRolesAssociation),
                    new EntityReferenceCollection()
                    {
                            new EntityReference(Entities.Role, entityCollection[0].Id)
                    });
                trace.Trace("Output - Associated security role with team");
            }
            else
                throw new InvalidPluginExecutionException("The role Tc.Ids.Base does not exist in the required business unit");
        }
    }
}
