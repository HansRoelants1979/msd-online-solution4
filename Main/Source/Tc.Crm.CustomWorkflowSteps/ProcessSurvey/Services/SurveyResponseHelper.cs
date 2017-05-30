using System.Text;
using Microsoft.Xrm.Sdk;
using Tc.Crm.CustomWorkflowSteps.ProcessSurvey.Models;
using System;
using System.Reflection;
using System.Linq;

namespace Tc.Crm.CustomWorkflowSteps.ProcessSurvey.Services
{
    public static class SurveyResponseHelper
    {
        /// <summary>
        /// To prepare survey response entity from payload
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static Entity GetResponseEntityFromPayload(Response response, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("trace is null.");
            trace.Trace("Processing GetResponseEntityFromPayLoad - start");
            if (response == null) throw new InvalidPluginExecutionException("Response in Json is null");
            var surveyResponse = new Entity(EntityName.SurveyResponse);
            surveyResponse[Attributes.SurveyResponse.ResponseId] = response.Id.ToString();

            if (response.SurveyId != null)
                surveyResponse[Attributes.SurveyResponse.SurveyId] = response.SurveyId.Value.ToString();

            if (!string.IsNullOrWhiteSpace(response.SurveyName))
                surveyResponse[Attributes.SurveyResponse.Subject] = response.SurveyName;

            if (!string.IsNullOrWhiteSpace(response.SurveyDescription))
                surveyResponse[Attributes.SurveyResponse.SurveyDescription] = response.SurveyDescription;

            if (!string.IsNullOrWhiteSpace(response.Mode))
                surveyResponse[Attributes.SurveyResponse.Mode] = response.Mode;

            if (!string.IsNullOrWhiteSpace(response.BeginTime))
                surveyResponse[Attributes.SurveyResponse.BeginTime] = DateTime.Parse(response.BeginTime);

            var customerFirstName = ContactHelper.GetFirstName(response.Contact, trace);
            if (!string.IsNullOrWhiteSpace(customerFirstName))
                surveyResponse[Attributes.SurveyResponse.CustomerFirstName] = customerFirstName;

            var customerLastName = ContactHelper.GetLastName(response.Contact, trace);
            if (!string.IsNullOrWhiteSpace(customerLastName))
                surveyResponse[Attributes.SurveyResponse.CustomerLastName] = customerLastName;

            var customerEmail = ContactHelper.GetEmail(response.Contact, trace);
            if (!string.IsNullOrWhiteSpace(customerEmail))
                surveyResponse[Attributes.SurveyResponse.CustomerEmail] = customerEmail;

            var customerPhone = ContactHelper.GetPhone(response.Contact, trace);
            if (!string.IsNullOrWhiteSpace(customerPhone))
                surveyResponse[Attributes.SurveyResponse.CustomerPhone] = customerPhone;

            surveyResponse[Attributes.SurveyResponse.ActivityAdditionalParams] = PrepareAdditionalParameters(response, trace);
            trace.Trace("Processing GetResponseEntityFromPayLoad - end");
            return surveyResponse;
        }

        /// <summary>
        /// To prepare additional parameters from payload
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private static string PrepareAdditionalParameters(Response response, ITracingService trace)
        {
            trace.Trace("Processing PrepareAdditionalParameters - start");
            var skipProperties = new string[] { "Contact", "Answers" };
            var propertyName = string.Empty;
            object propertyValue = null;
            var additionalParameters = new StringBuilder();
            additionalParameters.AppendLine("{");
            foreach (PropertyInfo property in response.GetType().GetProperties())
            {
                if (skipProperties.Contains(property.Name, StringComparer.OrdinalIgnoreCase)) continue;
                propertyValue = property.GetValue(response);
                if (propertyValue == null || string.IsNullOrWhiteSpace(propertyValue.ToString())) continue;
                propertyName = property.Name.Substring(0, 1).ToLower() + property.Name.Substring(1);
                if (property.PropertyType == typeof(Boolean)) propertyValue = propertyValue.ToString().ToLower();
                additionalParameters.AppendLine(FormatJson(propertyName, propertyValue, !(property.PropertyType == typeof(string))));                
            }
            additionalParameters.AppendLine("}");
            trace.Trace("Processing PrepareAdditionalParameters - end");
            return additionalParameters.ToString().Remove(additionalParameters.ToString().Length - 6, 1);
        }

        /// <summary>
        /// To format as JSON key value
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        /// <param name="isNotStringType"></param>
        /// <returns></returns>
        private static string FormatJson(string fieldName, object fieldValue, bool isNotStringType)
        {
            var format = string.Empty;
            if (!isNotStringType)
                format = "\"" + fieldName + "\": \"" + fieldValue + "\",";
            else
                format = "\"" + fieldName + "\": " + fieldValue + ",";

            return format;
        }

        /// <summary>
        /// To format as JSON key value
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        private static string FormatJson(string fieldName, object fieldValue)
        {
            return FormatJson(fieldName, fieldValue, false);
        }


    }
}
