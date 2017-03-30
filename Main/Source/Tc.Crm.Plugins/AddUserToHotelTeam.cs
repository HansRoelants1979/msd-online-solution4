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
                if (localContext.Depth > 1)
                    return;
                tracingService.Trace("Begin - CreateTeam");
                if (localContext.MessageName == Messages.Associate)
                {
                    string relationshipName = string.Empty, teamName = string.Empty;
                    Guid teamId = Guid.Empty, userId = Guid.Empty;
                    if (localContext.InputParameters.Contains(InputParameters.Relationship))
                    {
                        relationshipName = localContext.InputParameters[InputParameters.Relationship].ToString();
                        tracingService.Trace("Relationship " + relationshipName);
                    }
                   
                    if (relationshipName != Relationships.TeamMembershipAssociation + ".")
                        return;

                    GetInformationFromContext(localContext, tracingService,ref teamId, ref userId);

                    if (teamId == Guid.Empty || userId == Guid.Empty)
                        return;

                    if (!IsHotelTeam(service, teamId, tracingService, ref teamName))
                        return;

                    ProcessUserTeams(service, localContext, tracingService, localContext.BusinessUnitId, teamId, userId, teamName);
                    
                    tracingService.Trace("End - CreateTeam");
                    
                }

                //throw new InvalidPluginExecutionException("Throwing error");
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
        private void GetInformationFromContext(IPluginExecutionContext context, ITracingService trace,ref  Guid teamId, ref Guid userId)
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
        private bool IsHotelTeam(IOrganizationService service, Guid teamId, ITracingService trace,ref string teamName)
        {
            trace.Trace("CheckHotelTeam - start");
            bool hotelTeam = false;
            var team = service.Retrieve(Entities.Team, teamId, new ColumnSet(Attributes.Team.HotelTeam, Attributes.Team.Name));
            if(team != null)
            {
                if (team.Attributes.Contains(Attributes.Team.HotelTeam) && team.Attributes[Attributes.Team.HotelTeam] != null)
                {
                    hotelTeam = bool.Parse(team.Attributes[Attributes.Team.HotelTeam].ToString());
                    trace.Trace("HotelTeam is " + hotelTeam);
                }
                if(team.Attributes.Contains(Attributes.Team.Name) && team.Attributes[Attributes.Team.Name] != null)
                {
                    teamName = team.Attributes[Attributes.Team.Name].ToString();
                }
                
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

        private void ProcessUserTeams(IOrganizationService service, IPluginExecutionContext context, ITracingService trace, Guid businessUnitId, Guid teamId, Guid userId, string teamName)
        {
            trace.Trace("ProcessUserTeams - Start");
            var buCollection = GetBusinessUnits(service, trace, businessUnitId, teamId);
            if (buCollection == null || buCollection.Entities.Count == 0)
                return;
            List<Guid> buToProcess = AddUserToExistingTeam(service, trace, buCollection, userId);
            if(buToProcess != null && buToProcess.Count > 0)
            {
                var roleCollection = GetSecurityRolesByBu(service, trace, buToProcess);
                AddUserToNewTeam(service, trace, roleCollection, userId, teamName);
            }
            trace.Trace("ProcessUserTeams - End");
        }

        private void AddUserToNewTeam(IOrganizationService service, ITracingService trace,EntityCollection roleCollection, Guid userId, string teamName)
        {
            trace.Trace("AddUserToNewTeam - Start");
            if(roleCollection != null && roleCollection.Entities.Count > 0)
            {
                Guid businessUnitId = Guid.Empty;
                for(int i=0; i<roleCollection.Entities.Count; i++)
                {
                    var role = roleCollection.Entities[i];
                    if(role.Attributes.Contains(Attributes.Role.BusinessUnitId) && role.Attributes[Attributes.Role.BusinessUnitId] != null)
                    {
                        businessUnitId = ((EntityReference)role.Attributes[Attributes.Role.BusinessUnitId]).Id;
                        var teamId = CreateTeam(service, trace, teamName, businessUnitId);
                        AssociateRoleToTeam(service, trace, teamId, role.Id);
                        AssociateUserToTeam(service, trace, teamId, userId);
                    }
                }
            }
            trace.Trace("AddUserToNewTeam - End");
        }

        private List<Guid> AddUserToExistingTeam(IOrganizationService service, ITracingService trace, EntityCollection buCollection, Guid userId)
        {
            trace.Trace("AddUserToExistingTeam - Start");
            const string aliasTeam = "team.teamid";
            List<Guid> processedBu = new List<Guid>();
            List<Guid> buToProcess = new List<Guid>();
            for (int i = 0; i < buCollection.Entities.Count; i++)
            {
                var teamId = Guid.Empty;
                var buEntity = buCollection.Entities[i];
                if (buEntity != null)
                {
                    if (processedBu.Contains(buEntity.Id))
                        continue;
                    processedBu.Add(buEntity.Id);
                    if (buEntity.Attributes.Contains(aliasTeam) && buEntity.Attributes[aliasTeam] != null)
                    {
                        teamId = Guid.Parse(((AliasedValue)buEntity.Attributes[aliasTeam]).Value.ToString());
                    }
                    if (teamId == Guid.Empty)
                    {
                        buToProcess.Add(buEntity.Id);
                        trace.Trace("buToProcess " + buEntity.Id);
                        continue;
                    }
                    AssociateUserToTeam(service, trace, teamId, userId);
                }
            }
            trace.Trace("AddUserToExistingTeam - End");
            return buToProcess;
        }

        private EntityCollection GetBusinessUnits(IOrganizationService service, ITracingService trace, Guid businessUnitId, Guid teamId)
        {
            trace.Trace("GetBusinessUnits - Start");
            var buNameCondition = GetBuNameConditions();
            var query = string.Format(@"<fetch distinct='true' output-format='xml-platform' version='1.0' mapping='logical'>
                                        <entity name='businessunit'>      
                                        <attribute name='businessunitid' />
                                        <attribute name='name' />
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
            trace.Trace("GetBusinessUnits - End");
            return buCollection;
        }

        private string GetBuNameConditions()
        {            
            var buNameCondition = new StringBuilder();
            for (int i = 0; i < bu.Length; i++)
            {
                buNameCondition.Append(string.Format("<condition attribute='name' operator='eq' value = '{0}'/>", bu[i]));
            }
            return buNameCondition.ToString();
        }

        private EntityCollection GetSecurityRolesByBu(IOrganizationService service, ITracingService trace, List<Guid> businessUnits)
        {
            trace.Trace("GetSecurityRolesByBu - Start");
            var buCondition = GetBuConditions(businessUnits);
            var query = string.Format(@"<fetch distinct='false' output-format='xml - platform' version='1.0' mapping='logical'>
                                        <entity name='role'>
                                        <attribute name='businessunitid' />
                                        <attribute name='roleid' />
                                        <filter type='and'>
                                         <condition attribute='name' operator='eq' value='{0}' />
                                         <condition attribute='businessunitid' operator='in' >
                                          {1}
                                         </condition >
                                        </filter >
                                        </entity >
                                        </fetch > ", General.TeamRoleName, buCondition);
            var fetch = new FetchExpression(query);
            var roleCollection = service.RetrieveMultiple(fetch);
            trace.Trace("GetSecurityRolesByBu - End");
            return roleCollection;
        }

        private string GetBuConditions(List<Guid> businessUnits)
        {
            var buCondition = new StringBuilder();
            for (int i = 0; i < businessUnits.Count; i++)
            {
                buCondition.Append(string.Format("<value>{0}</value>", businessUnits[i]));
            }
            return buCondition.ToString();
        }

             

        private Guid CreateTeam(IOrganizationService service, ITracingService trace, string teamName, Guid businessUnitId)
        {
            trace.Trace("CreateTeam - Start");
            Entity team = new Entity(Entities.Team);
            team.Attributes.Add(Attributes.Team.Name, teamName);            
            team.Attributes.Add(Attributes.Team.BusinessUnitId, new EntityReference(Entities.BusinessUnit,
                businessUnitId));
            Guid teamId = service.Create(team);
            trace.Trace("CreateTeam - End");
            return teamId;
        }

        private void AssociateRoleToTeam(IOrganizationService service, ITracingService trace, Guid teamId, Guid roleId)
        {
            trace.Trace("AssociateRoleToTeam - Start");
            service.Associate(Entities.Team, 
                             teamId, 
                             new Relationship(Relationships.TeamRolesAssociation),
                             new EntityReferenceCollection()
                             {
                             new EntityReference(Entities.Role, roleId)
                             });
            trace.Trace("AssociateRoleToTeam - End");
        }

        private void AssociateUserToTeam(IOrganizationService service, ITracingService trace, Guid teamId, Guid userId)
        {
            trace.Trace("AssociateUserToTeam - Start");
            service.Associate(Entities.Team,
                            teamId,
                            new Relationship(Relationships.TeamMembershipAssociation),
                            new EntityReferenceCollection()
                            {
                             new EntityReference(Entities.User, userId)
                            });
            trace.Trace("AssociateUserToTeam - End");
        }
    }
}
