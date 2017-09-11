using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tc.Crm.Plugins.FollowUp.BusinessLogic
{
    public class CreateNoteOnFollowUpOperationService
    {

        public IPluginExecutionContext context = null;
        public IOrganizationService service = null;
        public ITracingService trace = null;

        public CreateNoteOnFollowUpOperationService(IPluginExecutionContext context, ITracingService trace, IOrganizationService service)
        {
            this.context = context;
            this.service = service;
            this.trace = trace;
        }

        private bool IsContextValid()
        {
            if (!context.MessageName.Equals("update", StringComparison.OrdinalIgnoreCase)) return false;
            if (context.Stage != (int)PluginStage.Postoperation) return false;
            if (!context.InputParameters.Contains(InputParameters.Target)
                || !(context.InputParameters[InputParameters.Target] is Entity))
                return false;
            return true;
        }

        public void PrePareNoteFromFolloWup()
        {
            Entity targetFollowUp = null;
            Guid followUpId = Guid.Empty;
            String contactTime = string.Empty;
            string  dueDate = string.Empty;
            string noteText = string.Empty;
            string rescheduleReason = string.Empty;
            string subject = "ReSchedule Reason";


            trace.Trace("Begin - PrePareNoteFromFollouWp");
            if (!IsContextValid()) return;

            if (!context.PreEntityImages.Contains("PreImage"))
            {
                trace.Trace("No pre update image has been configured.");
                return;
            }
                

            var followUpPreImage = (Entity)context.PreEntityImages["PreImage"];           
            if (!followUpPreImage.Contains(Attributes.FollowUp.DueDate) || followUpPreImage[Attributes.FollowUp.DueDate] == null || !followUpPreImage.Contains(Attributes.FollowUp.ContactTime) || followUpPreImage[Attributes.FollowUp.ContactTime] == null)
            {
                trace.Trace("Due Date/Contact Time is null or undefined.");
                return;
            }
            
            targetFollowUp = (Entity)context.InputParameters["Target"];
            if (targetFollowUp.Id != Guid.Empty)
                followUpId = targetFollowUp.Id;
            contactTime = followUpPreImage.FormattedValues[Attributes.FollowUp.ContactTime] ;
            dueDate = Convert.ToDateTime(followUpPreImage[Attributes.FollowUp.DueDate]).ToLocalTime().ToShortDateString();
            rescheduleReason = followUpPreImage[Attributes.FollowUp.RescheduleReason].ToString();            
            noteText = rescheduleReason + "-" + dueDate + " " + contactTime ;
            CreateNote(followUpId,subject, noteText);
            trace.Trace("End - PrePareNoteFromFollouWp");
        }

        private void CreateNote(Guid objectId,string subject, string noteText)
        {
            trace.Trace("Begin - CreateNote");

            Entity Note = new Entity(Entities.Annotation);
            if (objectId != Guid.Empty)
                Note[Attributes.Annotation.ObjectId] = new EntityReference(Entities.FollowUp, objectId);
            if(subject != null)
            Note[Attributes.Annotation.Subject] = subject;
            if(noteText != null)
            Note[Attributes.Annotation.NoteText] = noteText;

            service.Create(Note);

            trace.Trace("End - CreateNote");

        }

    }
}
