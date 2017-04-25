using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.Services
{
    public interface ISurveyService
    {
        string ProcessSurvey(string surveyData, ICrmService crmService);

        string ProcessResponse(SurveyReturnResponse response);
    }
}
