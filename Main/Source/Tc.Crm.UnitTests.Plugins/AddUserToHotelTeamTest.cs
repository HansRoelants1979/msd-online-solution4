using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeXrmEasy;

namespace Tc.Crm.UnitTests.Plugins
{
    [TestClass]
    public class AddUserToHotelTeamTest
    {

        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException))]
        public void AddUserToHotelTeam()
        { 
            var context = new XrmFakedContext();
            var cntxt = new XrmFakedPluginExecutionContext();           
            var entRefcol = new EntityReferenceCollection();           
            entRefcol.Add(new EntityReference("systemuser",Guid.NewGuid()));
            cntxt.InputParameters = new ParameterCollection();
            cntxt.InputParameters.Add("Relationship", "teammembership_association.");
            cntxt.InputParameters.Add("Target", new EntityReference("team", Guid.NewGuid()));
            cntxt.InputParameters.Add("RelatedEntities", entRefcol);
            cntxt.MessageName = "Associate";
            //context.ExecutePluginWithConfigurations<Crm.Plugins.AddUserToHotelTeam>(cntxt, "IL,UK", "");          
        }
    }
}
