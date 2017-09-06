using System;


namespace Tc.Crm.OutboundSynchronisation.Customer.Model
{
    public class EntityCacheMessageModel
    {
        public string Name { get; set; }

        public string Notes { get; set; }

        public string OutcomeId { get; set; }

        public Guid EntityCacheId { get; set; }
    }
}
