using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tc.Crm.Service.Models
{
    public class SurveyReturnResponse
    {
        public List<FailedSurvey> FailedSurveys { get; set; }
    }

    public class FailedSurvey
    {
        public string SurveyId { get; set; }
        public string Exception { get; set; }
    }
}