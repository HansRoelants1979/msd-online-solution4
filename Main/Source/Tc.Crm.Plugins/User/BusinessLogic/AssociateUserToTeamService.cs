using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.Plugins.User.BusinessLogic
{
    public class AssociateUserToTeamService
    {
        public IPluginExecutionContext context = null;
        public IOrganizationService service = null;
        public ITracingService trace = null;
        public string[] businessUnits;

        public AssociateUserToTeamService(IPluginExecutionContext context, ITracingService trace, IOrganizationService service, string[] businessUnits)
        {
            this.context = context;
            this.service = service;
            this.trace = trace;
            this.businessUnits = businessUnits;
        }

        private bool IsContextValid()
        {
            if (!context.MessageName.Equals(Messages.Associate, StringComparison.OrdinalIgnoreCase)) return false;
            if (context.Stage != (int)PluginStage.Postoperation) return false;
            if (!context.InputParameters.Contains(InputParameters.Relationship))
                return false;
            var relationshipName = context.InputParameters[InputParameters.Relationship].ToString();
            if (relationshipName != Relationships.TeamMembershipAssociation + ".")
                return false;
            return true;
        }

        public void DoActionsOnUserAssociate()
        {
            if (!IsContextValid()) return;

            var hotelTeamId = GetHotelTeamId();
            var users = GetUsersFromContext();

            if (hotelTeamId == Guid.Empty || users == null || users.Count == 0)
                return;

            var hotelTeam = GetHotelTeam(hotelTeamId);
            var isParentHotelTeam = IsParentHotelTeam(hotelTeam);
            var teamName = GetAttribute(hotelTeam, Attributes.Team.Name).ToString();
            if (!isParentHotelTeam)
                return;

            ProcessUserTeams(context.BusinessUnitId, hotelTeamId, users, teamName);
        }


        private object GetAttribute(Entity e, string name)
        {
            if (!e.Contains(name)) return null;
            return e[name];
        }

        private Entity GetHotelTeam(Guid teamId)
        {
            return service.Retrieve(Entities.Team
                                    , teamId
                                    , new ColumnSet(Attributes.Team.HotelTeam
                                                    , Attributes.Team.Name
                                                    , Attributes.Team.HotelTeamId));
        }

        /// <summary>
        /// To get teamId, users from context
        /// </summary>
        /// <param name="hotelTeamId"></param>
        /// <returns></returns>
        private EntityReferenceCollection GetUsersFromContext()
        {
            EntityReferenceCollection relatedEntities = null;


            if (context.InputParameters.Contains(InputParameters.RelatedEntities))
            {
                trace.Trace("contains related entities");
                if (context.InputParameters[InputParameters.RelatedEntities] is EntityReferenceCollection)
                {
                    relatedEntities = context.InputParameters[InputParameters.RelatedEntities] as EntityReferenceCollection;
                    trace.Trace("relatedEntities " + relatedEntities.Count);
                }
            }
            return relatedEntities;
        }

        private Guid GetHotelTeamId()
        {
            if (context.InputParameters.Contains(InputParameters.Target))
            {
                trace.Trace("contains target");
                if (context.InputParameters[InputParameters.Target] is EntityReference)
                {
                    var targetEntity = (EntityReference)context.InputParameters[InputParameters.Target];
                    if (targetEntity != null)
                    {
                        trace.Trace("Target " + targetEntity.LogicalName + " " + targetEntity.Name);
                        if (targetEntity.LogicalName == Entities.Team)
                            return targetEntity.Id;
                    }
                }
            }

            return Guid.Empty;
        }

        /// <summary>
        /// To check whether Team is Hotel Team or not
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="teamName"></param>
        /// <returns></returns>
        private bool IsParentHotelTeam(Entity team)
        {
            trace.Trace("CheckHotelTeam - start");
            var hotelTeam = false;
            if (team == null) return hotelTeam;
            if (team.Attributes.Contains(Attributes.Team.HotelTeam) && team.Attributes[Attributes.Team.HotelTeam] != null)
            {
                if (!team.Attributes.Contains(Attributes.Team.HotelTeamId) || team.Attributes[Attributes.Team.HotelTeamId] == null)
                {
                    hotelTeam = bool.Parse(team.Attributes[Attributes.Team.HotelTeam].ToString());
                    trace.Trace("HotelTeam is " + hotelTeam);
                }
            }
            trace.Trace("CheckHotelTeam - end");
            return hotelTeam;
        }


        /// <summary>
        /// To process Users, teams and roles functionality
        /// </summary>
        /// <param name="businessUnitId"></param>
        /// <param name="hotelTeamId"></param>
        /// <param name="users"></param>
        /// <param name="teamName"></param>
        private void ProcessUserTeams(Guid businessUnitId, Guid hotelTeamId, EntityReferenceCollection users, string teamName)
        {
            trace.Trace("ProcessUserTeams - Start");
            var businessUnits = GetBusinessUnits(hotelTeamId);
            if (businessUnits == null || businessUnits.Entities.Count == 0)
                return;
            var teamsToCreate = AddUserToExistingTeam(businessUnits, users);
            if (teamsToCreate == null || teamsToCreate.Count == 0) return;

            var roles = GetSecurityRolesByBusinessUnit(teamsToCreate);
            if (roles == null || roles.Entities.Count != (teamsToCreate.Count * 2))
                throw new InvalidPluginExecutionException("Security Role Tc.Ids.Base, Tc.Ids.Rep was not created for one or more Business Units.");

            AddUserToNewTeam(teamsToCreate, roles, users, hotelTeamId, teamName);

            trace.Trace("ProcessUserTeams - End");
        }

        /// <summary>
        /// To add security role, user to newly created teams
        /// </summary>
        /// <param name="buToProcess"></param>
        /// <param name="roleCollection"></param>
        /// <param name="users"></param>
        /// <param name="hotelTeamId"></param>
        /// <param name="teamName"></param>
        private void AddUserToNewTeam(Dictionary<Guid, string> buToProcess, EntityCollection roleCollection, EntityReferenceCollection users, Guid hotelTeamId, string teamName)
        {
            trace.Trace("AddUserToNewTeam - Start");
            foreach (var b in buToProcess)
            {
                var roles = FilterRolesByBusinessUnit(roleCollection, b.Key);
                if (roles != null && roles.Count == 2)
                {
                    var teamId = CreateTeam(hotelTeamId, b.Value + " : " + teamName, b.Key);
                    AssociateRoleToTeam(teamId, roles);
                    AssociateUserToTeam(teamId, users);
                }
            }
            trace.Trace("AddUserToNewTeam - End");
        }

        /// <summary>
        /// To filter roles using Business unit Id
        /// </summary>
        /// <param name="securityRoles"></param>
        /// <param name="businessUnitId"></param>
        /// <returns></returns>
        private EntityReferenceCollection FilterRolesByBusinessUnit(EntityCollection securityRoles, Guid businessUnitId)
        {
            trace.Trace("FilterRolesByBusinessUnit - Start");
            var rolesOfBusinessUnit = new EntityReferenceCollection();
            var roles = from role in securityRoles.Entities
                        where ((EntityReference)role.Attributes[Attributes.Role.BusinessUnitId]).Id == businessUnitId
                        select new EntityReference
                        {
                            LogicalName = role.LogicalName,
                            Id = role.Id
                        };

            if (roles == null || roles.Count() == 0) return rolesOfBusinessUnit;

            foreach (var role in roles)
            {
                rolesOfBusinessUnit.Add(role);
            }

            trace.Trace("FilterRolesByBusinessUnit - End");
            return rolesOfBusinessUnit;
        }

        /// <summary>
        /// To add user to existing teams of Business units
        /// </summary>
        /// <param name="buCollection"></param>
        /// <param name="users"></param>
        /// <returns></returns>
        private Dictionary<Guid, string> AddUserToExistingTeam(EntityCollection buCollection, EntityReferenceCollection users)
        {
            trace.Trace("AddUserToExistingTeam - Start");
            const string aliasTeamId = "team.teamid";
            var processedBusinessUnits = new List<Guid>();
            var teamsToCreate = new Dictionary<Guid, string>();          
           
            for (int i = 0; i < buCollection.Entities.Count; i++)
            {
                var teamId = Guid.Empty;
                var buEntity = buCollection.Entities[i];
                if (buEntity == null) continue;

                if (processedBusinessUnits.Contains(buEntity.Id))
                    continue;

                processedBusinessUnits.Add(buEntity.Id);

                if (buEntity.Attributes.Contains(aliasTeamId) && buEntity.Attributes[aliasTeamId] != null)
                {
                    teamId = Guid.Parse(((AliasedValue)buEntity.Attributes[aliasTeamId]).Value.ToString());
                }
                if (teamId == Guid.Empty)
                {
                    if (buEntity.Attributes.Contains(Attributes.BusinessUnit.Name) && buEntity.Attributes[Attributes.BusinessUnit.Name] != null)
                    {
                        var buName = buEntity.Attributes[Attributes.BusinessUnit.Name].ToString();
                        if (!teamsToCreate.Keys.Contains(buEntity.Id))
                        {
                            teamsToCreate.Add(buEntity.Id, buName);
                            trace.Trace("buToProcess " + buEntity.Id);
                        }
                    }
                    continue;
                }
                AssociateUserToTeam(teamId, users);
            }
            trace.Trace("AddUserToExistingTeam - End");

            return teamsToCreate;
        }

        /// <summary>
        /// To get Business units that are to be processed
        /// </summary>
        /// <param name="hotelTeamId"></param>
        /// <returns></returns>
        private EntityCollection GetBusinessUnits(Guid hotelTeamId)
        {
            trace.Trace("GetBusinessUnits - Start");
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
            var businessUnitCollection = service.RetrieveMultiple(fetch);
            trace.Trace("GetBusinessUnits - End");
            return businessUnitCollection;
        }

        /// <summary>
        /// To get Business unit name fetch conditions
        /// </summary>
        /// <returns></returns>
        private string GetBusinessUnitNameConditions()
        {
            if (businessUnits == null || businessUnits.Length == 0) throw new InvalidPluginExecutionException("Business unit names were not mentioned in unsecure config of AddUserToHotelTeam plugin.");

            var businessUnitNameCondition = new StringBuilder();
            for (int i = 0; i < businessUnits.Length; i++)
            {
                businessUnitNameCondition.Append(string.Format("<condition attribute='name' operator='eq' value = '{0}'/>", businessUnits[i]));
            }
            return businessUnitNameCondition.ToString();
        }

        /// <summary>
        /// To get Security roles for different Business units using security role name Tc.Ids.Base and Tc.Ids.Rep
        /// </summary>
        /// <param name="businessUnits"></param>
        /// <returns></returns>
        private EntityCollection GetSecurityRolesByBusinessUnit(Dictionary<Guid, string> businessUnits)
        {
            trace.Trace("GetSecurityRolesByBu - Start");
            var businessUnitCondition = GetBusinessUnitConditions(businessUnits);
            var query = string.Format(@"<fetch distinct='false' output-format='xml - platform' version='1.0' mapping='logical'>
                                        <entity name='role'>
                                        <attribute name='businessunitid'/>
                                        <attribute name='roleid' />
                                        <filter type='and'>
                                         <filter type='or'>
                                          <condition attribute='name' operator='eq' value='{0}'/>
                                          <condition attribute='name' operator='eq' value='{1}'/>
                                         </filter >
                                         <condition attribute='businessunitid' operator='in'>
                                          {2}
                                         </condition>
                                        </filter>
                                        </entity>
                                        </fetch> ", General.RoleTcIdBase, General.RoleTcIdRep, businessUnitCondition);
            var fetch = new FetchExpression(query);
            var roleCollection = service.RetrieveMultiple(fetch);
            trace.Trace("GetSecurityRolesByBu - End");
            return roleCollection;
        }

        /// <summary>
        /// To get fetchxml Business unit conditions
        /// </summary>
        /// <param name="businessUnits"></param>
        /// <returns></returns>
        private string GetBusinessUnitConditions(Dictionary<Guid, string> businessUnits)
        {
            var businessUnitCondition = new StringBuilder();
            foreach (var b in businessUnits)
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
            trace.Trace("CreateTeam - Start");
            var team = new Entity(Entities.Team);
            team.Attributes.Add(Attributes.Team.Name, teamName);
            team.Attributes.Add(Attributes.Team.HotelTeam, true);
            team.Attributes.Add(Attributes.Team.BusinessUnitId, new EntityReference(Entities.BusinessUnit, businessUnitId));
            team.Attributes.Add(Attributes.Team.HotelTeamId, new EntityReference(Entities.Team, hotelTeamId));
            var teamId = service.Create(team);
            trace.Trace("CreateTeam - End");
            return teamId;
        }

        /// <summary>
        /// To Asscoiate Security role to Team
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="roles"></param>
        private void AssociateRoleToTeam(Guid teamId, EntityReferenceCollection roles)
        {
            trace.Trace("AssociateRoleToTeam - Start");
            service.Associate(Entities.Team,
                                teamId,
                                new Relationship(Relationships.TeamRolesAssociation),
                                roles);
            trace.Trace("AssociateRoleToTeam - End");
        }

        /// <summary>
        /// To Associate User to Team
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="users"></param>
        private void AssociateUserToTeam(Guid teamId, EntityReferenceCollection users)
        {
            trace.Trace("AssociateUserToTeam - Start");
            service.Associate(Entities.Team,
                            teamId,
                            new Relationship(Relationships.TeamMembershipAssociation),
                            users);
            trace.Trace("AssociateUserToTeam - End");
        }
    }
}
