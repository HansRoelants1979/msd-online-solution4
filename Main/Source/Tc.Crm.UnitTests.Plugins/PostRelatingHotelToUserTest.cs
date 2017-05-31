using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using FakeXrmEasy;
using Tc.Crm.Plugins;
using Tc.Crm.Plugins.Hotel;
using Attributes = Tc.Crm.Plugins.Attributes;

namespace Tc.Crm.UnitTests.Plugins
{
    [TestClass]
    public class PostRelatingHotelToUserTest
    {
        [TestMethod]
        public void TestCreateHotelNoAction()
        {
            var context = new XrmFakedContext();
            context.Initialize(new List<Entity>());
            var target = new Entity(Entities.Hotel) { Id = Guid.NewGuid() };
            target[Attributes.Hotel.Name] = "Hotel XXX";
            target[Attributes.Hotel.MasterHotelId] = "12345";
            target[Attributes.Hotel.OwningTeam] = null;
            var pluginContext = new XrmFakedPluginExecutionContext();
            pluginContext.Stage = (int)PluginStage.Prevalidation;
            pluginContext.MessageName = Messages.Create;
            pluginContext.InputParameters = new ParameterCollection();
            pluginContext.InputParameters.Add(InputParameters.Target, target);
            context.ExecutePluginWith<Crm.Plugins.Hotel.PostRelatingHotelToUser>(pluginContext);
            var team = (from t in context.CreateQuery(Entities.Team) select t).ToList();
            Assert.AreEqual(0, team.Count);
        }

        [TestMethod]
        public void TestAssosiateHotelOwnerIsUser()
        {
            var businessUnitId = Guid.Empty;
            var userId = Guid.NewGuid();
            var hotelId = Guid.NewGuid();
            var context = GetAssosiateContext(businessUnitId, userId, hotelId, false, false, true, true);
            var pluginContext = GetPluginContext(businessUnitId, userId, hotelId, Messages.Associate);
            context.ExecutePluginWithConfigurations<PostRelatingHotelToUser>(pluginContext, "GB", null);
            var team = (from t in context.CreateQuery(Entities.Team) where t.Attributes.ContainsKey("name") select t).ToList();
            var teamRoleRelation = (from t in context.CreateQuery(Entities.Team) where t.Attributes.ContainsKey("roleid") select t).ToList();
            var userRoleRelation = (from t in context.CreateQuery(Entities.Team) where t.Attributes.ContainsKey("systemuserid") select t).ToList();
            Assert.AreEqual(2, team.Count); // 2 teams created
            Assert.AreEqual(3, teamRoleRelation.Count); // 3 security role assignments created 
            Assert.AreEqual(2, userRoleRelation.Count); // 2 user to team assignments created
        }

        [TestMethod]
        public void TestAssosiateHotelOwnerIsHotelTeamNoBusinessUnitTeam()
        {
            var businessUnitId = Guid.Empty;
            var userId = Guid.NewGuid();
            var hotelId = Guid.NewGuid();
            var context = GetAssosiateContext(businessUnitId, userId, hotelId, true, false, true, true);
            var pluginContext = GetPluginContext(businessUnitId, userId, hotelId, Messages.Associate);
            context.ExecutePluginWithConfigurations<PostRelatingHotelToUser>(pluginContext, "GB", null);
            var team = (from t in context.CreateQuery(Entities.Team) where t.Attributes.ContainsKey("name") select t).ToList();
            var teamRoleRelation = (from t in context.CreateQuery(Entities.Team) where t.Attributes.ContainsKey("roleid") select t).ToList();
            var userRoleRelation = (from t in context.CreateQuery(Entities.Team) where t.Attributes.ContainsKey("systemuserid") select t).ToList();
            Assert.AreEqual(2, team.Count); // 1 team existed, 1 created
            Assert.AreEqual(2, teamRoleRelation.Count); // 2 security role assignments created
            Assert.AreEqual(2, userRoleRelation.Count); // 2 user to team assignments created
        }

