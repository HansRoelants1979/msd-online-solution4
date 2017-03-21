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
        public static Entity GetResponseEntityFromPayLoad(Response response)
        {
            if (response == null) throw new InvalidPluginExecutionException("Response in Json is null");
            var surveyResponse = new Entity(EntityName.SurveyResponse);
            surveyResponse[Attributes.SurveyResponse.ResponseId] = Convert.ToInt32(response.Id);

            if (response.SurveyId != null)
                surveyResponse[Attributes.SurveyResponse.SurveyId] = Convert.ToInt32(response.SurveyId);

            if (!string.IsNullOrWhiteSpace(response.SurveyName))
                surveyResponse[Attributes.SurveyResponse.Subject] = response.SurveyName;

            if (!string.IsNullOrWhiteSpace(response.SurveyDescription))
                surveyResponse[Attributes.SurveyResponse.SurveyDescription] = response.SurveyDescription;

            if (!string.IsNullOrWhiteSpace(response.Mode))
                surveyResponse[Attributes.SurveyResponse.Mode] = response.Mode;

            if (!string.IsNullOrWhiteSpace(response.BeginTime))
                surveyResponse[Attributes.SurveyResponse.BeginTime] = DateTime.Parse(response.BeginTime);

            surveyResponse[Attributes.SurveyResponse.ActivityAdditionalParams] = PrepareAdditionalParameters(response);

            return surveyResponse;
        }

        /// <summary>
        /// To prepare additional parameters from payload
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private static string PrepareAdditionalParameters(Response response)
        {
            var additionalParameters = new StringBuilder();
            additionalParameters.Append("Id: " + response.Id);
            if (response.SurveyGatewayId != null)
                additionalParameters.AppendLine("SurveyGatewayId: " + response.SurveyGatewayId);
            if (!string.IsNullOrWhiteSpace(response.SurveyGatewayName))
                additionalParameters.AppendLine("SurveyGatewayName: " + response.SurveyGatewayName);
            if (!string.IsNullOrWhiteSpace(response.SurveyGatewayAlias))
                additionalParameters.AppendLine("SurveyGatewayAlias: " + response.SurveyGatewayAlias);
            if (response.SurveyId != null)
                additionalParameters.AppendLine("SurveyId: " + response.SurveyId);
            if (!string.IsNullOrWhiteSpace(response.SurveyName))
                additionalParameters.AppendLine("SurveyName: " + response.SurveyName);
            if (!string.IsNullOrWhiteSpace(response.BeginTime))
                additionalParameters.AppendLine("BeginTime: " + response.BeginTime);
            if (!string.IsNullOrWhiteSpace(response.DateOfService))
                additionalParameters.AppendLine("DateOfService: " + response.DateOfService);
            if (!string.IsNullOrWhiteSpace(response.RedemptionCode))
                additionalParameters.AppendLine("RedemptionCode: " + response.RedemptionCode);
            additionalParameters.AppendLine("Minutes: " + response.Minutes);
            additionalParameters.AppendLine("Complete: " + response.Complete);
            additionalParameters.AppendLine("Read: " + response.Read);
            if (!string.IsNullOrWhiteSpace(response.IpAddress))
                additionalParameters.AppendLine("IpAddress: " + response.IpAddress);
            if (!string.IsNullOrWhiteSpace(response.CookieUID))
                additionalParameters.AppendLine("CookieUID: " + response.CookieUID);
            if (response.OfferId != null)
                additionalParameters.AppendLine("OfferId: " + response.OfferId);
            if (!string.IsNullOrWhiteSpace(response.OfferName))
                additionalParameters.AppendLine("OfferName: " + response.OfferName);
            if (!string.IsNullOrWhiteSpace(response.OfferCode))
                additionalParameters.AppendLine("OfferCode: " + response.OfferCode);
            if (response.UnitId != null)
                additionalParameters.AppendLine("UnitId: " + response.UnitId);
            if (!string.IsNullOrWhiteSpace(response.UnitExternalId))
                additionalParameters.AppendLine("UnitExternalId: " + response.UnitExternalId);
            if (!string.IsNullOrWhiteSpace(response.UnitName))
                additionalParameters.AppendLine("UnitName: " + response.UnitName);
            if (response.OrganizationId != null)
                additionalParameters.AppendLine("OrganizationId: " + response.OrganizationId);
            if (!string.IsNullOrWhiteSpace(response.OrganizationName))
                additionalParameters.AppendLine("OrganizationName: " + response.OrganizationName);
            if (!string.IsNullOrWhiteSpace(response.ExclusionReason))
                additionalParameters.AppendLine("ExclusionReason: " + response.ExclusionReason);
            if (!string.IsNullOrWhiteSpace(response.Mode))
                additionalParameters.AppendLine("Mode: " + response.Mode);
            if (!string.IsNullOrWhiteSpace(response.SurveyDescription))
                additionalParameters.AppendLine("SurveyDescription: " + response.SurveyDescription);

            return additionalParameters.ToString();
        }


    }
}
