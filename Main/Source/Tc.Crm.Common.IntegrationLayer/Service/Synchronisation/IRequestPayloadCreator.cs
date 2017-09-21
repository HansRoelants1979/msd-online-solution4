using Tc.Crm.Common.IntegrationLayer.Model;

namespace Tc.Crm.Common.IntegrationLayer.Service.Synchronisation
{
    public interface IRequestPayloadCreator
    {
        /// <summary>
        /// Get payload for syncronisation service call
        /// </summary>
        /// <param name="model">Entity model to create payload for</param>
        /// <returns>json payload of call</returns>
        string GetPayload(EntityModel model);
    }
}