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

        public ConfirmationResponse ProcessResponse(IntegrationLayerResponse ilResponse)
        {            
            try
            {
                var entityCacheId = Guid.Empty;
                if (ilResponse == null) throw new ArgumentNullException(Constants.Parameters.DataJson);
                if (string.IsNullOrWhiteSpace(ilResponse.CorrelationId)) return new ConfirmationResponse { Message = Messages.CorrelationIdWasMissing, StatusCode = System.Net.HttpStatusCode.BadRequest };
                if (crmService == null) throw new ArgumentNullException(Constants.Parameters.CrmService);
                var successStatus = new List<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.Accepted, HttpStatusCode.NonAuthoritativeInformation,
                                                               HttpStatusCode.NoContent, HttpStatusCode.ResetContent, HttpStatusCode.PartialContent };
                if (successStatus.Contains(ilResponse.SourceSystemStatusCode))
                {
                    entityCacheId = crmService.ProcessEntityCacheMessage(Guid.Parse(ilResponse.CorrelationId), ilResponse.SourceSystemEntityID, Status.Inactive, EntityCacheMessageStatusReason.EndtoEndSuccess);
                    if (entityCacheId != Guid.Empty)
                        crmService.ProcessEntityCache(entityCacheId, Status.Inactive, EntityCacheStatusReason.Succeeded);
                }
                else
                {
                    entityCacheId = crmService.ProcessEntityCacheMessage(Guid.Parse(ilResponse.CorrelationId), ilResponse.SourceSystemEntityID, Status.Inactive, EntityCacheMessageStatusReason.Failed, PrepareEntityCacheMessageNotes(ilResponse));
                }
                if (entityCacheId != Guid.Empty)
                    return new ConfirmationResponse { StatusCode = System.Net.HttpStatusCode.OK, Message = string.Empty };
                else
                    return new ConfirmationResponse { Message = Messages.FailedToUpdateEntityCacheMessage, StatusCode = System.Net.HttpStatusCode.BadRequest };

            }
            catch(Exception ex)
            {
                Trace.TraceError("Unexpected error occured at ProcessResponse::Message:{0}||Trace:{1}", ex.Message, ex.StackTrace.ToString());
                return new ConfirmationResponse { Message = Messages.FailedToUpdateEntityCacheMessage, StatusCode = System.Net.HttpStatusCode.GatewayTimeout };
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