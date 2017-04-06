using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;


namespace Tc.Crm.Plugins.Hotel.BusinessLogic
{
    public class HotelOwnerService
    {
        public IPluginExecutionContext context = null;
        public IOrganizationService service = null;
        public ITracingService trace = null;

        public HotelOwnerService(IPluginExecutionContext context, ITracingService trace, IOrganizationService service)
        {
            this.context = context;
            this.service = service;
            this.trace = trace;
        }

        private bool IsContextValid()
        {
            if (!context.MessageName.Equals("create", StringComparison.OrdinalIgnoreCase)) return false;
            if (context.Stage != 40) return false;
            if (!context.InputParameters.Contains(InputParameters.Target)
                || !(context.InputParameters[InputParameters.Target] is Entity))
                return false;
            return true;
        }

        /// <summary>
        /// Creating a team and setting this created team as owner to Hotel
        /// </summary>
        public void AddOwner()
        {
            if (!IsContextValid()) return;

            var targetHotel = context.InputParameters["Target"] as Entity;
            var teamId = CreateTeam(targetHotel);
            if (teamId == Guid.Empty)
                throw new InvalidPluginExecutionException("Unable to create team.");

            trace.Trace("Output - Created Team with Id: {0}", teamId);
            AssociateSecurityRole(teamId);
            AssignHotelToTeam(teamId, targetHotel.Id);
            trace.Trace("Output - Update owner of hotel");

        }

        /// <summary>
        /// To create team
        /// </summary>
        /// <param name="hotel"></param>
        /// <returns></returns>
        private Guid CreateTeam(Entity hotel)
        {
            string hotelName = hotel.GetAttributeValue<string>("tc_name");
            string masterHotelId = hotel.GetAttributeValue<string>("tc_masterhotelid");

            if (string.IsNullOrEmpty(hotelName) || string.IsNullOrEmpty(masterHotelId))
                throw new InvalidPluginExecutionException("The hotel name and hotel master Id must both be populated");

            string teamName = $"Hotel Team: {hotelName} - {masterHotelId}";

            trace.Trace("Output - Creating Team with Name: {0}", teamName);
            Entity team = new Entity(Entities.Team);
            team.Attributes.Add(Attributes.Team.Name, teamName);
            team.Attributes.Add(Attributes.Team.HotelTeam, true);
            team.Attributes.Add(Attributes.Team.BusinessUnitId, new EntityReference(Entities.BusinessUnit,
                                                                                    context.BusinessUnitId));
            return service.Create(team);
        }

        /// <summary>
        /// To asscoiate security role to the team created through plugin
        /// </summary>
        /// <param name="teamId"></param>
        private void AssociateSecurityRole(Guid teamId)
        {
            var roles = GetBaseSecurityRole();
            trace.Trace("Output - Query for security roles");
            if (roles == null || roles.Entities.Count != 1)
                throw new InvalidPluginExecutionException("The role Tc.Ids.Base does not exist in the required business unit");

            trace.Trace("Output - One security role with found with matching name and BU");
            service.Associate(
                Entities.Team,
                teamId,
                new Relationship(Relationships.TeamRolesAssociation),
                new EntityReferenceCollection()
                {
                            new EntityReference(Entities.Role, roles.Entities[0].Id)
                });
            trace.Trace("Output - Associated security role with team");


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
        /// To assign hotel to team created through plugin
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="hotelId"></param>
        private void AssignHotelToTeam(Guid teamId, Guid hotelId)
        {
            var assignRequest = new AssignRequest
            {
                Target = new EntityReference(Entities.Hotel, hotelId),
                Assignee = new EntityReference(Entities.Team, teamId)
            };
            service.Execute(assignRequest);
        }
    }
}
