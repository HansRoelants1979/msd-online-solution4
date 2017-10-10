using System;
using Tc.Crm.Service.Models;
using System.Diagnostics;
using Tc.Crm.Service.Constants;
using Tc.Crm.Common;
using System.Text;
using System.Net;
using System.Collections.Generic;

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
                if (ilResponse == null) throw new ArgumentNullException(Constants.Parameters.DataJson);
				if (crmService == null) throw new ArgumentNullException(Constants.Parameters.CrmService);
				if (string.IsNullOrWhiteSpace(ilResponse.CorrelationId)) return new ConfirmationResponse { Message = Messages.MissingCorrelationId, StatusCode = HttpStatusCode.BadRequest };                
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
					if (isSuccess)
					{
						crmService.ActivateRelatedPendingEntityCache(entityCacheId);
					}
					crmService.ProcessEntityCache(entityCacheId, isSuccess ? Status.Inactive : Status.Active, isSuccess ? EntityCacheStatusReason.Succeeded : EntityCacheStatusReason.InProgress, isSuccess);
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