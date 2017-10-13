using System;

namespace Tc.Crm.Plugins.Merge.Models
{
    public sealed class EntityCacheMessage : EntityModel
    {
        public EntityCacheMessage(Guid id) : base(id)
        {
        }
        public string OutcomeId { get; set; }

        public Guid EntityCacheId { get; set; }
    }
}