using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using FakeXrmEasy;

namespace Tc.Crm.UnitTests.Plugins
{
    [TestClass]
    public class PrevalidationCreateHotelTest
    {
        [TestMethod]
        public void CreateHotelNoName()
        {
            var context = new XrmFakedContext();                     
            var target = new Microsoft.Xrm.Sdk.Entity("tc_hotel") { Id = Guid.NewGuid() };           
            var pluginContext = new XrmFakedPluginExecutionContext();
            pluginContext.Stage = 10;
            pluginContext.MessageName = "create";
            pluginContext.InputParameters = new ParameterCollection();
            pluginContext.InputParameters.Add("Target", target);
            Assert.ThrowsException<InvalidPluginExecutionException>(() => context.ExecutePluginWith<Crm.Plugins.Hotel.PrevalidationCreateHotel>(pluginContext));
        }

        [TestMethod]
        public void CreateHotelNoMasterId()
        {
            var context = new XrmFakedContext();           
            var target = new Microsoft.Xrm.Sdk.Entity("tc_hotel") { Id = Guid.NewGuid() };
            target["tc_name"] = "Hotel XXX";
            var pluginContext = new XrmFakedPluginExecutionContext();
            pluginContext.Stage = 10;
            pluginContext.MessageName = "create";
            pluginContext.InputParameters = new ParameterCollection();
            pluginContext.InputParameters.Add("Target", target);
            Assert.ThrowsException<InvalidPluginExecutionException>(() => context.ExecutePluginWith<Crm.Plugins.Hotel.PrevalidationCreateHotel>(pluginContext));
        }

        [TestMethod]
        public void CreateHotelNoRoles()
        {
            var role = new Entity("role", Guid.NewGuid());
            var context = new XrmFakedContext();
            context.Initialize(new List<Entity> { role });          
            var target = new Microsoft.Xrm.Sdk.Entity("tc_hotel") { Id = Guid.NewGuid() };
            target["tc_name"] = "Hotel XXX";
            target["tc_masterhotelid"] = "12345";
            var pluginContext = new XrmFakedPluginExecutionContext();
            pluginContext.Stage = 10;
            pluginContext.MessageName = "create";
            pluginContext.InputParameters = new ParameterCollection();
            pluginContext.InputParameters.Add("Target", target);
            Assert.ThrowsException<InvalidPluginExecutionException>(() => context.ExecutePluginWith<Crm.Plugins.Hotel.PrevalidationCreateHotel>(pluginContext));
        }

        [TestMethod]
        public void CreateTeam()
        {
            var role = new Entity("role", Guid.NewGuid());
            role["name"] = "Tc.Ids.Base";
            role["businessunitid"] = Guid.Empty;           
            var context = new XrmFakedContext();
            context.Initialize(new List<Entity>() {
            role
            });
            context.AddRelationship("teamroles_association", new XrmFakedRelationship("teamid", "roleid", "team", "role"));
            var target = new Entity("tc_hotel") { Id = Guid.NewGuid() };
            target["tc_name"] = "Hotel XXX";
            target["tc_masterhotelid"] = "1002";          
            var pluginContext = new XrmFakedPluginExecutionContext();
            pluginContext.Stage = 10;
            pluginContext.MessageName = "create";
            pluginContext.InputParameters = new ParameterCollection();
            pluginContext.InputParameters.Add("Target", target);
            context.ExecutePluginWith<Crm.Plugins.Hotel.PrevalidationCreateHotel>(pluginContext);
            var team = (from t in context.CreateQuery("team")
                          select t).ToList();
            Assert.IsTrue(team.Count == 1);
        }       
    }
}
