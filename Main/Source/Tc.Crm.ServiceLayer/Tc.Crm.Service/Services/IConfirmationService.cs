using System;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.Services
{
    public interface IConfirmationService
    {
        ConfirmationResponse ProcessResponse(Guid entityCacheMessageId, IntegrationLayerResponse ilResponse);
    }
}
