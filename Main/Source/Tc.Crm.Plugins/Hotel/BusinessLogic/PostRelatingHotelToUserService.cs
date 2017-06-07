using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tc.Crm.Plugins.Hotel.BusinessLogic
{
    public class PostRelatingHotelToUserService
    {
        private IPluginExecutionContext context = null;
        private IOrganizationService service = null;
        private ITracingService trace = null;
        private string[] businessUnitsNames;

        public PostRelatingHotelToUserService(IPluginExecutionContext context, ITracingService trace, IOrganizationService service, string[] businessUnits)
        {
            this.context = context;
            this.service = service;
            this.trace = trace;
            this.businessUnitsNames = businessUnits;
        }

        #region Public Implementation

        /// <summary>
        /// Creating a team and setting this created team as owner to Hotel
        /// </summary>
        public void AddUserToHotelTeam()
        {
            trace.Trace("Begin - AddUserToHotelTeam");
            // get user and hotel
            var userReference = (EntityReference)context.InputParameters[InputParameters.Target];
            var relatedEntities = context.InputParameters[InputParameters.RelatedEntities] as EntityReferenceCollection;
            var hotelReference = relatedEntities[0];
            var hotel = service.Retrieve(Entities.Hotel, hotelReference.Id, new ColumnSet(new string[] { Attributes.Hotel.Name, Attributes.Hotel.MasterHotelId, Attributes.Hotel.OwningTeam }));
            trace.Trace("Adding user to teams of hotel: {0}", hotel.GetAttributeValue<string>(Attributes.Hotel.Name));
            // Check hotel owner. Create team if owner is user
            Guid teamId;
            string teamName = GetHotelTeamName(hotel);            
            var owningTeam = hotel.GetAttributeValue<EntityReference>(Attributes.Hotel.OwningTeam);            
            if (owningTeam != null && IsParentHotelTeam(owningTeam.Id))
            {
                trace.Trace("Hotel: {0} already assigned to a hotel team", hotel.GetAttributeValue<string>(Attributes.Hotel.Name));
                teamId = owningTeam.Id;
            }
            else
            {
                teamId = CreateTeam(context.BusinessUnitId, teamName);
                AssociateBaseSecurityRole(teamId);
                trace.Trace("Setting hotel owner to team");
                SetHotelOwner(hotel, teamId);
            }
            AssociateUserToTeam(teamId, userReference);
            ProcessBusinessUnitUserTeams(teamId, userReference, teamName);
            trace.Trace("End - AddUserToHotelTeam");
        }

        /// <summary>
        /// Remove user from hotel teams: main hotel team and all teams of business units
        /// </summary>
        public void RemoveUserFromHotelTeams()
        {
            trace.Trace("Begin - RemoveUserFromHotelTeams");            
            var targetReference = (EntityReference)context.InputParameters[InputParameters.Target];            
            var relatedEntities = context.InputParameters[InputParameters.RelatedEntities] as EntityReferenceCollection;
            // get user and hotel
            var isUserTarget = string.Equals(Entities.User, targetReference.LogicalName, StringComparison.InvariantCultureIgnoreCase);
            var userReference = isUserTarget ? targetReference : relatedEntities[0];
            var hotelReference = isUserTarget ? relatedEntities[0] : targetReference;
            var hotel = service.Retrieve(Entities.Hotel, hotelReference.Id, new ColumnSet(new string[] { Attributes.Hotel.Name, Attributes.Hotel.MasterHotelId, Attributes.Hotel.OwningTeam }));
            trace.Trace("Removing user from teams of hotel: {0}", hotel.GetAttributeValue<string>(Attributes.Hotel.Name));
            var owningTeam = hotel.GetAttributeValue<EntityReference>(Attributes.Hotel.OwningTeam);
            if (owningTeam == null || !IsParentHotelTeam(owningTeam.Id))
            {
                trace.Trace("Hotel: {0} is not assigned to a hotel team", hotel.GetAttributeValue<string>(Attributes.Hotel.Name));
                return;
            }
            // remove from hotel team
            DisassociateUserFromTeam(owningTeam.Id, userReference);
            // remove from business unit hotel teams
            var childTeams = GetChildTeams(owningTeam.Id);
            if (childTeams == null || childTeams.Entities.Count == 0)
                return;
            RemoveUserFromTeams(childTeams, userReference);

            trace.Trace("End - RemoveUserFromHotelTeams");
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// To check whether Team is parent hotel team
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        private bool IsParentHotelTeam(Guid teamId)
        {
            var isHotelTeam = false;
            var team = service.Retrieve(Entities.Team, teamId, new ColumnSet(Attributes.Team.HotelTeam, Attributes.Team.HotelTeamId));
            if (team == null) return isHotelTeam;
            // check if no parent hotel team id for team and flag HotelTeam is true
            if (team.Attributes.Contains(Attributes.Team.HotelTeam) && team.Attributes[Attributes.Team.HotelTeam] != null &&
            (!team.Attributes.Contains(Attributes.Team.HotelTeamId) || team.Attributes[Attributes.Team.HotelTeamId] == null))
            {
                isHotelTeam = bool.Parse(team.Attributes[Attributes.Team.HotelTeam].ToString());
                return isHotelTeam;
            }
            return isHotelTeam;
        }

        // check if user is assigned to team
        private bool IsUserAssignedToTeam(Guid teamId, EntityReference user)
        {
            var query = string.Format(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                                <entity name='team'>
                                    <attribute name='teamid' />
                                    <filter type='and'>
                                        <condition attribute='teamid' operator='eq' uitype='team' value='{0}' />
                                    </filter>
                                    <link-entity name='teammembership' from='teamid' to='teamid' visible='false' intersect='true'>
                                        <link-entity name='systemuser' from='systemuserid' to='systemuserid' alias='ab'>
                                            <filter type='and'>
                                                <condition attribute='systemuserid' operator='eq' value='{1}'/>
                                            </filter>
                                        </link-entity>
                                    </link-entity>
                                </entity>
                            </fetch>", teamId, user.Id);
            var fetch = new FetchExpression(query);
            var relation = service.RetrieveMultiple(fetch);
            return relation != null && relation.Entities.Count > 0;
        }

        #region Disassociation

        /// <summary>
        /// To get child teams of hotel team
        /// </summary>
        /// <param name="hotelTeamId"></param>
        /// <returns></returns>
        private EntityCollection GetChildTeams(Guid hotelTeamId)
        {
            var query = string.Format(@"<fetch distinct='false' output-format='xml-platform' version='1.0' mapping='logical'>
                                        <entity name='team'>
                                        <attribute name='teamid'/>
                                        <filter type='and'>
                                         <condition attribute='tc_hotelteamid' operator='eq' value='{0}' />
                                        </filter>
                                        </entity>
                                        </fetch>", new object[] { hotelTeamId });
            var fetch = new FetchExpression(query);
            var teamCollection = service.RetrieveMultiple(fetch);
            trace.Trace("Retrieved child teams of hotel team");
            return teamCollection;
        }

        /// <summary>
        /// To Disassociate users from Hotel Teams
        /// </summary>
        /// <param name="childTeams"></param>
        /// <param name="users"></param>
        private void RemoveUserFromTeams(EntityCollection teams, EntityReference user)
        {
            for (int i = 0; i < teams.Entities.Count; i++)
            {
                var childTeam = teams[i];
                if (childTeam == null) continue;
                if (childTeam.Attributes.Contains(Attributes.Team.TeamId) && childTeam.Attributes[Attributes.Team.TeamId] != null)
                    DisassociateUserFromTeam(childTeam.Id, user);
            }
        }

        /// <summary>
        /// To Disassociate users from Hotel Team
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="users"></param>
        private void DisassociateUserFromTeam(Guid teamId, EntityReference user)
        {
            if (IsUserAssignedToTeam(teamId, user))
            {
                service.Disassociate(Entities.Team,
                                teamId,
                                new Relationship(Relationships.TeamMembershipAssociation),
                                new EntityReferenceCollection { user });
                trace.Trace("Disassociated user from team");
            }
            else
            {
                trace.Trace("User is not associated with team");
            }
        }

        #endregion

        #region Association

        /// <summary>
        /// Set hotel owner to created team
        /// </summary>
        /// <param name="hotel"></param>
        /// <param name="teamId"></param>
        private void SetHotelOwner(Entity hotel, Guid teamId)
        {
            hotel.Attributes[Attributes.Hotel.Owner] = new EntityReference(Entities.Team, teamId);
            service.Update(hotel);
            trace.Trace("Set hotel owner to team.");
        }

        private void ProcessBusinessUnitUserTeams(Guid hotelTeamId, EntityReference user, string teamName)
        {
            var businessUnits = GetBusinessUnits(hotelTeamId);
            if (businessUnits == null || businessUnits.Entities.Count == 0)
                return;
            const string aliasTeamId = "team.teamid";
            var processedBusinessUnits = new List<Guid>();
            for (int i = 0; i < businessUnits.Entities.Count; i++)
            {
                var teamId = Guid.Empty;
                var businessUnit = businessUnits.Entities[i];
                if (businessUnit == null) continue;
                if (processedBusinessUnits.Contains(businessUnit.Id)) continue;
                processedBusinessUnits.Add(businessUnit.Id);

                var businessUnitName = businessUnit.Attributes.Contains(Attributes.BusinessUnit.Name) && businessUnit.Attributes[Attributes.BusinessUnit.Name] != null ? businessUnit.Attributes[Attributes.BusinessUnit.Name].ToString() : null;
                
                if (businessUnit.Attributes.Contains(aliasTeamId) && businessUnit.Attributes[aliasTeamId] != null)
                {
                    teamId = Guid.Parse(((AliasedValue)businessUnit.Attributes[aliasTeamId]).Value.ToString());
                    trace.Trace("Business unit {0} already have team.", businessUnitName);
                }
                if (teamId == Guid.Empty)
                {
                    if (!string.IsNullOrEmpty(businessUnitName))
                    {
                        teamId = CreateTeam(businessUnit.Id, businessUnitName + " : " + teamName, hotelTeamId);
                        AssociateBusinessUnitSecurityRole(teamId, businessUnit.Id);
                    }
                }
                AssociateUserToTeam(teamId, user);
            }
        }

        /// <summary>
        /// Add user to team if not added already
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="user"></param>
        private bool AssociateUserToTeam(Guid teamId, EntityReference user)
        {
            if (!IsUserAssignedToTeam(teamId, user))
            {
                service.Associate(
                    Entities.Team,
                    teamId,
                    new Relationship(Relationships.TeamMembershipAssociation),
                    new EntityReferenceCollection { user });
                trace.Trace("Assigned user to team");
                return true;
            }
            else
            {
                trace.Trace("User is already assigned to team");
                return false;
            }
        }

        /// <summary>
        /// To create team
        /// </summary>
        /// <param name="hotel"></param>
        /// <returns></returns>
        private Guid CreateTeam(Guid businessUnit, string teamName, Guid? parentHotelTeam = null)
        {
            trace.Trace("Creating Team with Name: {0}", teamName);
            Entity team = new Entity(Entities.Team);
            team.Attributes.Add(Attributes.Team.Name, teamName);
            team.Attributes.Add(Attributes.Team.HotelTeam, true);
            team.Attributes.Add(Attributes.Team.BusinessUnitId, new EntityReference(Entities.BusinessUnit, businessUnit));
            if (parentHotelTeam.HasValue)
            {
                team.Attributes.Add(Attributes.Team.HotelTeamId, new EntityReference(Entities.Team, parentHotelTeam.Value));
            }
            var teamId = service.Create(team);
            if (teamId == Guid.Empty)
                throw new InvalidPluginExecutionException("Unable to create team.");
            trace.Trace("Created Team with Id: {0}", teamId);
            return teamId;
        }

        private static string GetHotelTeamName(Entity hotel)
        {
            string hotelName = hotel.GetAttributeValue<string>(Attributes.Hotel.Name);
            string masterHotelId = hotel.GetAttributeValue<string>(Attributes.Hotel.MasterHotelId);

            if (string.IsNullOrEmpty(hotelName) || string.IsNullOrEmpty(masterHotelId))
                throw new InvalidPluginExecutionException("The hotel name and hotel master Id must both be populated");

            return $"Hotel Team: {hotelName} - {masterHotelId}";
        }

        /// <summary>
        /// To asscoiate security role Tc.Ids.Base to the team created through plugin
        /// </summary>
        /// <param name="teamId"></param>
        private void AssociateBaseSecurityRole(Guid teamId)
        {
            var roles = GetBaseSecurityRole();
            if (roles == null || roles.Entities.Count != 1)
                throw new InvalidPluginExecutionException("The role Tc.Ids.Base does not exist in the required business unit");
            AssociateRolesToTeam(teamId, new EntityReferenceCollection() { new EntityReference(Entities.Role, roles.Entities[0].Id) });
            trace.Trace("Associated security role {0} with team", General.RoleTcIdBase);
        }

        private void AssociateBusinessUnitSecurityRole(Guid teamId, Guid businessUnitId)
        {
            var roles = GetSecurityRolesByBusinessUnit(businessUnitId);
            AssociateRolesToTeam(teamId, roles);
            trace.Trace("Associated roles to business unit team.");
        }

        /// <summary>
        /// To Asscoiate Security role to Team
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="roles"></param>
        private void AssociateRolesToTeam(Guid teamId, EntityReferenceCollection roles)
        {
            service.Associate(Entities.Team,
                                teamId,
                                new Relationship(Relationships.TeamRolesAssociation),
                                roles);
        }

        /// <summary>
        /// To get Security role Tc.Ids.Base of this Logged in user Business unit
        /// </summary>
        /// <returns></returns>
        private EntityCollection GetBaseSecurityRole()
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
                                    new ConditionExpression(Attributes.Role.Name, ConditionOperator.Equal, General.RoleTcIdBase)
                                },
                            },
                        }
                    }
            };
            return service.RetrieveMultiple(queryForSecurityRole);
        }

        /// <summary>
        /// To get Security roles for different Business units using security role name Tc.Ids.Base and Tc.Ids.Rep
        /// </summary>
        /// <param name="businessUnits"></param>
        /// <returns></returns>
        private EntityReferenceCollection GetSecurityRolesByBusinessUnit(Guid businessUnitsId)
        {
            var query = string.Format(@"<fetch distinct='false' output-format='xml-platform' version='1.0' mapping='logical'>
                                        <entity name='role'>
                                        <attribute name='businessunitid'/>
                                        <attribute name='roleid' />
                                        <filter type='and'>
                                         <filter type='or'>
                                          <condition attribute='name' operator='eq' value='{0}'/>
                                          <condition attribute='name' operator='eq' value='{1}'/>
                                         </filter >
                                         <condition attribute='businessunitid' operator='eq' value='{2}'>
                                          {2}
                                         </condition>
                                        </filter>
                                        </entity>
                                        </fetch> ", General.RoleTcIdBase, General.RoleTcIdRep, businessUnitsId);
            var fetch = new FetchExpression(query);
            var roleCollection = service.RetrieveMultiple(fetch);
            if (roleCollection == null || roleCollection.Entities.Count != 2)
                throw new InvalidPluginExecutionException("Security Role Tc.Ids.Base, Tc.Ids.Rep was not created for one or more Business Units.");
            trace.Trace("Retrieved security roles for business unit");
            var roles = roleCollection.Entities.Select(r => new EntityReference { LogicalName = r.LogicalName, Id = r.Id }).ToList();
            return new EntityReferenceCollection(roles);
        }

        private EntityCollection GetBusinessUnits(Guid hotelTeamId)
        {
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
            return businessUnitCollection;
        }

        private string GetBusinessUnitNameConditions()
        {
            if (businessUnitsNames == null || businessUnitsNames.Length == 0) throw new InvalidPluginExecutionException("Business unit names were not mentioned in unsecure config of plugin.");

            var businessUnitNameCondition = new StringBuilder();
            for (int i = 0; i < businessUnitsNames.Length; i++)
            {
                businessUnitNameCondition.Append(string.Format("<condition attribute='name' operator='eq' value = '{0}'/>", businessUnitsNames[i]));
            }
            return businessUnitNameCondition.ToString();
        }

        #endregion

        #endregion
    }
}
