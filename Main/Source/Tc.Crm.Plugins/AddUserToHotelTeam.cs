using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;

namespace Tc.Crm.Plugins
{
    public class AddUserToHotelTeam : IPlugin
    {
        public string[] bu;
        public AddUserToHotelTeam(string unSecureConfig, string SecureConfig)
        {
            bu = unSecureConfig.Split(',');
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext localContext = (IPluginExecutionContext)
               serviceProvider.GetService(typeof(IPluginExecutionContext));

            ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(localContext.UserId);
            try
            {
                tracingService.Trace("Begin - CreateTeam");
                if (localContext.MessageName == Messages.Associate)
                {
                    string relationshipName = string.Empty;
                    Guid teamId = Guid.Empty, userId = Guid.Empty;
                    if (localContext.InputParameters.Contains(InputParameters.Relationship))
                    {
                        relationshipName = localContext.InputParameters[InputParameters.Relationship].ToString();
                        tracingService.Trace("Relationship " + relationshipName);
                    }

                    if (relationshipName != Relationships.TeamMembershipAssociation)
                        return;

                    GetInformationFromContext(localContext, tracingService, teamId, userId);

                    if (teamId == Guid.Empty || userId == Guid.Empty)
                        return;

                    if (!IsHotelTeam(service, teamId, tracingService))
                        return;

                    ProcessUserTeams(service, localContext, tracingService, localContext.BusinessUnitId, teamId);

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

       

        /// <summary>
        /// To get teamId, userId from context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="trace"></param>
        /// <param name="teamId"></param>
        /// <param name="userId"></param>
        private void GetInformationFromContext(IPluginExecutionContext context, ITracingService trace,  Guid teamId, Guid userId)
        {
            EntityReference targetEntity = null;
            string relationshipName = string.Empty;
            EntityReferenceCollection relatedEntities = null;
            EntityReference relatedEntity = null;

            if (context.InputParameters.Contains(InputParameters.Target))
            {
                trace.Trace("contains target");
                if (context.InputParameters[InputParameters.Target] is EntityReference)
                {
                    targetEntity = (EntityReference)context.InputParameters[InputParameters.Target];
                    trace.Trace("Target " + targetEntity.LogicalName + " " + targetEntity.Name);
                    if(targetEntity.LogicalName == Entities.Team)
                        teamId = targetEntity.Id;
                }
            }

            if (context.InputParameters.Contains(InputParameters.RelatedEntities))
            {
                trace.Trace("contains related entities");
                if (context.InputParameters[InputParameters.RelatedEntities] is EntityReferenceCollection)
                {
                    relatedEntities = context.InputParameters[InputParameters.RelatedEntities] as EntityReferenceCollection;
                    relatedEntity = relatedEntities[0];
                    trace.Trace("Related Entities " + relatedEntity.LogicalName + " " + relatedEntity.Name);
                    if (relatedEntity.LogicalName == Entities.User)
                        userId = relatedEntity.Id;
                }
            }
        }

        /// <summary>
        /// To check whether Team is Hotel Team or not
        /// </summary>
        /// <param name="service"></param>
        /// <param name="teamId"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        private bool IsHotelTeam(IOrganizationService service, Guid teamId, ITracingService trace)
        {
            trace.Trace("CheckHotelTeam - start");
            bool hotelTeam = false;
            var team = service.Retrieve(Entities.Team, teamId, new ColumnSet(Attributes.Team.HotelTeam));
            if(team != null && team.Attributes.Contains(Attributes.Team.HotelTeam) && team.Attributes[Attributes.Team.HotelTeam] != null)
            {                
                hotelTeam = bool.Parse(team.Attributes[Attributes.Team.HotelTeam].ToString());
                trace.Trace("HotelTeam is " + hotelTeam);
                
            }
            trace.Trace("CheckHotelTeam - end");
            return hotelTeam;
        }

        private void GetBusinessUnits(IOrganizationService service, IPluginExecutionContext context, ITracingService trace)
        {
            List<ConditionExpression> buNameConditions = new List<ConditionExpression>();
            for (int i = 0; i < bu.Length; i++)
            {
                buNameConditions.Add(new ConditionExpression(Attributes.BusinessUnit.Name, ConditionOperator.Equal, bu[i]));
            }
            FilterExpression filterByName = new FilterExpression(LogicalOperator.Or);
            filterByName.Conditions.AddRange(buNameConditions.ToArray());
            QueryExpression queryForBusinessUnit = new QueryExpression(Entities.BusinessUnit)
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
                                new ConditionExpression(Attributes.BusinessUnit.BusinessUnitId,ConditionOperator.NotEqual, context.BusinessUnitId)                               
                            }
                        },
                    }
                },
                LinkEntities =
                {
                    new LinkEntity(Entities.BusinessUnit,Entities.Team,Attributes.BusinessUnit.BusinessUnitId,Attributes.Team.BusinessUnitId,JoinOperator.LeftOuter)
                    {
                        Columns = new ColumnSet(new string[] { "name" }),
                        LinkCriteria =
                        {
                            Filters =
                            {
                                new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        //new ConditionExpression(Attributes.Team.Name, ConditionOperator.EndsWith, "TeamName"),
                                        new ConditionExpression(Attributes.Team.HotelTeam, ConditionOperator.Equal,true),
                                        new ConditionExpression(Attributes.Team.HotelTeamId, ConditionOperator.Equal, Guid.Empty)
                                    }
                                }
                            }
                        }
                    }
                }
                
            };
            queryForBusinessUnit.Criteria.Filters.Add(filterByName);     
           

        }

        private void ProcessUserTeams(IOrganizationService service, IPluginExecutionContext context, ITracingService trace, Guid businessUnitId, Guid teamId)
        {
           var buCollection = GetBusinessUnits(service, trace, businessUnitId, teamId);
           
        }

        private EntityCollection GetBusinessUnits(IOrganizationService service, ITracingService trace, Guid businessUnitId, Guid teamId)
        {
            var buNameCondition = GetBuNameConditions();
            var query = string.Format(@"<fetch distinct='true' output-format='xml-platform' version='1.0' mapping='logical'>
                                        <entity name='businessunit'>      
                                        <attribute name='businessunitid' />
                                        <filter type='and'>
                                         <filter type='or'>
                                          {0}
                                         </filter>
                                         <condition attribute='businessunitid' operator='ne' value='{1}' />
                                        </filter>
                                        <link-entity name='team' alias='team' from='businessunitid' to='businessunitid' link-type='outer' >
                                        <attribute name='teamid' />
                                        <filter type='and'>                                        
                                         <condition attribute='tc_hotelteamid' operator='eq' value='{2}' />
                                        </filter>
                                        </link-entity>
                                        </entity>
                                        </fetch>", new object[] { buNameCondition, businessUnitId, teamId });
            var fetch = new FetchExpression(query);
            var buCollection = service.RetrieveMultiple(fetch);
            return buCollection;
        }

        private string GetBuNameConditions()
        {
            var buNameCondition = new StringBuilder();
            for(int i=0; i< bu.Length; i++)
            {
                buNameCondition.Append(string.Format("<condition attribute='name' operator='eq' value = '{0}'", bu[i]));
            }
            return buNameCondition.ToString();
        } 

        private Guid CreateTeam(IOrganizationService service, ITracingService trace, string teamName, Guid businessUnitId)
        {
            Entity team = new Entity(Entities.Team);
            team.Attributes.Add(Attributes.Team.Name, teamName);            
            team.Attributes.Add(Attributes.Team.BusinessUnitId, new EntityReference(Entities.BusinessUnit,
                businessUnitId));
            Guid teamId = service.Create(team);
            return teamId;
        }

        private void AssociateRoleToTeam(IOrganizationService service, ITracingService trace, Guid teamId, Guid roleId)
        {
            service.Associate(Entities.Team, 
                             teamId, 
                             new Relationship(Relationships.TeamRolesAssociation),
                             new EntityReferenceCollection()
                             {
                             new EntityReference(Entities.Role, roleId)
                             });
        }

        private void AssociateUserToTeam(IOrganizationService service, ITracingService trace, Guid teamId, Guid userId)
        {
            service.Associate(Entities.Team,
                            teamId,
                            new Relationship(Relationships.TeamRolesAssociation),
                            new EntityReferenceCollection()
                            {
                             new EntityReference(Entities.User, userId)
                            });
        }
    }
}
