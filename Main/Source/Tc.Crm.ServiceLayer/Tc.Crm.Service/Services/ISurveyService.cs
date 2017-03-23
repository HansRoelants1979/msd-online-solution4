using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.Services
{
    public interface ISurveyService
    {
        SurveyReturnResponse ProcessSurvey(string surveyData, ICrmService crmService);
    }
}
