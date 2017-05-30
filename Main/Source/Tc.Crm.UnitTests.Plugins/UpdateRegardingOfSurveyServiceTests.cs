using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeXrmEasy;

namespace Tc.Crm.UnitTests.Plugins
{
    [TestClass]
    public class UpdateRegardingOfSurveyServiceTests
    {

        private XrmFakedPluginExecutionContext GetPluginContext(string surveyId)
        {
            var entCase = new Entity("incident", Guid.NewGuid());
            if (!string.IsNullOrWhiteSpace(surveyId))
                entCase.Attributes["tc_surveyid"] = Guid.Parse(surveyId);
            var cntxt = new XrmFakedPluginExecutionContext();
            cntxt.InputParameters = new ParameterCollection();            
            cntxt.InputParameters.Add("Target", entCase);
            cntxt.MessageName = "Create";
            cntxt.Stage = 40;
            return cntxt;
        }

        [TestMethod]
        public void TestWithSurveyId()
        {
            var context = new XrmFakedContext();
            var surveyId = Guid.NewGuid();
            var survey = new Entity("tc_surveyresponse", surveyId);
            survey["regardingobjectid"] = null;
            context.Initialize(new List<Entity> { survey });
            var cntxt = GetPluginContext(surveyId.ToString());
            context.ExecutePluginWith<Tc.Crm.Plugins.Case.PostCaseCreationUpdateRegardingOfSurvey>(cntxt);
            var surveyUpdated = (from t in context.CreateQuery("tc_surveyresponse")
                                 where t.Id == surveyId
                                 select t).ToList();
            Assert.IsTrue(surveyUpdated.Count == 1);
            var surveyUpdated1 = surveyUpdated[0];
            Assert.IsNotNull(surveyUpdated1["regardingobjectid"]);
        }

        [TestMethod]
        public void TestWithoutSurveyId()
        {
            var context = new XrmFakedContext();
            var surveyId = Guid.NewGuid();
            var survey = new Entity("tc_surveyresponse", surveyId);
            survey["regardingobjectid"] = null;
            context.Initialize(new List<Entity> { survey });
            var cntxt = GetPluginContext("");
            context.ExecutePluginWith<Tc.Crm.Plugins.Case.PostCaseCreationUpdateRegardingOfSurvey>(cntxt);
            var surveyUpdated = (from t in context.CreateQuery("tc_surveyresponse")
                                 where t.Id == surveyId
                                 select t).ToList();
            Assert.IsTrue(surveyUpdated.Count == 1);
            var surveyUpdated1 = surveyUpdated[0];
            Assert.IsNull(surveyUpdated1["regardingobjectid"]);
        }
    }
}
