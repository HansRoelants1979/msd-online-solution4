using System;

namespace Tc.Crm.Plugins.Merge.Models
{
    public sealed class EntityCache : EntityModel
    {
        public EntityCache(Guid id) : base(id)
        {
        }
        public Guid RecordId { get; set; }

        public EntityCacheType Type { get; set; }
    }
}