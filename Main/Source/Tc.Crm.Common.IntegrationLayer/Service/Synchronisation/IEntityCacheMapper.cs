using Tc.Crm.Common.IntegrationLayer.Model;

namespace Tc.Crm.Common.IntegrationLayer.Service.Synchronisation
{
    public interface IEntityCacheMapper
    {
        /// <summary>
        /// Converts EntityModel to schema object
        /// </summary>
        /// <param name="model">EntityModel object to convert</param>
        /// <returns>Object of schema type</returns>
        /// <summary/>
        object Map(EntityModel model);
    }
}