using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Tc.Crm.Plugins
{
    public class ProcessUserHotelTeam
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="context"></param>
        /// <param name="tracingService"></param>
        public ProcessUserHotelTeam(IOrganizationService service, IPluginExecutionContext context, ITracingService tracingService, string[] businessUnitNames)
        {
            this.Service = service;
            this.Context = context;
            this.TracingService = tracingService;
            this.BusinessUnitNames = businessUnitNames;
        }

        /// <summary>
        /// To process Teams, add user to team and add role to team
        /// </summary>
        public void ProcessAddUserToHotelTeam()
        {
            Guid hotelTeamId = Guid.Empty;
            string teamName = string.Empty;
            EntityReferenceCollection users;
            users = GetInformationFromContext(ref hotelTeamId);

            if (hotelTeamId == Guid.Empty || users == null || users.Count == 0)
                return;

            if (!IsHotelTeam(hotelTeamId,  ref teamName))
                return;

            ProcessUserTeams(Context.BusinessUnitId, hotelTeamId, users, teamName);
        }

        /// <summary>
        /// To get teamId, userId from context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="trace"></param>
        /// <param name="hotelTeamId"></param>
        /// <param name="userId"></param>
        private EntityReferenceCollection GetInformationFromContext(ref Guid hotelTeamId)
        {
            EntityReferenceCollection relatedEntities = null;
            if (Context.InputParameters.Contains(InputParameters.Target))
            {
                TracingService.Trace("contains target");
                if (Context.InputParameters[InputParameters.Target] is EntityReference)
                {
                    var targetEntity = (EntityReference)Context.InputParameters[InputParameters.Target];
                    if (targetEntity != null)
                    {
                        TracingService.Trace("Target " + targetEntity.LogicalName + " " + targetEntity.Name);
                        if (targetEntity.LogicalName == Entities.Team)
                            hotelTeamId = targetEntity.Id;
                    }
                }
            }

            if (Context.InputParameters.Contains(InputParameters.RelatedEntities))
            {
                TracingService.Trace("contains related entities");
                if (Context.InputParameters[InputParameters.RelatedEntities] is EntityReferenceCollection)
                {
                    relatedEntities = Context.InputParameters[InputParameters.RelatedEntities] as EntityReferenceCollection;
                    TracingService.Trace("relatedEntities " + relatedEntities.Count);                   
                }
            }
            return relatedEntities;
        }

        /// <summary>
        /// To check whether Team is Hotel Team or not
        /// </summary>
        /// <param name="service"></param>
        /// <param name="teamId"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        private bool IsHotelTeam(Guid teamId, ref string teamName)
        {
            TracingService.Trace("CheckHotelTeam - start");
            bool hotelTeam = false;
            var team = Service.Retrieve(Entities.Team, teamId, new ColumnSet(Attributes.Team.HotelTeam, Attributes.Team.Name));
            if (team != null)
            {
                if (team.Attributes.Contains(Attributes.Team.HotelTeam) && team.Attributes[Attributes.Team.HotelTeam] != null)
                {
                    hotelTeam = bool.Parse(team.Attributes[Attributes.Team.HotelTeam].ToString());
                    TracingService.Trace("HotelTeam is " + hotelTeam);
                }
                if (team.Attributes.Contains(Attributes.Team.Name) && team.Attributes[Attributes.Team.Name] != null)
                {
                    teamName = team.Attributes[Attributes.Team.Name].ToString();
                }

            }
            TracingService.Trace("CheckHotelTeam - end");
            return hotelTeam;
        }


        /// <summary>
        /// To process Users, teams and roles functionality
        /// </summary>
        /// <param name="service"></param>
        /// <param name="context"></param>
        /// <param name="trace"></param>
        /// <param name="businessUnitId"></param>
        /// <param name="teamId"></param>
        /// <param name="userId"></param>
        /// <param name="teamName"></param>
        private void ProcessUserTeams(Guid businessUnitId, Guid hotelTeamId, EntityReferenceCollection users, string teamName)
        {
            TracingService.Trace("ProcessUserTeams - Start");
            var buCollection = GetBusinessUnits(hotelTeamId);
            if (buCollection == null || buCollection.Entities.Count == 0)
                return;
            Dictionary<Guid,string> buToProcess = AddUserToExistingTeam(buCollection, users);
            if (buToProcess != null && buToProcess.Count > 0)
            {
                var roleCollection = GetSecurityRolesByBusinessUnit(buToProcess);
                if (roleCollection == null || roleCollection.Entities.Count != (buToProcess.Count*2))
                    throw new InvalidPluginExecutionException("Security Role Tc.Ids.Base was not created for one or more Business Units.");
                AddUserToNewTeam(buToProcess, roleCollection, users, hotelTeamId, teamName);
            }
            TracingService.Trace("ProcessUserTeams - End");
        }

        /// <summary>
        /// To add security role, user to newly created teams
        /// </summary>
        /// <param name="service"></param>
        /// <param name="trace"></param>
        /// <param name="roleCollection"></param>
        /// <param name="userId"></param>
        /// <param name="teamName"></param>
        private void AddUserToNewTeam(Dictionary<Guid,string> buToProcess, EntityCollection roleCollection, EntityReferenceCollection users, Guid hotelTeamId, string teamName)
        {
            TracingService.Trace("AddUserToNewTeam - Start");
            foreach (KeyValuePair<Guid,string> b in buToProcess)
            {
                var roles = FilterRolesByBusinessUnit(roleCollection, b.Key);
                if (roles != null || roles.Count == 2)
                {
                    var teamId = CreateTeam(hotelTeamId, b.Value + " : " + teamName, b.Key);
                    AssociateRoleToTeam(teamId, roles);
                    AssociateUserToTeam(teamId, users);
                }
            }
            TracingService.Trace("AddUserToNewTeam - End");
        }

        /// <summary>
        /// To filter roles using Business unit Id
        /// </summary>
        /// <param name="securityRoles"></param>
        /// <param name="businessUnitId"></param>
        /// <returns></returns>
        private EntityReferenceCollection FilterRolesByBusinessUnit(EntityCollection securityRoles, Guid businessUnitId)
        {
            TracingService.Trace("FilterRolesByBusinessUnit - Start");
            var rolesOfBusinessUnit = new EntityReferenceCollection();
            IEnumerable<Entity> roles = from role in securityRoles.Entities
                                        where ((EntityReference)role.Attributes[Attributes.Role.BusinessUnitId]).Id == businessUnitId
                                        select role;
            foreach(var role in roles)
            {
                rolesOfBusinessUnit.Add(new EntityReference(role.LogicalName, role.Id));
            }
            TracingService.Trace("FilterRolesByBusinessUnit - End");
            return rolesOfBusinessUnit;
        }

        /// <summary>
        /// To add user to existing teams of Business units
        /// </summary>
        /// <param name="service"></param>
        /// <param name="trace"></param>
        /// <param name="buCollection"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private Dictionary<Guid,string> AddUserToExistingTeam(EntityCollection buCollection, EntityReferenceCollection users)
        {
            TracingService.Trace("AddUserToExistingTeam - Start");
            const string aliasTeamId = "team.teamid";
            List<Guid> processedBusinessUnits = new List<Guid>();
            Dictionary<Guid, string> businessUnitsToProcess = new Dictionary<Guid, string>();
            for (int i = 0; i < buCollection.Entities.Count; i++)
            {
                var teamId = Guid.Empty;
                var buName = string.Empty;
                var buEntity = buCollection.Entities[i];
                if (buEntity != null)
                {
                    if (processedBusinessUnits.Contains(buEntity.Id))
                        continue;
                    processedBusinessUnits.Add(buEntity.Id);
                    if (buEntity.Attributes.Contains(aliasTeamId) && buEntity.Attributes[aliasTeamId] != null)
                    {
                        teamId = Guid.Parse(((AliasedValue)buEntity.Attributes[aliasTeamId]).Value.ToString());
                    }
                    if (teamId == Guid.Empty)
                    {
                        if(buEntity.Attributes.Contains(Attributes.BusinessUnit.Name) && buEntity.Attributes[Attributes.BusinessUnit.Name] != null)
                        {
                            buName = buEntity.Attributes[Attributes.BusinessUnit.Name].ToString();
                            if (!businessUnitsToProcess.Keys.Contains(buEntity.Id))
                            {
                                businessUnitsToProcess.Add(buEntity.Id, buName);
                                TracingService.Trace("buToProcess " + buEntity.Id);
                            }
                        }                        
                        continue;
                    }
                    AssociateUserToTeam(teamId, users);
                }
            }
            TracingService.Trace("AddUserToExistingTeam - End");
            return businessUnitsToProcess;
        }

        /// <summary>
        /// To get Business units that are to be processed
        /// </summary>
        /// <param name="service"></param>
        /// <param name="trace"></param>
        /// <param name="businessUnitId"></param>
        /// <param name="hotelTeamId"></param>
        /// <returns></returns>
        private EntityCollection GetBusinessUnits(Guid hotelTeamId)
        {
            TracingService.Trace("GetBusinessUnits - Start");
            var businessUnitNameCondition = GetBusinessUnitNameConditions();
            var query = string.Format(@"<fetch distinct='true' output-format='xml-platform' version='1.0' mapping='logical'>
                                        <entity name='businessunit'>      
                                        <attribute name='businessunitid' />
                                        <attribute name='name' />                                         
                                        <filter type='or'>
                                          {0}
                                        </filter>
                                        <link-entity name='team' alias='team' from='businessunitid' to='businessunitid' link-type='outer' >
                                        <attribute name='teamid' />
                                        <filter type='and'>                                        
                                         <condition attribute='tc_hotelteamid' operator='eq' value='{1}' />
                                        </filter>
                                        </link-entity>
                                        </entity>
                                        </fetch>", new object[] { businessUnitNameCondition, hotelTeamId });
            var fetch = new FetchExpression(query);
            var businessUnitCollection = Service.RetrieveMultiple(fetch);
            TracingService.Trace("GetBusinessUnits - End");
            return businessUnitCollection;
        }

        /// <summary>
        /// To get Business unit name fetch conditions
        /// </summary>
        /// <returns></returns>
        private string GetBusinessUnitNameConditions()
        {
            if (BusinessUnitNames == null || BusinessUnitNames.Length == 0) throw new InvalidPluginExecutionException("Business unit names were not mentioned in unsecure config of AddUserToHotelTeam plugin.");

            var businessUnitNameCondition = new StringBuilder();
            for (int i = 0; i < BusinessUnitNames.Length; i++)
            {
                businessUnitNameCondition.Append(string.Format("<condition attribute='name' operator='eq' value = '{0}'/>", BusinessUnitNames[i]));
            }
            return businessUnitNameCondition.ToString();
        }

        /// <summary>
        /// To get Security roles for different Business units using name Tc.Ids.Base
        /// </summary>
        /// <param name="service"></param>
        /// <param name="trace"></param>
        /// <param name="businessUnits"></param>
        /// <returns></returns>
        private EntityCollection GetSecurityRolesByBusinessUnit(Dictionary<Guid,string> businessUnits)
        {
            TracingService.Trace("GetSecurityRolesByBu - Start");
            var businessUnitCondition = GetBusinessUnitConditions(businessUnits);
            var query = string.Format(@"<fetch distinct='false' output-format='xml - platform' version='1.0' mapping='logical'>
                                        <entity name='role'>
                                        <attribute name='businessunitid' />
                                        <attribute name='roleid' />
                                        <filter type='and'>
                                         <filter type='or'>
                                          <condition attribute='name' operator='eq' value='{0}' />
                                          <condition attribute='name' operator='eq' value='{1}' />
                                         </filter >
                                         <condition attribute='businessunitid' operator='in' >
                                          {2}
                                         </condition >
                                        </filter >
                                        </entity >
                                        </fetch > ", General.RoleTcIdBase,General.RoleTcIdRep, businessUnitCondition);
            var fetch = new FetchExpression(query);
            var roleCollection = Service.RetrieveMultiple(fetch);
            TracingService.Trace("GetSecurityRolesByBu - End");
            return roleCollection;
        }

        /// <summary>
        /// To get fetchxml Business unit conditions
        /// </summary>
        /// <param name="businessUnits"></param>
        /// <returns></returns>
        private string GetBusinessUnitConditions(Dictionary<Guid,string> businessUnits)
        {
            var businessUnitCondition = new StringBuilder();
            foreach(KeyValuePair<Guid,string> b in businessUnits)
            {
                businessUnitCondition.Append(string.Format("<value>{0}</value>", b.Key.ToString()));
            }
            return businessUnitCondition.ToString();
        }


        /// <summary>
        /// To create Team
        /// </summary>
        /// <param name="service"></param>
        /// <param name="trace"></param>
        /// <param name="teamName"></param>
        /// <param name="businessUnitId"></param>
        /// <returns></returns>
        private Guid CreateTeam(Guid hotelTeamId, string teamName, Guid businessUnitId)
        {
            TracingService.Trace("CreateTeam - Start");
            Entity team = new Entity(Entities.Team);
            team.Attributes.Add(Attributes.Team.Name, teamName);
            team.Attributes.Add(Attributes.Team.HotelTeam, true);
            team.Attributes.Add(Attributes.Team.BusinessUnitId, new EntityReference(Entities.BusinessUnit, businessUnitId));
            team.Attributes.Add(Attributes.Team.HotelTeamId, new EntityReference(Entities.Team, hotelTeamId));
            Guid teamId = Service.Create(team);
            TracingService.Trace("CreateTeam - End");
            return teamId;
        }

        /// <summary>
        /// To Asscoiate Security role to Team
        /// </summary>
        /// <param name="service"></param>
        /// <param name="trace"></param>
        /// <param name="teamId"></param>
        /// <param name="roleId"></param>
        private void AssociateRoleToTeam(Guid teamId, EntityReferenceCollection roles)
        {
            TracingService.Trace("AssociateRoleToTeam - Start");
            Service.Associate(Entities.Team,
                             teamId,
                             new Relationship(Relationships.TeamRolesAssociation),
                             roles);
            TracingService.Trace("AssociateRoleToTeam - End");
        }

        /// <summary>
        /// To Associate User to Team
        /// </summary>
        /// <param name="service"></param>
        /// <param name="trace"></param>
        /// <param name="teamId"></param>
        /// <param name="userId"></param>
        private void AssociateUserToTeam(Guid teamId, EntityReferenceCollection users)
        {
            TracingService.Trace("AssociateUserToTeam - Start");
            Service.Associate(Entities.Team,
                            teamId,
                            new Relationship(Relationships.TeamMembershipAssociation),
                            users);
            TracingService.Trace("AssociateUserToTeam - End");
        }

        private IPluginExecutionContext Context { get; set; }
        private ITracingService TracingService { get; set; }
        private IOrganizationService Service { get; set; }
        private string[] BusinessUnitNames { get; set; }
    }

    
}
