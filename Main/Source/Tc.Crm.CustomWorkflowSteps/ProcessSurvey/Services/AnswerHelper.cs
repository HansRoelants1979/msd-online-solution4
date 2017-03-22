using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Tc.Crm.CustomWorkflowSteps.ProcessSurvey.Models;
using System.Text;
using System;

namespace Tc.Crm.CustomWorkflowSteps.ProcessSurvey.Services
{
    public static class AnswerHelper
    {
        /// <summary>
        /// To prepare feedback entity from payload
        /// </summary>
        /// <param name="answer"></param>
        /// <returns></returns>
        public static Entity GetFeedbackEntityFromPayLoad(Answer answer, ITracingService trace)
        {
            trace.Trace("Processing GetFeedbackEntityFromPayLoad - start");
            if (answer == null) throw new InvalidPluginExecutionException("Answer class in json payload is null");
            var feedback = new Entity(EntityName.SurveyResponseFeedback);
            if (!string.IsNullOrWhiteSpace(answer.FieldText))
                feedback[Attributes.SurveyResponseFeedback.Name] = answer.FieldText;
            if (answer.Id != null)
                feedback[Attributes.SurveyResponseFeedback.QuestionId] = Int32.Parse(answer.Id.ToString());
            feedback[Attributes.SurveyResponseFeedback.QuestionFieldId] = Convert.ToInt32(answer.FieldId);
            if (!string.IsNullOrWhiteSpace(answer.FieldName))
                feedback[Attributes.SurveyResponseFeedback.QuestionName] = answer.FieldName;
            if (!string.IsNullOrWhiteSpace(answer.FieldLabel))
                feedback[Attributes.SurveyResponseFeedback.QuestionFieldLabel] = answer.FieldLabel;
            if (!string.IsNullOrWhiteSpace(answer.FieldType.ToString()))
            {
                feedback[Attributes.SurveyResponseFeedback.QuestionFieldType] = answer.FieldType.ToString();
                feedback[Attributes.SurveyResponseFeedback.QuestionResponse] = GetValueByFieldType(answer,trace);
            }
            trace.Trace("Processing GetFeedbackEntityFromPayLoad - end");
            return feedback;
        }

        /// <summary>
        /// To get value by field type
        /// </summary>
        /// <param name="answer"></param>
        /// <returns></returns>
        private static string GetValueByFieldType(Answer answer, ITracingService trace)
        {
            trace.Trace("Processing GetValueByFieldType - start");
            string value = string.Empty;
            switch (answer.FieldType)
            {
                case FieldType.UNKNOWN:
                case FieldType.TEXT:
                case FieldType.NUMERIC:
                case FieldType.DATE:
                case FieldType.TIME:
                case FieldType.DAY_OF_WEEK:
                case FieldType.MONTH:
                case FieldType.BOOLEAN:
                    value = answer.LiteralValue;
                    break;
                case FieldType.RATING:
                case FieldType.MULTIPLE_CHOICE:
                    value = GetOptionValue(answer.Option,trace);
                    break;
                case FieldType.COMMENT:
                    value = GetCommentValue(answer,trace);
                    break;
            }
            trace.Trace("Processing GetValueByFieldType - end");
            return value;
        }

        /// <summary>
        /// To get option value
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        private static string GetOptionValue(Option option, ITracingService trace)
        {
            trace.Trace("Processing GetOptionValue - start");
            var value = new StringBuilder();
            value.Append("Id: " + option.Id);
            if (!string.IsNullOrWhiteSpace(option.Name))
                value.AppendLine("Name: " + option.Name);

            if (!string.IsNullOrWhiteSpace(option.DefaultLabel))
            {
                value.AppendLine("Label: " + option.DefaultLabel);
            }
            else if (option.Label != null && option.Label.Count > 0)
            {
                for (int i = 0; i < option.Label.Count; i++)
                {
                    if (option.Label[i] != null)
                        value.AppendLine(GetLocalizedValue(option.Label[i],trace));
                }
            }
            trace.Trace("Processing GetOptionValue - end");
            return value.ToString();
        }

        /// <summary>
        /// To get localized value
        /// </summary>
        /// <param name="localizedString"></param>
        /// <returns></returns>
        private static string GetLocalizedValue(LocalizedString localizedString, ITracingService trace)
        {
            trace.Trace("Processing GetLocalizedValue - start");
            var value = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(localizedString.Value))
                value.AppendLine("Label: " + localizedString.Value);
            if (!string.IsNullOrWhiteSpace(localizedString.Locale))
                value.AppendLine("Locale: " + localizedString.Locale);
            trace.Trace("Processing GetLocalizedValue - end");
            return value.ToString();
        }

        /// <summary>
        /// To get comment value
        /// </summary>
        /// <param name="answer"></param>
        /// <returns></returns>
        private static string GetCommentValue(Answer answer, ITracingService trace)
        {
            trace.Trace("Processing GetCommentValue - start");
            var value = new StringBuilder();
            if (answer.CommentId != null)
                value.Append("CommentId: " + answer.CommentId);
            if (!string.IsNullOrWhiteSpace(answer.CommentType))
                value.AppendLine("CommentType: " + answer.CommentType);
            if (answer.CommentArchived != null)
                value.AppendLine("CommentArchived: " + answer.CommentArchived);
            trace.Trace("Processing GetCommentValue - end");
            return value.ToString();
        }

        /// <summary>
        /// To find booking from survey payload
        /// </summary>
        /// <param name="answers"></param>
        /// <returns></returns>
        public static string FindBooking(List<Answer> answers, ITracingService trace)
        {
            trace.Trace("Processing FindBooking - start");
            var bookingNumber = string.Empty;
            if (answers != null && answers.Count > 0)
            {
                var booking = answers.Find(a => a.FieldId == PayloadSurveyFieldMapping.BookingNumber);
                var sourceMarket = answers.Find(a => a.FieldId == PayloadSurveyFieldMapping.SourceMarket);
                if (booking != null && !string.IsNullOrWhiteSpace(booking.LiteralValue))
                {
                    if (sourceMarket != null && !string.IsNullOrWhiteSpace(sourceMarket.LiteralValue))
                    {
                        bookingNumber = booking.LiteralValue + sourceMarket.LiteralValue;
                    }
                }
            }
            trace.Trace("Processing FindBooking - end");
            return bookingNumber;
        }

        /// <summary>
        /// To find customer from survey payload
        /// </summary>
        /// <param name="answers"></param>
        /// <returns></returns>
        public static string FindCustomer(List<Answer> answers, ITracingService trace)
        {
            trace.Trace("Processing FindCustomer - start");
            var customer = string.Empty;
            if (answers != null && answers.Count > 0)
            {
                var forename = answers.Find(a => a.FieldId == PayloadSurveyFieldMapping.ForeName);
                var surName = answers.Find(a => a.FieldId == PayloadSurveyFieldMapping.SurName);
                if (forename != null && !string.IsNullOrWhiteSpace(forename.LiteralValue))
                {
                    if (surName != null && !string.IsNullOrWhiteSpace(surName.LiteralValue))
                    {
                        customer = surName.LiteralValue + " " + forename.LiteralValue;
                    }
                }
            }
            trace.Trace("Processing FindCustomer - end");
            return customer;
        }
    }
}
