using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tc.Crm.Service.Models;
using InMoment.WebService.Rest.Data;


namespace Tc.Crm.Service.Services
{
    public class SurveyService : ISurveyService
    {
        public SurveyReturnResponse ProcessSurvey(string surveyData, ICrmService crmService)
        {
            if (string.IsNullOrWhiteSpace(surveyData)) throw new ArgumentNullException(Constants.Parameters.DataJson);
            if (crmService == null) throw new ArgumentNullException(Constants.Parameters.CrmService);

            var response = crmService.ExecuteActionForSurveyCreate(surveyData);
            if (response == null) throw new InvalidOperationException(Constants.Messages.ResponseNull);
            return new SurveyReturnResponse { Created = true };
        }
    }
}