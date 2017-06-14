using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Tc.Crm.CustomWorkflowSteps.ProcessSurvey.Models;
using System;
using System.Linq;

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
        /// To prepare feedback entity from payload
        /// </summary>
        /// <param name="answer"></param>
        /// <param name="existingFeedback"></param>
        /// <param name="isSurveyCreate"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        public static Entity GetFeedbackEntityFromPayload(Answer answer, Dictionary<Guid, string> existingFeedback, bool isCreateSurvey, ITracingService trace)
        {
            Entity feedback = null;
            if (existingFeedback != null && existingFeedback.Count > 0)
            {
                KeyValuePair<Guid, string> feedbackKey = existingFeedback.FirstOrDefault(f => f.Value == answer.Id.ToString());
                if (feedbackKey.Key != Guid.Empty)
                {
                    feedback = GetFeedbackEntityFromPayload(answer, trace);
                    feedback.Id = feedbackKey.Key;
                }
            }
            else if (isCreateSurvey)
            {
                feedback = GetFeedbackEntityFromPayload(answer, trace);
            }

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
                    if (answer.FieldId == PayloadSurveyFieldMapping.TourOperatorCode)
                        value = ReplaceTourOperator(value);
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
        /// To find booking number from survey payload
        /// </summary>
        /// <param name="answers"></param>
        /// <returns></returns>
        public static string GetBookingNumber(List<Answer> answers, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Trace service is null;");
            trace.Trace("Processing FindBookingNumber - start");
            var bookingNumber = string.Empty;
            if (answers != null && answers.Count > 0)
            {
                var booking = answers.Find(a => a.FieldId == PayloadSurveyFieldMapping.BookingNumber);              
                if (booking != null && !string.IsNullOrWhiteSpace(booking.LiteralValue))
                {
                    bookingNumber = booking.LiteralValue;                    
                }
            }
            trace.Trace("Processing FindBookingNumber - end");
            return bookingNumber;
        }

        /// <summary>
        /// To find source market from survey payload
        /// </summary>
        /// <param name="answers"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        public static string GetSourceMarket(List<Answer> answers, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Trace service is null;");
            trace.Trace("Processing FindSourceMarket - start");
            var sourceMarketName = string.Empty;
            if (answers != null && answers.Count > 0)
            {  
                var sourceMarket = answers.Find(a => a.FieldId == PayloadSurveyFieldMapping.SourceMarket);
                if (sourceMarket != null && !string.IsNullOrWhiteSpace(sourceMarket.LiteralValue))
                {
                    sourceMarketName = sourceMarket.LiteralValue;
                }
            }
            trace.Trace("Processing FindSourceMarket - end");
            return sourceMarketName;
        }

        /// <summary>
        /// To find tour operator from survey payload
        /// </summary>
        /// <param name="answers"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        public static string GetTourOperator(List<Answer> answers, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Trace service is null;");
            trace.Trace("Processing FindTourOperator - start");
            var tourOperatorCode = string.Empty;
            if (answers != null && answers.Count > 0)
            {
                var tourOperator = answers.Find(a => a.FieldId == PayloadSurveyFieldMapping.TourOperatorCode);
                if (tourOperator != null && !string.IsNullOrWhiteSpace(tourOperator.LiteralValue))
                {
                    tourOperatorCode = tourOperator.LiteralValue;
                }
            }
            tourOperatorCode = ReplaceTourOperator(tourOperatorCode);
            trace.Trace("Processing FindTourOperator - end");
            return tourOperatorCode;
        }

        /// <summary>
        /// To replace tour operator code to 'TCUK' if the value is 'UKI1'
        /// </summary>
        /// <param name="tourOperatorCode"></param>
        /// <returns></returns>
        private static string ReplaceTourOperator(string tourOperatorCode)
        {
            if (string.Equals(tourOperatorCode, General.TourOperatorCodeToReplace, StringComparison.OrdinalIgnoreCase))
                tourOperatorCode = General.ReplacedTourOperatorCode;
            return tourOperatorCode;
        }

        /// <summary>
        /// To find brand from survey payload
        /// </summary>
        /// <param name="answers"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        public static string GetBrand(List<Answer> answers, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Trace service is null;");
            trace.Trace("Processing FindBrand - start");
            var brandName = string.Empty;
            if (answers != null && answers.Count > 0)
            {
                var brand = answers.Find(a => a.FieldId == PayloadSurveyFieldMapping.Brand);
                if (brand != null && !string.IsNullOrWhiteSpace(brand.LiteralValue))
                {
                    brandName = brand.LiteralValue;
                }
            }
            trace.Trace("Processing FindBrand - end");
            return brandName;
        }        
    }
}
