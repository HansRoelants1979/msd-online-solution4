using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tc.Crm.Service.Services;
using System;
using Tc.Crm.Service.Models;
using FakeXrmEasy;
using Tc.Crm.ServiceTests;


namespace Tc.Crm.ServiceTests.Services
{
    [TestClass()]
    public class SurveyServiceTests
    {
        XrmFakedContext context;
        ISurveyService surveyService;
        TestCrmService crmService;

        [TestInitialize()]
        public void TestSetup()
        {
            context = new XrmFakedContext();
            surveyService = new SurveyService();
            crmService = new TestCrmService(context);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException), Service.Constants.Parameters.DataJson)]
        public void SurveyIsNull()
        {
            surveyService.ProcessSurvey(null, crmService);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException), Service.Constants.Parameters.DataJson)]
        public void SurveyIsEmpty()
        {
            surveyService.ProcessSurvey(string.Empty, crmService);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException), Service.Constants.Parameters.CrmService)]
        public void CrmServiceIsNull()
        {
            surveyService.ProcessSurvey("some data", null);
        }

        [TestMethod()]        
        public void ResponseIsNull()
        {
            crmService.Switch = DataSwitch.Response_NULL;
            SurveyReturnResponse resp = surveyService.ProcessSurvey("some data", crmService);
            Assert.AreEqual(resp.Created, false);
        }

        [TestMethod()]
        public void OnSuccess()
        {
            crmService.Switch = DataSwitch.Created;
            SurveyReturnResponse resp = surveyService.ProcessSurvey("{}", crmService);
            Assert.AreEqual(resp.Created, true);
        }
    }
}
