using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Tc.Crm.Plugins
{
    public class ProcessRemoveUserFromHotelTeam
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="context"></param>
        /// <param name="tracingService"></param>
        /// <param name="businessUnitNames"></param>
        public ProcessRemoveUserFromHotelTeam(IOrganizationService service, IPluginExecutionContext context, ITracingService tracingService)
        {
            this.Service = service;
            this.Context = context;
            this.TracingService = tracingService;            
        }

        /// <summary>
        /// To Disasociate user from all the child hotel teams
        /// </summary>
        public void ProcessDisassociateUserFromHotelTeam()
        {
            var hotelTeamId = Guid.Empty;
            var users = GetInformationFromContext(ref hotelTeamId);

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
        /// <param name="teamId"></param>
        /// <returns></returns>
        private bool IsParentHotelTeam(Guid teamId)
        {
            TracingService.Trace("CheckHotelTeam - start");
            var hotelTeam = false;
            var team = Service.Retrieve(Entities.Team, teamId, new ColumnSet(Attributes.Team.HotelTeam, Attributes.Team.HotelTeamId));
            if (team != null)
            {
                if (team.Attributes.Contains(Attributes.Team.HotelTeam) && team.Attributes[Attributes.Team.HotelTeam] != null)
                {
                    if (!team.Attributes.Contains(Attributes.Team.HotelTeamId) || team.Attributes[Attributes.Team.HotelTeamId] == null)
                    {
                        hotelTeam = bool.Parse(team.Attributes[Attributes.Team.HotelTeam].ToString());
                        TracingService.Trace("HotelTeam is " + hotelTeam);
                    }   
                }
            }
            TracingService.Trace("CheckHotelTeam - end");
            return hotelTeam;
        }

        /// <summary>
        /// To get child teams of hotel team
        /// </summary>
        /// <param name="hotelTeamId"></param>
        /// <returns></returns>
        private EntityCollection GetChildTeams(Guid hotelTeamId)
        {
            TracingService.Trace("GetChildTeams - Start");           
            var query = string.Format(@"<fetch distinct='false' output-format='xml-platform' version='1.0' mapping='logical'>
                                        <entity name='team'>
                                        <attribute name='teamid'/>
                                        <filter type='and'>
                                         <condition attribute='tc_hotelteamid' operator='eq' value='{0}' />
                                        </filter>
                                        </entity>
                                        </fetch>", new object[] { hotelTeamId });
            var fetch = new FetchExpression(query);
            var teamCollection = Service.RetrieveMultiple(fetch);
            TracingService.Trace("GetChildTeams - End");
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
                if (childTeam != null)
                {
                    if (childTeam.Attributes.Contains(Attributes.Team.TeamId) && childTeam.Attributes[Attributes.Team.TeamId] != null)
                    {
                        DisassociateUserFromTeam(childTeam.Id, users);
                    }
                }
            }
        }


        /// <summary>
        /// To Disassociate users from Hotel Team
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="users"></param>
        private void DisassociateUserFromTeam(Guid teamId, EntityReferenceCollection users)
        {
            TracingService.Trace("DisassociateUserFromTeam - Start");
            Service.Disassociate(Entities.Team,
                            teamId,
                            new Relationship(Relationships.TeamMembershipAssociation),
                            users);
            TracingService.Trace("DisassociateUserFromTeam - End");
        }

        private IPluginExecutionContext Context { get; set; }
        private ITracingService TracingService { get; set; }
        private IOrganizationService Service { get; set; }
    }
}
