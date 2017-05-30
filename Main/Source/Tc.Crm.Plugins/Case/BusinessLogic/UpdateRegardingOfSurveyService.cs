using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tc.Crm.Plugins.Case.BusinessLogic
{
    public class UpdateRegardingOfSurveyService
    {
        public IPluginExecutionContext context = null;
        public IOrganizationService service = null;
        public ITracingService trace = null;
        public string[] businessUnits;

        public UpdateRegardingOfSurveyService(IPluginExecutionContext context, ITracingService trace, IOrganizationService service)
        {
            this.context = context;
            this.service = service;
            this.trace = trace;
          
        }

        private bool IsContextValid()
        {
            if (!context.MessageName.Equals(Messages.Create, StringComparison.OrdinalIgnoreCase)) return false;
            if (context.Stage != (int)PluginStage.Postoperation) return false;
            if (context.PrimaryEntityName != Entities.Case) return false;            
            return true;
        }

        public void DoActionsOnCreateCase()
        {
            trace.Trace("DoActionsOnCreateCase - Start");
            if (!IsContextValid()) return;
            trace.Trace("Context is valid");
            if(context.InputParameters.Contains(InputParameters.Target) && context.InputParameters[InputParameters.Target] is Entity)
            {
                trace.Trace("Contains Input Parameters 'Target' as Entity");
                var Case = (Entity)context.InputParameters[InputParameters.Target];               
                if (!Case.Attributes.Contains(Attributes.Case.SurveyId)) return;
                if (string.IsNullOrWhiteSpace(Case.Attributes[Attributes.Case.SurveyId].ToString())) return;
                trace.Trace("Receieved Survey Id "+ Case.Attributes[Attributes.Case.SurveyId].ToString());
                var surveyId = Guid.Parse(Case.Attributes[Attributes.Case.SurveyId].ToString());
                UpdateRegardingOfSurveyResponse(surveyId, Case.Id);                
            }
            trace.Trace("DoActionsOnCreateCase - End");
        }

        private void UpdateRegardingOfSurveyResponse(Guid surveyId, Guid caseId)
        {
            trace.Trace("UpdateSurveyResponse - Start");
            var survey = new Entity(Entities.SurveyResponse);
            survey.Attributes[Attributes.SurveyResponse.Regarding] = new EntityReference(Entities.Case, caseId);
            survey.Id = surveyId;
            service.Update(survey);
            trace.Trace("UpdateSurveyResponse - End");
        }
    }
}
