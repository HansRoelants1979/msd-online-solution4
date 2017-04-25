using System;
using Tc.Crm.Service.Models;
using System.Diagnostics;
using System.Text;


namespace Tc.Crm.Service.Services
{
    public class SurveyService : ISurveyService
    {
        public string ProcessSurvey(string surveyData, ICrmService crmService)
        {
            var failedSurveys = string.Empty;
            if (string.IsNullOrWhiteSpace(surveyData)) throw new ArgumentNullException(Constants.Parameters.DataJson);
            if (crmService == null) throw new ArgumentNullException(Constants.Parameters.CrmService);
            try
            {
                var response = crmService.ExecuteActionForSurveyCreate(surveyData);
                if (response == null) throw new InvalidOperationException(Constants.Messages.ResponseNull);
                failedSurveys = ProcessResponse(response);
            }
            catch(Exception ex)
            {
                Trace.TraceError("Unexpected error occured at ProcessSurvey::Message:{0}||Trace:{1}", ex.Message, ex.StackTrace.ToString());
                return Constants.General.Error;
            }
            return failedSurveys;
        }

        public string ProcessResponse(SurveyReturnResponse response)
        {
            var failedSurveys = new StringBuilder();
            var failedSurveysList = response.FailedSurveys;
            if(failedSurveysList != null && failedSurveysList.Count > 0)
            {
                for(int i=0;i<failedSurveysList.Count;i++)
                {
                    if (failedSurveys.Length == 0)
                        failedSurveys.Append(failedSurveysList[i].SurveyId);
                    else
                        failedSurveys.Append("," + failedSurveysList[i].SurveyId);

                    Trace.TraceError("Error occured in custom action for Survey:: {0} || Exception:: {1}", failedSurveysList[i].SurveyId, failedSurveysList[i].Exception);
                }
            }
            return failedSurveys.ToString();
        }
    }

   
}