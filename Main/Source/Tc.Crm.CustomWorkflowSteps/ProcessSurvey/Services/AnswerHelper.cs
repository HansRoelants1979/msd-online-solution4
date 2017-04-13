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
        public static Entity GetFeedbackEntityFromPayload(Answer answer, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Trace service is null;");
            trace.Trace("Processing GetFeedbackEntityFromPayLoad - start");
            if (answer == null) throw new InvalidPluginExecutionException("Answer class in json payload is null");
            var feedback = new Entity(EntityName.SurveyResponseFeedback);
            if (!string.IsNullOrWhiteSpace(answer.FieldText))
                feedback[Attributes.SurveyResponseFeedback.Name] = answer.FieldText;
            if (answer.Id != null)
                feedback[Attributes.SurveyResponseFeedback.QuestionId] = answer.Id.ToString();
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
            if (trace == null) throw new InvalidPluginExecutionException("Trace service is null;");
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
                case FieldType.COMMENT:
                    value = answer.LiteralValue;
                    break;
                case FieldType.RATING:
                case FieldType.MULTIPLE_CHOICE:
                    value = GetOptionValue(answer.Option,trace);
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
            if (trace == null) throw new InvalidPluginExecutionException("Trace service is null;");
            trace.Trace("Processing GetOptionValue - start");            
            trace.Trace("Processing GetOptionValue - end");
            return option.DefaultLabel;
        }
                      
        /// <summary>
        /// To find booking from survey payload
        /// </summary>
        /// <param name="answers"></param>
        /// <returns></returns>
        public static string FindBooking(List<Answer> answers, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Trace service is null;");
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
            if (trace == null) throw new InvalidPluginExecutionException("Trace service is null;");
            trace.Trace("Processing FindCustomer - start");
            var customer = string.Empty;
            if (answers != null && answers.Count > 0)
            {
                var surName = answers.Find(a => a.FieldId == PayloadSurveyFieldMapping.Surname);               
                if (surName != null && !string.IsNullOrWhiteSpace(surName.LiteralValue))
                {
                    customer = surName.LiteralValue;
                }                
            }
            trace.Trace("Processing FindCustomer - end");
            return customer;
        }
    }
}
