using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using FakeXrmEasy;

namespace Tc.Crm.UnitTests.Plugins
{
    [TestClass]
    public class CreateSharePointDocumentLocationTests
    {
        Guid annotationId = Guid.NewGuid();
        Guid opportunityId = Guid.NewGuid();
        Guid incidentId = Guid.NewGuid();
        Guid sdlId = Guid.NewGuid();
        Guid sdlIdBase = Guid.NewGuid();

        Entity sdlEntity = null;
        Entity sdlEntityBase = null;
        Entity incidentEntity = null;

        [TestInitialize]
        public void Initialise()
        {
            sdlEntity = new Entity("sharepointdocumentlocation", sdlId);
            sdlEntity.Attributes.Add("regardingobjectid", new EntityReference("incident", incidentId));

            sdlEntityBase = new Entity("sharepointdocumentlocation", sdlIdBase);
            sdlEntityBase.Attributes.Add("relativeurl", "incident");

            incidentEntity = new Entity("incident", incidentId);
            incidentEntity.Attributes.Add("ticketnumber", "TC-XX-XXXX");
        }

        [TestMethod]
        public void CreateNoteNoAttachment()
        {
            var context = new XrmFakedContext();            
            var target = new Microsoft.Xrm.Sdk.Entity("annotation") { Id = annotationId };           
            var fakedPlugin = context.ExecutePluginWithTarget<Crm.Plugins.CreateSharePointDocumentLocation>(target);
            var documentLocations = (from t in context.CreateQuery("sharepointdocumentlocation")
                                     select t).ToList();                       
            Assert.IsTrue(documentLocations.Count == 0);            
        }

        [TestMethod]
        public void CreateNoteWithAttachmentRegardingNull()
        {
            var context = new XrmFakedContext();
            var target = new Microsoft.Xrm.Sdk.Entity("annotation")
            {
                Id = annotationId,               
            };            
            target.Attributes.Add("filename", "spam.txt");
            var fakedPlugin = context.ExecutePluginWithTarget<Crm.Plugins.CreateSharePointDocumentLocation>(target);
            var documentLocations = (from t in context.CreateQuery("sharepointdocumentlocation")
                                     select t).ToList();
            Assert.IsTrue(documentLocations.Count == 0);
        }

        [TestMethod]
        public void CreateNoteWithAttachmentRegardingOpportunity()
        {
            var context = new XrmFakedContext();
            var target = new Microsoft.Xrm.Sdk.Entity("annotation")
            {
                Id = annotationId,
            };
            target.Attributes.Add("objectid", new EntityReference("opportunity", opportunityId));
            target.Attributes.Add("filename", "spam.txt");
            var fakedPlugin = context.ExecutePluginWithTarget<Crm.Plugins.CreateSharePointDocumentLocation>(target);
            var documentLocations = (from t in context.CreateQuery("sharepointdocumentlocation")
                                     select t).ToList();
            Assert.IsTrue(documentLocations.Count == 0);
        }

        [TestMethod]
        public void CreateNoteWithAttachmentRegardingCaseWithExistingDocumentLocation()
        {
            var context = new XrmFakedContext();
            context.Initialize(new List<Entity> { sdlEntity });
            var target = new Microsoft.Xrm.Sdk.Entity("annotation")
            {
                Id = annotationId,
            };
            target.Attributes.Add("objectid", new EntityReference("incident", incidentId));
            target.Attributes.Add("filename", "spam.txt");
            var fakedPlugin = context.ExecutePluginWithTarget<Crm.Plugins.CreateSharePointDocumentLocation>(target);
            var documentLocations = (from t in context.CreateQuery("sharepointdocumentlocation")
                                     where t.Id != sdlEntity.Id
                                     select t)
                                     .ToList();
            Assert.IsTrue(documentLocations.Count == 0);
        }

        [TestMethod]
        public void CreateNoteWithAttachmentRegardingCaseNoDocumentLocationNoBaseDocumentLocation()
        {
            var context = new XrmFakedContext();
            context.Initialize(new List<Entity>() { incidentEntity } );
            var target = new Microsoft.Xrm.Sdk.Entity("annotation")
            {
                Id = annotationId,
            };
            target.Attributes.Add("objectid", new EntityReference("incident", incidentId));
            target.Attributes.Add("filename", "spam.txt");

            Assert.ThrowsException<InvalidPluginExecutionException>(() => 
                context.ExecutePluginWithTarget<Crm.Plugins.CreateSharePointDocumentLocation>(target));            
        }

        [TestMethod]
        public void CreateNoteWithAttachmentRegardingCaseNoDocumentLocationWithBaseDocumentLocation()
        {
            var context = new XrmFakedContext();
            context.Initialize(new List<Entity>() { incidentEntity, sdlEntityBase });
            var target = new Microsoft.Xrm.Sdk.Entity("annotation")
            {
                Id = annotationId,
            };
            target.Attributes.Add("objectid", new EntityReference("incident", incidentId));
            target.Attributes.Add("filename", "spam.txt");
            var fakedPlugin = context.ExecutePluginWithTarget<Crm.Plugins.CreateSharePointDocumentLocation>(target);
            var documentLocations = (from t in context.CreateQuery("sharepointdocumentlocation")
                                     where t.Id != sdlEntityBase.Id
                                     select t)
                                     .ToList();
            Assert.IsTrue(documentLocations.Count == 1);
            Assert.IsTrue(documentLocations[0].Attributes["relativeurl"].ToString() == "TC-XX-XXXX");
            Assert.IsTrue(documentLocations[0].Attributes["name"].ToString() == "TC-XX-XXXX");
            Assert.IsTrue((documentLocations[0].Attributes["parentsiteorlocation"] as EntityReference)
                .Id == sdlEntityBase.Id);
            Assert.IsTrue((documentLocations[0].Attributes["regardingobjectid"] as EntityReference)
                .Id == incidentId);            
        }
    }
}
