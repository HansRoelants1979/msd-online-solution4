using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Tc.Crm.Plugins.User.BusinessLogic
{
    public class DeassociateUserFromTeamService
    {
        public IPluginExecutionContext context = null;
        public IOrganizationService service = null;
        public ITracingService trace = null;


        public DeassociateUserFromTeamService(IPluginExecutionContext context, ITracingService trace, IOrganizationService service)
        {
            this.context = context;
            this.service = service;
            this.trace = trace;
        }

        private bool IsContextValid()
        {
            if (!context.MessageName.Equals(Messages.Disassociate, StringComparison.OrdinalIgnoreCase)) return false;
            if (context.Stage != 40) return false;
            if (!context.InputParameters.Contains(InputParameters.Relationship))
                return false;
            var relationshipName = context.InputParameters[InputParameters.Relationship].ToString();
            if (relationshipName != Relationships.TeamMembershipAssociation + ".")
                return false;
            return true;
        }

        public void DoActionsOnUserDisassociate()
        {
            if (!IsContextValid()) return;

            var hotelTeamId = GetHotelTeamId();
            var users = GetUsersFromContext();

            if (hotelTeamId == Guid.Empty || users == null || users.Count == 0)
                return;

            if (!IsParentHotelTeam(hotelTeamId))
                return;

            var childTeams = GetChildTeams(hotelTeamId);

            if (childTeams == null || childTeams.Entities.Count == 0)
                return;

            RemoveUserFromTeam(childTeams, users);
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
        /// <returns></returns>
        private bool IsParentHotelTeam(Guid teamId)
        {
            trace.Trace("CheckHotelTeam - start");
            var hotelTeam = false;
            var team = service.Retrieve(Entities.Team, teamId, new ColumnSet(Attributes.Team.HotelTeam, Attributes.Team.HotelTeamId));
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
        /// To get child teams of hotel team
        /// </summary>
        /// <param name="hotelTeamId"></param>
        /// <returns></returns>
        private EntityCollection GetChildTeams(Guid hotelTeamId)
        {
            trace.Trace("GetChildTeams - Start");
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
            trace.Trace("GetChildTeams - End");
            return teamCollection;
        }


        /// <summary>
        /// To Disassociate users from Hotel Teams
        /// </summary>
        /// <param name="childTeams"></param>
        /// <param name="users"></param>
        private void RemoveUserFromTeam(EntityCollection childTeams, EntityReferenceCollection users)
        {
            for (int i = 0; i < childTeams.Entities.Count; i++)
            {
                var childTeam = childTeams[i];
                if (childTeam == null) continue;
                if (childTeam.Attributes.Contains(Attributes.Team.TeamId) && childTeam.Attributes[Attributes.Team.TeamId] != null)
                    DisassociateUserFromTeam(childTeam.Id, users);
            }
        }


        /// <summary>
        /// To Disassociate users from Hotel Team
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="users"></param>
        private void DisassociateUserFromTeam(Guid teamId, EntityReferenceCollection users)
        {
            trace.Trace("DisassociateUserFromTeam - Start");
            service.Disassociate(Entities.Team,
                            teamId,
                            new Relationship(Relationships.TeamMembershipAssociation),
                            users);
            trace.Trace("DisassociateUserFromTeam - End");
        }
    }
}
