using System;
using Tc.Crm.Service.Models;
using System.Diagnostics;
using Tc.Crm.Service.Constants;
using Tc.Crm.Common;
using System.Text;
using System.Net;
using System.Collections.Generic;
using Tc.Crm.Common.Constants.EntityRecords;
using System.Linq;

namespace Tc.Crm.Service.Services
{
    public class ConfirmationService : IConfirmationService
    {
        ICrmService crmService;
		public ConfirmationService(ICrmService crmService)
        {
            this.crmService = crmService;
		}

        public ConfirmationResponse ProcessResponse(Guid entityCacheMessageId, IntegrationLayerResponse ilResponse)
        {            
            try
            {
                var entityCacheId = Guid.Empty;
				if (entityCacheMessageId == Guid.Empty) throw new ArgumentNullException(Parameters.EntityCacheMessageId);
				if (ilResponse == null) throw new ArgumentNullException(Parameters.DataJson);
				if (crmService == null) throw new ArgumentNullException(Parameters.CrmService);
				var successStatus = new List<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.Accepted, HttpStatusCode.NonAuthoritativeInformation,
                                                               HttpStatusCode.NoContent, HttpStatusCode.ResetContent, HttpStatusCode.PartialContent };

				var isSuccess = successStatus.Contains(ilResponse.SourceSystemStatusCode);
				entityCacheId = crmService.ProcessEntityCacheMessage(entityCacheMessageId,
					ilResponse.SourceSystemEntityID, Status.Inactive,
					isSuccess ? EntityCacheMessageStatusReason.EndtoEndSuccess : EntityCacheMessageStatusReason.Failed,
					isSuccess ? null : PrepareEntityCacheMessageNotes(ilResponse));
				if (entityCacheId != Guid.Empty)
				{
					// activate pending entity cache before putting to success to elimitate possibility plugin will create active request in between
					Status status;
					EntityCacheStatusReason statusReason;
					DateTime? eligibleRetryTime = null;
					if (isSuccess)
					{
						status = Status.Inactive;
						statusReason = EntityCacheStatusReason.Succeeded;
						crmService.ActivateRelatedPendingEntityCache(entityCacheId);
					}
					else
					{
						var count = crmService.GetEntityCacheMessageCount(entityCacheId);
						var schedule = GetRetrySchedule();
						var hasMoreRetries = count <= schedule.Length;
						eligibleRetryTime = hasMoreRetries ? GetEligibleRetryTime(schedule, count - 1) : (DateTime?) null;						
						status = hasMoreRetries ? Status.Active : Status.Inactive;
						statusReason = hasMoreRetries ? EntityCacheStatusReason.InProgress : EntityCacheStatusReason.Failed;
					}
					crmService.ProcessEntityCache(entityCacheId, status, statusReason, isSuccess, eligibleRetryTime);
					return new ConfirmationResponse { StatusCode = HttpStatusCode.OK, Message = string.Empty };
				}
                return new ConfirmationResponse { Message = string.Format(Messages.MsdCorrelationIdDoesNotExist, entityCacheMessageId), StatusCode = HttpStatusCode.BadRequest };
            }
            catch(Exception ex)
            {
                Trace.TraceError("Unexpected error occured at ProcessResponse::Message:{0}||Trace:{1}", ex.Message, ex.StackTrace.ToString());
                return new ConfirmationResponse { Message = Messages.FailedToUpdateEntityCacheMessage, StatusCode = HttpStatusCode.GatewayTimeout };
            }            
        }

		private int[] GetRetrySchedule()
		{
			var config = crmService.GetConfiguration(Configuration.OutboundSynchronisationMaxRetries);
			return config.Split(',').Select(int.Parse).ToArray();
		}
		private DateTime GetEligibleRetryTime(int[] retryMinutes, int retry) => (retryMinutes.Length > 0 && retry < retryMinutes.Length) ? DateTime.UtcNow.AddMinutes(retryMinutes[retry]) : DateTime.UtcNow;

		private string PrepareEntityCacheMessageNotes(IntegrationLayerResponse ilResponse)
        {
            var notes = new StringBuilder();
            if (ilResponse == null) return notes.ToString();
            notes.AppendLine("SourceSystemStatusCode: " + ilResponse.SourceSystemStatusCode);
            if (!string.IsNullOrWhiteSpace(ilResponse.SourceSystemRequest)) notes.AppendLine("SourceSystemRequest: " + ilResponse.SourceSystemRequest);
            if (!string.IsNullOrWhiteSpace(ilResponse.SourceSystemResponse)) notes.AppendLine("SourceSystemResponse: " + ilResponse.SourceSystemResponse);
            return notes.ToString();
        }
    }
}