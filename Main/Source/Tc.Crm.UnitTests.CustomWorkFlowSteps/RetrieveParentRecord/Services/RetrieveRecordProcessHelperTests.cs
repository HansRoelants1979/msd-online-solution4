using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.UnitTests.CustomWorkFlowSteps;
using Microsoft.Xrm.Sdk;
using Tc.Crm.CustomWorkflowSteps.RetrieveParentRecord.Services;
using Tc.Crm.CustomWorkflowSteps.RetrieveParentRecord;
using FakeXrmEasy;
using System.Reflection;
using Tc.Crm.CustomWorkflowSteps;
using static FakeXrmEasy.XrmFakedRelationship;
using Microsoft.Xrm.Sdk.Messages;

namespace Tc.Crm.UnitTests.CustomWorkFlowSteps.RetrieveParentRecord.Services
{
    [TestClass()]
    public class RetrieveRecordProcessHelperTests
    {
        XrmFakedContext fakedContext;

        Guid accountId = Guid.NewGuid();
        Guid contactId = Guid.NewGuid();
        Guid IncidentId = Guid.NewGuid();

        TestTracingService trace;
        IOrganizationService service;

        [TestInitialize()]
        public void Setp()
        {
            fakedContext = new XrmFakedContext();
            trace = new TestTracingService();
            service = fakedContext.GetFakedOrganizationService();
            fakedContext.Data = new Dictionary<string, Dictionary<Guid, Entity>>();
            #region Account
            var accounts = new Dictionary<Guid, Entity>();
            var account = new Entity("account");
            account["name"] = "Barrhead Travel";
            account.Id = accountId;
            accounts.Add(accountId, account);
            #endregion Account

            #region Contact
            var contacts = new Dictionary<Guid, Entity>();
            var contact = new Entity("contact");
            contact["name"] = "Alan Haslam";
            contact.Id = contactId;
            contact["parentcustomerid"] = new EntityReference("account", accountId);
            contacts.Add(contactId, contact);           
            #endregion Contact

            #region Incident
            var incidents = new Dictionary<Guid, Entity>();
            var incident = new Entity("incident");
            incident["name"] = "TC-00291-G9M7";
            incident.Id = IncidentId;
            incident["customerid"] = new EntityReference("contact", contactId);
            incidents.Add(IncidentId, incident);
            #endregion Incident

            fakedContext.Data.Add("account", accounts);
            fakedContext.Data.Add("contact", contacts);
            fakedContext.Data.Add("incident", incidents);
            

            #region relationships

            fakedContext.AddRelationship("contact_customer_accounts", new XrmFakedRelationship
            {              
                Entity1LogicalName = "contact",
                Entity1Attribute = "parentcustomerid",
                Entity2LogicalName = "account",
                Entity2Attribute = "accountid",
                RelationshipType = enmFakeRelationshipType.OneToMany
        });

            fakedContext.AddRelationship("incident_customer_contacts", new XrmFakedRelationship
            {
                Entity2LogicalName = "incident",
                Entity2Attribute = "customerid",
                Entity1LogicalName = "contact",
                Entity1Attribute = "contactid",
                RelationshipType = enmFakeRelationshipType.OneToMany
            });

            var request = new AssociateRequest()
            {
                Target = incident.ToEntityReference(),
                RelatedEntities = new EntityReferenceCollection()
                {
                    new EntityReference("contact", contactId),
                },
                Relationship = new Relationship("incident_customer_contacts")
            };


            service.Execute(request);

            var request1 = new AssociateRequest()
            {
                Target = contact.ToEntityReference(),
                RelatedEntities = new EntityReferenceCollection()
                {
                    new EntityReference("account", accountId),
                },
                Relationship = new Relationship("contact_customer_accounts")
            };


            service.Execute(request1);
            #endregion relationships

            
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidPluginExecutionException), "incident_customer_contacts relationship is not present in the CRM system")]
        public void RetrieveParentRecord()
        {
            fakedContext.ProxyTypesAssembly = Assembly.GetAssembly(typeof(CrmEarlyBound.Queue));
            var inputs = new Dictionary<string, object>() {
                { "Expression", "incident||incident_customer_contacts;contact||contact_customer_accounts;account"}
                };
            //fakedContext.CallerId = new EntityReference("systemuser", u2Id);
           fakedContext.ExecuteCodeActivity<RetrieveParentRecordActivity>(inputs);
            
        }
    }

}
