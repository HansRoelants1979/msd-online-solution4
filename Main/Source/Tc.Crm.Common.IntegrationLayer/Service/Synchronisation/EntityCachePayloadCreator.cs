using Newtonsoft.Json;
using System;
using Tc.Crm.Common.IntegrationLayer.Model;

namespace Tc.Crm.Common.IntegrationLayer.Service.Synchronisation
{
    public abstract class EntityCachePayloadCreator : IRequestPayloadCreator
    {
        protected abstract IEntityCacheMapper Mapper { get; }
        public virtual string GetPayload(string recordId, EntityModel model)
        {
            var settings = new JsonSerializerSettings()
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
            var data = Mapper.Map(recordId, model);
            var payload = JsonConvert.SerializeObject(data, settings);
            return payload;
        }
    }
}
