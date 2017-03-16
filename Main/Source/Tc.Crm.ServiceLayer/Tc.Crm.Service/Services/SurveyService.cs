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
        public SurveyResponse ProcessSurvey(IList<Response> response)
        {

            return new SurveyResponse { Created=true };
        }
    }
}