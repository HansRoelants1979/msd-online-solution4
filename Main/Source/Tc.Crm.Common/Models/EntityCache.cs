using Entities = Tc.Crm.Common.Constants;

namespace Tc.Crm.Common.Models
{
	public class EntityCache : EntityModel
    {
        public string Data { get; set; }
        public string SourceSystemId { get; set; }
        public string SourceMarket { get; set; }
        public string RecordId { get; set; }
        public string Type { get; set; }
        public EntityCacheOperation Operation { get; set; }
        public override string EntityName => Entities.EntityName.EntityCache;
		public EntityCacheStatusReason StatusReason { get; set; }
		public int RequestsCount { get; set; }
    }
}