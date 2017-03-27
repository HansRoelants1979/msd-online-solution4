using System.Text;
using Microsoft.Xrm.Sdk;
using Tc.Crm.CustomWorkflowSteps.ProcessSurvey.Models;
using System;

namespace Tc.Crm.CustomWorkflowSteps.ProcessSurvey.Services
{
    public static class SurveyResponseHelper
    {
        /// <summary>
        /// To prepare survey response entity from payload
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static Entity GetResponseEntityFromPayLoad(Response response, ITracingService trace)
        {
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
            var additionalParameters = new StringBuilder();
            additionalParameters.AppendLine("{");
            additionalParameters.AppendLine(FormatJson("Id", response.Id, true));
            if (response.SurveyGatewayId != null)
                additionalParameters.AppendLine(FormatJson("SurveyGatewayId", response.SurveyGatewayId, true));
            if (!string.IsNullOrWhiteSpace(response.SurveyGatewayName))
                additionalParameters.AppendLine(FormatJson("SurveyGatewayName",response.SurveyGatewayName));
            if (!string.IsNullOrWhiteSpace(response.SurveyGatewayAlias))
                additionalParameters.AppendLine(FormatJson("SurveyGatewayAlias", response.SurveyGatewayAlias));
            if (response.SurveyId != null)
                additionalParameters.AppendLine(FormatJson("SurveyId", response.SurveyId, true));
            if (!string.IsNullOrWhiteSpace(response.SurveyName))
                additionalParameters.AppendLine(FormatJson("SurveyName", response.SurveyName));
            if (!string.IsNullOrWhiteSpace(response.BeginTime))
                additionalParameters.AppendLine(FormatJson("BeginTime", response.BeginTime));
            if (!string.IsNullOrWhiteSpace(response.DateOfService))
                additionalParameters.AppendLine(FormatJson("DateOfService", response.DateOfService));
            if (!string.IsNullOrWhiteSpace(response.RedemptionCode))
                additionalParameters.AppendLine(FormatJson("RedemptionCode", response.RedemptionCode));
            additionalParameters.AppendLine(FormatJson("Minutes", response.Minutes, true));
            additionalParameters.AppendLine(FormatJson("Complete", response.Complete.ToString().ToLower(), true));
            additionalParameters.AppendLine(FormatJson("Read", response.Read.ToString().ToLower(), true));
            if (!string.IsNullOrWhiteSpace(response.IpAddress))
                additionalParameters.AppendLine(FormatJson("IpAddress", response.IpAddress));
            if (!string.IsNullOrWhiteSpace(response.CookieUID))
                additionalParameters.AppendLine(FormatJson("CookieUID", response.CookieUID));
            if (response.OfferId != null)
                additionalParameters.AppendLine(FormatJson("OfferId", response.OfferId, true));
            if (!string.IsNullOrWhiteSpace(response.OfferName))
                additionalParameters.AppendLine(FormatJson("OfferName", response.OfferName));
            if (!string.IsNullOrWhiteSpace(response.OfferCode))
                additionalParameters.AppendLine(FormatJson("OfferCode", response.OfferCode));
            if (response.UnitId != null)
                additionalParameters.AppendLine(FormatJson("UnitId", response.UnitId, true));
            if (!string.IsNullOrWhiteSpace(response.UnitExternalId))
                additionalParameters.AppendLine(FormatJson("UnitExternalId", response.UnitExternalId));
            if (!string.IsNullOrWhiteSpace(response.UnitName))
                additionalParameters.AppendLine(FormatJson("UnitName", response.UnitName));
            if (response.OrganizationId != null)
                additionalParameters.AppendLine(FormatJson("OrganizationId", response.OrganizationId, true));
            if (!string.IsNullOrWhiteSpace(response.OrganizationName))
                additionalParameters.AppendLine(FormatJson("OrganizationName", response.OrganizationName));
            if (!string.IsNullOrWhiteSpace(response.ExclusionReason))
                additionalParameters.AppendLine(FormatJson("ExclusionReason", response.ExclusionReason));
            if (!string.IsNullOrWhiteSpace(response.Mode))
                additionalParameters.AppendLine(FormatJson("Mode", response.Mode));
            if (!string.IsNullOrWhiteSpace(response.SurveyDescription))
                additionalParameters.AppendLine(FormatJson("SurveyDescription", response.SurveyDescription));            
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
