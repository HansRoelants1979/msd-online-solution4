using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tc.Crm.CustomWorkflowSteps.GetTeamDefaultQueue.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Tc.Crm.UnitTests.CustomWorkFlowSteps;
using FakeXrmEasy;
using System.Reflection;
using Microsoft.Xrm.Sdk.Metadata;
using Tc.Crm.CustomWorkflowSteps.GetTeamDefaultQueue;
using static FakeXrmEasy.XrmFakedRelationship;
using Microsoft.Xrm.Sdk.Messages;

namespace Tc.Crm.UnitTests.CustomWorkFlowSteps.GetTeamDefaultQueue.Service
{
    [TestClass()]
    public class GetTeamDefaultQueueServiceTests
    {
        XrmFakedContext fakedContext;
        IOrganizationService service;
        Guid qId = Guid.NewGuid();
        Guid buId = Guid.NewGuid();
        Guid teamId = Guid.NewGuid();
        Guid frId = Guid.NewGuid();

        [TestInitialize()]
        public void Setup()
        {
            fakedContext = new XrmFakedContext();
            service = fakedContext.GetFakedOrganizationService();
            fakedContext.Data = new Dictionary<string, Dictionary<Guid, Entity>>();

            #region BusinessUnit
            var businessUnits = new Dictionary<Guid, Entity>();
            var bu = new Entity("businessunit");
            bu["name"] = "DE";
            bu["businessunitid"] = buId;
            bu.Id = buId;
            businessUnits.Add(buId, bu);
            #endregion BusinessUnit            
            #region Source Markets
            var sourceMarkets = new Dictionary<Guid, Entity>();
            var fr = new Entity("tc_country");
            fr["tc_iso_code"] = "DE";
            fr["tc_countryname"] = "Germany";
            fr["tc_name"] = "Germany";
            fr["tc_sourcemarketbusinessunitid"] = bu;            
            var deId = Guid.NewGuid();
            fr["tc_countryid"] = deId;
            fr.Id = frId;
            sourceMarkets.Add(frId, fr);
            #endregion Source Markets            
            #region Queues
            var queues = new Dictionary<Guid, Entity>();
            var q1 = new Entity("queue");
            q1["name"] = "<DE>";
            q1["tc_sourcemarket"] = new EntityReference("tc_country", frId);
            q1.Id = qId;
            q1["queueid"] = qId;
            queues.Add(qId, q1);
            #endregion Queues
            #region Team
            var teams = new Dictionary<Guid, Entity>();
            var team = new Entity("team");
            team["name"] = "DE";
            team["teamid"] = teamId;
            team.Id = teamId;
            team["isdefault"] = "1";
            team["businessunitid"] = new EntityReference("businessunit",buId);
            //team["businessunitid"] = buId;
            team["queueid"] = q1;
            teams.Add(teamId, bu);            
            #endregion Team



            fakedContext.Data.Add("tc_country", sourceMarkets);
            fakedContext.Data.Add("queue", queues);
            fakedContext.Data.Add("businessunit", businessUnits);
            fakedContext.Data.Add("team", teams);
            #region Relationship
            fakedContext.AddRelationship("business_unit_teams", new XrmFakedRelationship
            {
                Entity2LogicalName = "businessunit",
                Entity2Attribute = "businessunitid",
                Entity1LogicalName = "team",
                Entity1Attribute = "teamid",
                RelationshipType = enmFakeRelationshipType.OneToMany
            });

            var request = new AssociateRequest()
            {
                Target = team.ToEntityReference(),
                RelatedEntities = new EntityReferenceCollection()
                {
                    new EntityReference("businessunit", buId),
                },
                Relationship = new Relationship("business_unit_teams")
            };


            service.Execute(request);
            #endregion Relationship

        }

        //[TestMethod()]
        //public void GetTeamDefaultQueue()
        //{
        //    fakedContext.ProxyTypesAssembly = Assembly.GetAssembly(typeof(CrmEarlyBound.Queue));
        //    var inputs = new Dictionary<string, object>() {

        //        { "SourceMarket", new EntityReference("tc_country",frId) }
        //        };
        //    var result = fakedContext.ExecuteCodeActivity<GetTeamDefaultQueueActivity>(inputs);
        //    var expected = qId;
        //    Assert.AreEqual(expected, ((EntityReference)result["Queue"]).Id);
        //}
    }
}
