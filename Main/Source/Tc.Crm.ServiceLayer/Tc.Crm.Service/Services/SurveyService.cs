using System;
using Tc.Crm.Service.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tc.Crm.Service.Services
{
    public class SurveyService : ISurveyService
    {
        public SurveyReturnResponse ProcessSurvey(string surveyData, ICrmService crmService)
        {
            if (string.IsNullOrWhiteSpace(surveyData)) throw new ArgumentNullException(Constants.Parameters.DataJson);
            if (crmService == null) throw new ArgumentNullException(Constants.Parameters.CrmService);
            Survey survey = new Survey();
            survey.Responses = (IList<SurveyResponse>)JsonConvert.DeserializeObject(surveyData, survey.Responses.GetType());
            var surveyJson = JsonConvert.SerializeObject(survey);
            try
            {
                var response = crmService.ExecuteActionForSurveyCreate(surveyJson);
                if (response == null) throw new InvalidOperationException(Constants.Messages.ResponseNull);
            }
            catch(Exception ex)
            {
                Trace.TraceError("Unexpected error occured at ProcessSurvey::Message:{0}||Trace:{1}", ex.Message, ex.StackTrace.ToString());
                return new SurveyReturnResponse { Created = false };
            }
            return new SurveyReturnResponse { Created = true };
        }
    }
}