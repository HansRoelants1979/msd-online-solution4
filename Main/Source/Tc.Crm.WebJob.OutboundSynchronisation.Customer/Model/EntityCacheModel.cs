using System;


namespace Tc.Crm.OutboundSynchronisation.Customer.Model
{
    public class EntityCacheModel
    {
        public Guid EntityCacheId { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }
        public string SourceMarket { get; set; }
        public string RecordId { get; set; }
        public string Type { get; set; }
        public int? Operation { get; set; }
    }

    
}
