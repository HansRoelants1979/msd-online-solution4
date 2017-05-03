using System.Linq;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeXrmEasy;
using System;

namespace Tc.Crm.UnitTests.Plugins
{
    [TestClass]
    public class PreoperationUpdateEmailBodyTest
    {
        [TestMethod]
        public void EmailWithNoDescription()
        {
            var context = new XrmFakedContext();
            var target = new Microsoft.Xrm.Sdk.Entity("email") { Id = Guid.NewGuid() };
            var pluginContext = new XrmFakedPluginExecutionContext();
            pluginContext.Stage = 20;
            pluginContext.MessageName = "create";
            pluginContext.InputParameters = new ParameterCollection();
            pluginContext.InputParameters.Add("Target", target);
            Assert.ThrowsException<InvalidPluginExecutionException>(() => context.ExecutePluginWith<Crm.Plugins.Email.PreoperationUpdateEmailBody>(pluginContext));
        }

        [TestMethod]
        public void EmailWithNoGuid()
        {
            var context = new XrmFakedContext();
            var target = new Microsoft.Xrm.Sdk.Entity("email") { Id = Guid.Empty };
            var pluginContext = new XrmFakedPluginExecutionContext();
            pluginContext.Stage = 20;
            pluginContext.MessageName = "create";
            pluginContext.InputParameters = new ParameterCollection();
            pluginContext.InputParameters.Add("Target", target);
            Assert.ThrowsException<InvalidPluginExecutionException>(() => context.ExecutePluginWith<Crm.Plugins.Email.PreoperationUpdateEmailBody>(pluginContext));
        }

        //[TestMethod]
        //public void EmailWithDescription()
        //{
        //    var context = new XrmFakedContext();
        //    var target = new Microsoft.Xrm.Sdk.Entity("email") { Id = Guid.NewGuid() };
        //    var pluginContext = new XrmFakedPluginExecutionContext();
        //    pluginContext.Stage = 20;
        //    pluginContext.MessageName = "create";
        //    pluginContext.InputParameters = new ParameterCollection();
        //    pluginContext.InputParameters.Add("Target", target);
        //    target["description"] = "{!EmailHeaderFooter:Test Footer1}";
        //    var emailDescription = string.Empty;
        //    var fakedPlugin = context.ExecutePluginWith<Crm.Plugins.Email.PreoperationUpdateEmailBody>(pluginContext);
         
        //}

        //[TestMethod]
        //public void EmailWithNoFooterHeaderNameDescription()
        //{
        //    var context = new XrmFakedContext();
        //    var target = new Microsoft.Xrm.Sdk.Entity("email") { Id = Guid.NewGuid() };
        //    var pluginContext = new XrmFakedPluginExecutionContext();
        //    pluginContext.Stage = 20;
        //    pluginContext.MessageName = "create";
        //    pluginContext.InputParameters = new ParameterCollection();
        //    pluginContext.InputParameters.Add("Target", target);
        //    target["description"] = "{XXXX}";
        //    var emailDescription = string.Empty;
        //    var fakedPlugin = context.ExecutePluginWith<Crm.Plugins.Email.PreoperationUpdateEmailBody>(pluginContext);
        //    var email = context.CreateQueryFromEntityName("email").FirstOrDefault();
        //    Assert.IsTrue((email.Attributes["description"] as string) == "{XXXX}");
        //    //Assert.IsTrue( emailDescription == "{ XXXX}");
        //}
    }
}
