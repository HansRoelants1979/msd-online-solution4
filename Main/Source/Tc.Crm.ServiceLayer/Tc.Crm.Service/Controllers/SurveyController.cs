using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tc.Crm.Service.Filters;
using Tc.Crm.Service.Services;
using System.Diagnostics;
using InMoment.WebService.Rest.Data;
using Newtonsoft.Json;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.Controllers
{
    [RequireHttps]
    public class SurveyController : ApiController
    {
        ISurveyService surveyService;
        ICrmService crmService;
        public SurveyController(ISurveyService surveyService, ICrmService crmService)
        {
            this.surveyService = surveyService;
            this.crmService = crmService;
        }

        
        [Route("api/v1/survey/create")]
        [Route("api/survey/create")]
        [HttpPut]
        [JsonWebTokenAuthorize]
        public HttpResponseMessage Create(IList<SurveyResponse> survey)
        {
            try
            {
                if (survey == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Constants.Messages.BookingDataPassedIsNullOrCouldNotBeParsed);
                }
                if(survey.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Constants.Messages.BookingDataPassedIsNullOrCouldNotBeParsed);
                }
                var jsonData = JsonConvert.SerializeObject(survey);
                var response = surveyService.ProcessSurvey(jsonData, crmService);
                if (response.Created == true)
                    return Request.CreateResponse(HttpStatusCode.Created);
                else
                    return Request.CreateResponse(HttpStatusCode.GatewayTimeout, Constants.Messages.FailedtoCreateSurvey);                

            }
            catch (Exception ex)
            {
                Trace.TraceError("Unexpected error Survey.Create::Message:{0}||Trace:{1}", ex.Message, ex.StackTrace.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
    }
}
