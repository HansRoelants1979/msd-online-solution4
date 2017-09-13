using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeXrmEasy;
using Tc.Crm.Plugins;
using Attributes = Tc.Crm.Plugins.Attributes;
using Tc.Crm.Plugins.OptionSetValues;

namespace Tc.Crm.UnitTests.Plugins
{
    [TestClass]
    public class CreateNoteOnFollowUpOperationServiceTests
    {
        private Entity followUpEntity = null;
        private Entity noteEntity =null;
        private Entity userSettingsEntity = null;        
        Guid initiatingUserId = Guid.NewGuid();
        Guid callingUserId = Guid.NewGuid();
        
        XrmFakedContext context = null;

        [TestInitialize]

        public void Initialise()
        {
            followUpEntity = new Entity(Entities.FollowUp, Guid.NewGuid());            
            followUpEntity.Attributes.Add(Attributes.FollowUp.ContactTime, null);
            followUpEntity.Attributes.Add(Attributes.FollowUp.DueDate, null);
            followUpEntity.Attributes.Add(Attributes.FollowUp.RescheduleReason, null) ;
            followUpEntity.FormattedValues[Attributes.FollowUp.ContactTime] = "12-00 - 14:30 ";

            noteEntity = new Entity(Entities.Annotation, Guid.NewGuid());
            noteEntity.Attributes.Add(Attributes.Annotation.ObjectId,followUpEntity.Id);
            noteEntity.Attributes.Add(Attributes.Annotation.Subject, "Reschedule Reason");
            noteEntity.Attributes.Add(Attributes.Annotation.NoteText,followUpEntity.Attributes[Attributes.FollowUp.RescheduleReason]);            


            userSettingsEntity = new Entity("usersettings", Guid.NewGuid());
            userSettingsEntity.Attributes.Add("timezonecode", 190);
            userSettingsEntity.Attributes.Add("systemuserid", Guid.NewGuid());

            context = new XrmFakedContext();
            
            context.Initialize(new List<Entity>() { followUpEntity, noteEntity, userSettingsEntity });
        }

        private XrmFakedPluginExecutionContext GetPluginContext()
        {
           
            var cntxt = new XrmFakedPluginExecutionContext();
            cntxt.InputParameters = new ParameterCollection();
            cntxt.InputParameters.Add(InputParameters.Target, followUpEntity);
            cntxt.PreEntityImages = new EntityImageCollection();
            cntxt.PreEntityImages.Add(Attributes.FollowUp.ContactTime, followUpEntity);
            cntxt.PreEntityImages.Add(Attributes.FollowUp.DueDate, followUpEntity);
            cntxt.PreEntityImages.Add(Attributes.FollowUp.RescheduleReason, followUpEntity);
            cntxt.PreEntityImages.Add("PreImage", followUpEntity);            
            cntxt.InitiatingUserId = initiatingUserId;
            cntxt.UserId = initiatingUserId;
            cntxt.MessageName = "update";
            cntxt.Stage = 40;            
            return cntxt;
        }


        [TestMethod]
        public void FollowupWithNoPreImages()
        {
            var cntxt = GetPluginContext();
            cntxt.PreEntityImages.Remove("PreImage");
            var fakedPlugin = context.ExecutePluginWith<Crm.Plugins.FollowUp.CreateNoteOnFollowUpOperation>(cntxt);

            Assert.IsTrue(context.GetFakeTracingService().DumpTrace().Contains(string.Format("No pre update image has been configured.")));

        }

        [TestMethod]
        public void FollowupWithNoDueDateValue()
        {
            var cntxt = GetPluginContext();                
                   
            var fakedPlugin = context.ExecutePluginWith<Crm.Plugins.FollowUp.CreateNoteOnFollowUpOperation>(cntxt);          
            
            Assert.IsTrue(context.GetFakeTracingService().DumpTrace().Contains(string.Format("Due Date  is null or undefined.")));
            
        }

        [TestMethod]
        public void NoUserSettingsEntities()
        {
            followUpEntity.Attributes.Remove(Attributes.FollowUp.DueDate);
            followUpEntity.Attributes.Add(Attributes.FollowUp.DueDate, new DateTime(2018, 09, 15, 05, 06, 07, DateTimeKind.Local));
            var cntxt = GetPluginContext();       
            var fakedPlugin = context.ExecutePluginWith<Crm.Plugins.FollowUp.CreateNoteOnFollowUpOperation>(cntxt);

            Assert.IsTrue(context.GetFakeTracingService().DumpTrace().Contains(string.Format("User Setings Entities is null or count is null")));

        }



    }
}
