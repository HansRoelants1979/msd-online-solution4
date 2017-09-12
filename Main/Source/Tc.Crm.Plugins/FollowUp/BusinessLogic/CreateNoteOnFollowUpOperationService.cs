using Microsoft.Crm.Sdk.Messages;
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
            string contactTime = string.Empty;
            DateTime  dueDate = new DateTime();
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
            if (!followUpPreImage.Contains(Attributes.FollowUp.DueDate) || followUpPreImage[Attributes.FollowUp.DueDate] == null)
            {
                trace.Trace("Due Date Time is null or undefined.");
                return;
            }
            
            targetFollowUp = (Entity)context.InputParameters["Target"];
            if (targetFollowUp.Id != Guid.Empty)
                followUpId = targetFollowUp.Id;
            if(followUpPreImage[Attributes.FollowUp.ContactTime] != null)
               contactTime = followUpPreImage.FormattedValues[Attributes.FollowUp.ContactTime];
            int userTimeZone = RetrieveCurrentUserTimeZoneCode(service);
            dueDate = LocalTimeFromUTCTime(Convert.ToDateTime(followUpPreImage[Attributes.FollowUp.DueDate]),userTimeZone,service);            
            rescheduleReason = followUpPreImage[Attributes.FollowUp.RescheduleReason].ToString();            
            noteText = rescheduleReason + "-" + dueDate.ToShortDateString() + " " + contactTime ;
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

        public DateTime LocalTimeFromUTCTime(DateTime utcTime, int timeZoneCode, IOrganizationService service)


        {
            var request = new LocalTimeFromUtcTimeRequest
            {
                TimeZoneCode = timeZoneCode,
                UtcTime = utcTime.ToUniversalTime()
            };

            var response = (LocalTimeFromUtcTimeResponse)service.Execute(request);
            return response.LocalTime;

        }


        private int RetrieveCurrentUserTimeZoneCode(IOrganizationService service)

        {
            int timeZoneCode =85;

            var currentUserSettings = service.RetrieveMultiple(
                new QueryExpression("usersettings")
                {
                    ColumnSet = new ColumnSet("timezonecode"),
                    Criteria = new FilterExpression
                    {
                        Conditions =
                      {
                            new ConditionExpression("systemuserid", ConditionOperator.EqualUserId)
                        }

                    }

                }).Entities[0].ToEntity<Entity>();

            //return time zone code

            return timeZoneCode= Convert.ToInt32(currentUserSettings.Attributes["timezonecode"]);



        }


    }
}
