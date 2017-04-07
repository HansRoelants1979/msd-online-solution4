using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeXrmEasy;

namespace Tc.Crm.UnitTests.Plugins
{
    [TestClass]
    public class PostAssociateUserToTeamTest
    {

        private XrmFakedPluginExecutionContext GetPluginContext(Guid userId, Guid teamId)
        {
            var entRefcol = new EntityReferenceCollection();
            entRefcol.Add(new EntityReference("systemuser", userId));
            var cntxt = new XrmFakedPluginExecutionContext();
            cntxt.InputParameters = new ParameterCollection();
            cntxt.InputParameters.Add("Relationship", "teammembership_association.");
            cntxt.InputParameters.Add("Target", new EntityReference("team", teamId));
            cntxt.InputParameters.Add("RelatedEntities", entRefcol);            
            cntxt.MessageName = "Associate";
            cntxt.Stage = 40;

            return cntxt;
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException))]
        public void AddUserToHotelWithNoTeamInfo()
        { 
            var context = new XrmFakedContext();
            var cntxt = GetPluginContext(Guid.NewGuid(), Guid.NewGuid());
            context.ExecutePluginWithConfigurations<Crm.Plugins.User.PostAssociateUserToTeam>(cntxt, "IL,UK", "");          
        }

        [TestMethod]        
        public void CheckChildPipeLineisNotExecuting()
        {            
            var context = new XrmFakedContext();
            var teamId = Guid.NewGuid();
            var team = new Entity("team",teamId);
            team["name"] = "xx";
            team["tc_hotelteam"] = true;
            team["tc_hotelteamid"] = Guid.NewGuid();
            context.Initialize(new List<Entity> { team });
            var cntxt = GetPluginContext(Guid.NewGuid(), teamId);
            context.ExecutePluginWithConfigurations<Crm.Plugins.User.PostAssociateUserToTeam>(cntxt, "IL,UK","");
            var teams = (from t in context.CreateQuery("team")
                         where t.Id != teamId
                         select t).ToList();
            Assert.IsTrue(teams.Count == 0);
        }

        [TestMethod]       
        public void CheckCreateTeamWithNoBusinessUnits()
        {
            var context = new XrmFakedContext();
            var teamId = Guid.NewGuid();
            var team = new Entity("team", teamId);
            team["name"] = "xx";
            team["tc_hotelteam"] = true;           
            context.Initialize(new List<Entity> { team });
            var cntxt = GetPluginContext(Guid.NewGuid(), teamId);
            context.ExecutePluginWithConfigurations<Crm.Plugins.User.PostAssociateUserToTeam>(cntxt, "IL,UK", "");
            var teams = (from t in context.CreateQuery("team")
                         where t.Id != teamId
                         select t).ToList();
            Assert.IsTrue(teams.Count == 0);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException))]
        public void CheckCreateTeamWithNoSecurityRoles()
        {
            var context = new XrmFakedContext();
            var teamId = Guid.NewGuid();
            var team = new Entity("team", teamId);
            team["name"] = "xx";
            team["tc_hotelteam"] = true;
            var bu = new Entity("businessunit",Guid.NewGuid());
            bu["name"] = "IL";
            context.Initialize(new List<Entity> { team, bu });
            var cntxt = GetPluginContext(Guid.NewGuid(), teamId);
            context.ExecutePluginWithConfigurations<Crm.Plugins.User.PostAssociateUserToTeam>(cntxt, "IL", "");           
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException))]
        public void CheckCreateTeamWithOneSecurityRole()
        {
            var context = new XrmFakedContext();
            var teamId = Guid.NewGuid();
            var buId = Guid.NewGuid();
            var team = new Entity("team", teamId);
            team["name"] = "xx";
            team["tc_hotelteam"] = true;
            var bu = new Entity("businessunit", buId);
            bu["name"] = "IL";
            var role = new Entity("role", Guid.NewGuid());
            role["name"] = "Tc.Ids.Base";
            role["businessunitid"] = new EntityReference("businessunit", buId);
            context.Initialize(new List<Entity> { team, bu, role });
            var cntxt = GetPluginContext(Guid.NewGuid(),teamId);
            context.ExecutePluginWithConfigurations<Crm.Plugins.User.PostAssociateUserToTeam>(cntxt, "IL", "");
        }

        [TestMethod]       
        public void CheckCreateTeamWithTwoSecurityRoles()
        {
            var context = new XrmFakedContext();
            var teamId = Guid.NewGuid();
            var buId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var team = new Entity("team", teamId);
            team["name"] = "xx";
            team["tc_hotelteam"] = true;
            var bu = new Entity("businessunit", buId);
            bu["name"] = "IL";
            var role = new Entity("role", Guid.NewGuid());
            role["name"] = "Tc.Ids.Base";
            role["businessunitid"] = new EntityReference("businessunit", buId);
            var role1 = new Entity("role", Guid.NewGuid());
            role1["name"] = "Tc.Ids.Rep";
            role1["businessunitid"] = new EntityReference("businessunit", buId);
            var user = new Entity("systemuser", userId);
            context.Initialize(new List<Entity> { team, bu, role, role1, user });
            var cntxt = GetPluginContext(userId, teamId);
            context.AddRelationship("teamroles_association", new XrmFakedRelationship("teamid", "roleid", "team", "role"));
            context.AddRelationship("teammembership_association", new XrmFakedRelationship("teamid", "systemuserid", "team", "systemuser"));
            context.ExecutePluginWithConfigurations<Crm.Plugins.User.PostAssociateUserToTeam>(cntxt, "IL", "");
            var teams = (from t in context.CreateQuery("team")
                         where t.Id != teamId
                         select t).ToList();
            Assert.IsTrue(teams.Count == 1);
        }
    }
}
