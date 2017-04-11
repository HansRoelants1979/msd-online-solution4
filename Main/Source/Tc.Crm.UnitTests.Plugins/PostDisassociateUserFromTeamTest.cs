using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeXrmEasy;

namespace Tc.Crm.UnitTests.Plugins
{
    [TestClass]
    public class PostDisassociateUserFromTeamTest
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
            cntxt.MessageName = "Disassociate";
            cntxt.Stage = 40;

            return cntxt;
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException))]
        public void RemoveUserFromHotelWithNoTeamInfo()
        {
            var context = new XrmFakedContext();
            var cntxt = GetPluginContext(Guid.NewGuid(), Guid.NewGuid());
            context.ExecutePluginWith<Crm.Plugins.User.PostDisassociateUserFromTeam>(cntxt);
        }

        [TestMethod]
        public void CheckChildPipeLineisNotExecuting()
        {
            var context = new XrmFakedContext();
            var teamId = Guid.NewGuid();
            var team = new Entity("team", teamId);
            team["name"] = "xx";
            team["tc_hotelteam"] = true;
            team["tc_hotelteamid"] = Guid.NewGuid();
            context.Initialize(new List<Entity> { team });
            var cntxt = GetPluginContext(Guid.NewGuid(), teamId);
            context.ExecutePluginWith<Crm.Plugins.User.PostDisassociateUserFromTeam>(cntxt);           
        }

        
    }
}