        [TestMethod]
        public void TestAssosiateHotelOwnerIsHotelTeamAllTeamsCreated()
        {
            var businessUnitId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var hotelId = Guid.NewGuid();
            var context = GetAssosiateContext(businessUnitId, userId, hotelId, true, true, true, true);
            var pluginContext = GetPluginContext(businessUnitId, userId, hotelId, Messages.Associate);
            context.ExecutePluginWithConfigurations<PostRelatingHotelToUser>(pluginContext, "GB", null);
            var team = (from t in context.CreateQuery(Entities.Team) where t.Attributes.ContainsKey("name") select t).ToList();
            var teamRoleRelation = (from t in context.CreateQuery(Entities.Team) where t.Attributes.ContainsKey("roleid") select t).ToList();
            var userRoleRelation = (from t in context.CreateQuery(Entities.Team) where t.Attributes.ContainsKey("systemuserid") select t).ToList();
            Assert.AreEqual(2, team.Count); // 2 teams existed
            Assert.AreEqual(0, teamRoleRelation.Count); // no new security role assignments created
            Assert.AreEqual(2, userRoleRelation.Count); // 2 user to team assignments created
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException))]
        public void TestAssosiateWithNoSecurityRole()
        {
            var businessUnitId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var hotelId = Guid.NewGuid();
            var context = GetAssosiateContext(businessUnitId, userId, hotelId, false, false, false, false);
            var pluginContext = GetPluginContext(businessUnitId, userId, hotelId, Messages.Associate);
            context.ExecutePluginWithConfigurations<PostRelatingHotelToUser>(pluginContext, "GB", null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException))]
        public void TestAssosiateWithNoBuSecurityRole()
        {
            var businessUnitId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var hotelId = Guid.NewGuid();
            var context = GetAssosiateContext(businessUnitId, userId, hotelId, false, false, true, false);
            var pluginContext = GetPluginContext(businessUnitId, userId, hotelId, Messages.Associate);
            context.ExecutePluginWithConfigurations<PostRelatingHotelToUser>(pluginContext, "GB", null);
        }

        [TestMethod]
        public void TestDisassosiate()
        {
            var businessUnitId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var hotelId = Guid.NewGuid();
            var context = GetDisassosiateContext(businessUnitId, userId, hotelId);
            var pluginContext = GetPluginContext(businessUnitId, userId, hotelId, Messages.Disassociate);
            context.ExecutePluginWith<PostRelatingHotelToUser>(pluginContext);
        }

        private static XrmFakedContext GetAssosiateContext(Guid businessUnitId, Guid userId, Guid hotelId, bool createOwningTeam = false, bool createBusinessUnitTeam = false, bool createSecurityRole = true, bool createBuSecurityRole = true)
        {
            var entities = new List<Entity>();
            // role
            if (createSecurityRole)
            {
                var baseRole = new Entity(Entities.Role, Guid.NewGuid());
                baseRole[Attributes.Role.Name] = General.RoleTcIdBase;
                baseRole[Attributes.Role.BusinessUnitId] = businessUnitId;
                entities.Add(baseRole);
            }
            // business unit
            var businessUnit = new Entity(Entities.BusinessUnit, Guid.NewGuid());
            businessUnit[Attributes.BusinessUnit.Name] = "GB";
            entities.Add(businessUnit);
            // roles for unit
            if (createBuSecurityRole)
            {
                var role = new Entity(Entities.Role, Guid.NewGuid());
                role[Attributes.Role.Name] = General.RoleTcIdBase;
                role[Attributes.Role.BusinessUnitId] = businessUnit.Id;
                entities.Add(role);
                role = new Entity(Entities.Role, Guid.NewGuid());
                role[Attributes.Role.Name] = General.RoleTcIdRep;
                role[Attributes.Role.BusinessUnitId] = businessUnit.Id;
                entities.Add(role);
            }
            // hotel
            var hotel = new Entity(Entities.Hotel) { Id = hotelId };
            hotel[Attributes.Hotel.Name] = "Hotel XXX";
            hotel[Attributes.Hotel.MasterHotelId] = "1002";
            hotel[Attributes.Hotel.OwningTeam] = null;
            entities.Add(hotel);
            if (createOwningTeam)
            {
                var team = new Entity(Entities.Team) { Id = Guid.NewGuid() };
                team[Attributes.Team.Name] = "Hotel Team: Hotel XXX - 1002";
                team[Attributes.Team.HotelTeam] = true;
                hotel[Attributes.Hotel.OwningTeam] = new EntityReference(Entities.Team, team.Id);
                entities.Add(team);
                if (createBusinessUnitTeam)
                {
                    var buTeam = new Entity(Entities.Team) { Id = Guid.NewGuid() };
                    buTeam[Attributes.Team.Name] = "GB: Hotel Team: Hotel XXX - 1002";
                    buTeam[Attributes.Team.HotelTeam] = true;
                    buTeam[Attributes.Team.BusinessUnitId] = new EntityReference(Entities.BusinessUnit, businessUnit.Id);
                    entities.Add(buTeam);
                }
            }
            // user
            var user = new Entity(Entities.User) { Id = userId };
            user["businessunit"] = businessUnitId;
            entities.Add(user);
            var context = new XrmFakedContext();
            context.Initialize(entities);
            var relationship = new XrmFakedRelationship(Entities.Team, "teamid", "systemuserid", "team", "systemuser");
            context.AddRelationship(Relationships.TeamMembershipAssociation, relationship);
            relationship = new XrmFakedRelationship(Entities.Team, "teamid", "roleid", "team", "role");
            context.AddRelationship(Relationships.TeamRolesAssociation, relationship);
            return context;
        }

        private static XrmFakedContext GetDisassosiateContext(Guid businessUnitId, Guid userId, Guid hotelId)
        {
            // role
            var baseRole = new Entity(Entities.Team, Guid.NewGuid());
            baseRole["name"] = General.RoleTcIdBase;
            baseRole["businessunitid"] = businessUnitId;
            // team
            var team = new Entity(Entities.Team) { Id = Guid.NewGuid()};
            team[Attributes.Team.HotelTeam] = true;
            // hotel
            var hotel = new Entity(Entities.Hotel) { Id = hotelId };
            hotel[Attributes.Hotel.OwningTeam] = new EntityReference(Entities.Team, team.Id);
            // user
            var user = new Entity(Entities.User) { Id = userId };
            user["businessunit"] = businessUnitId;
            var context = new XrmFakedContext();
            context.Initialize(new List<Entity>() {
            baseRole, hotel, user, team
            });
            var relationship = new XrmFakedRelationship(Entities.Team, "teamid", "systemuserid", "team", "systemuser");
            context.AddRelationship(Relationships.TeamMembershipAssociation, relationship);
            return context;
        }

        private static XrmFakedPluginExecutionContext GetPluginContext(Guid businessUnitId, Guid userId, Guid hotelId, string message)
        {
            var pluginContext = new XrmFakedPluginExecutionContext();
            pluginContext.Stage = (int)PluginStage.Postoperation;
            pluginContext.BusinessUnitId = businessUnitId;
            pluginContext.MessageName = message;
            pluginContext.InputParameters = new ParameterCollection();
            var relationship = new Relationship(Relationships.UserHotels);
            pluginContext.InputParameters.Add(InputParameters.Relationship, relationship);
            var target = new EntityReference(Entities.User, userId);
            pluginContext.InputParameters.Add(InputParameters.Target, target);
            var relatedEntities = new EntityReferenceCollection() { new EntityReference(Entities.Hotel, hotelId) };
            pluginContext.InputParameters.Add(InputParameters.RelatedEntities, relatedEntities);

            return pluginContext;
        }
    }
}
