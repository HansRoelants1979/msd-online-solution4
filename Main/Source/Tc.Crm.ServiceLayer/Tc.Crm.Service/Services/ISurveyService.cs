using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.Service.Models;
using InMoment.WebService.Rest.Data;

namespace Tc.Crm.Service.Services
{
    public interface ISurveyService
    {
        SurveyResponse ProcessSurvey(IList<Response> response);
    }
}
