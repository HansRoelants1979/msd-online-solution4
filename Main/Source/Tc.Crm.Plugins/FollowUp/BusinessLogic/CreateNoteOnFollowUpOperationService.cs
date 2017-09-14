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
            DateTime dueDate = new DateTime();
            string contactTime = string.Empty;
            string noteText = string.Empty;
            string rescheduleReason = string.Empty;
            string subject = "Reschedule Reason";


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
                trace.Trace("Due Date  is null or undefined.");
                return;
            }           

              targetFollowUp = (Entity)context.InputParameters["Target"];
            if (targetFollowUp.Id != Guid.Empty)
                followUpId = targetFollowUp.Id;

            //get reschedule reasom value from PreImage
            if (!followUpPreImage.Contains(Attributes.FollowUp.RescheduleReason)) return;
            if (followUpPreImage[Attributes.FollowUp.RescheduleReason] == null) return;
                    rescheduleReason = followUpPreImage[Attributes.FollowUp.RescheduleReason].ToString();

            // get contact time value from PreImage
            if (followUpPreImage.Contains(Attributes.FollowUp.ContactTime))
                if (followUpPreImage[Attributes.FollowUp.ContactTime] != null)
                    contactTime = followUpPreImage.FormattedValues[Attributes.FollowUp.ContactTime];

            // get due date value from PreImage
            int userTimeZone = RetrieveCurrentUserTimeZoneCode(service);
            if (userTimeZone == -1) return;
            dueDate = LocalTimeFromUTCTime(Convert.ToDateTime(followUpPreImage[Attributes.FollowUp.DueDate]), userTimeZone, service);
            

            noteText = rescheduleReason + "-" + dueDate.ToShortDateString() + " " + contactTime;
            CreateNote(followUpId, subject, noteText);

            trace.Trace("End - PrePareNoteFromFollouWp");
        }

        private void CreateNote(Guid objectId, string subject, string noteText)
        {
            trace.Trace("Begin - CreateNote");
            Entity Note = new Entity(Entities.Annotation);
            if (objectId != Guid.Empty)
                Note[Attributes.Annotation.ObjectId] = new EntityReference(Entities.FollowUp, objectId);
            if (subject != null)
                Note[Attributes.Annotation.Subject] = subject;
            if (noteText != null)
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
            int timeZoneCode = -1;
            var currentUserSettings = service.RetrieveMultiple(
                new QueryExpression(Entities.UserSettings)
                {
                    ColumnSet = new ColumnSet(Attributes.UserSettings.TimeZoneCode),
                    Criteria = new FilterExpression
                    {
                        Conditions =
                      {
                            new ConditionExpression(Attributes.UserSettings.SystemUserId, ConditionOperator.Equal,context.InitiatingUserId)
                        }
                    }
                });

            var entities = currentUserSettings.Entities;
            if (entities == null || entities.Count == 0)
            {
                trace.Trace("User Setings Entities is null or count is null");
                return timeZoneCode;
            }
            if (entities[0].Contains("timezonecode") && entities[0]["timezonecode"] != null)
            {
                timeZoneCode = Convert.ToInt32(entities[0]["timezonecode"]);
            }
            return timeZoneCode;
        }
    }
}
