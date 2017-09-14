using Tc.Crm.Common.IntegrationLayer.Model;
using Tc.Crm.Common.IntegrationLayer.Service.Synchronisation;

namespace Tc.Crm.OutboundSynchronisation.CustomerTests
{
    public class TestRequestPayloadCreator : IRequestPayloadCreator
    {
        public string GetPayload(EntityModel model)
        {
            return string.Empty;
        }
    }
}