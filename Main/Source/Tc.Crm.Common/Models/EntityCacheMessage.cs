using System;
using Entities = Tc.Crm.Common.Constants;

namespace Tc.Crm.Common.Models
{
    public class EntityCacheMessage : EntityModel
    {
        public string Notes { get; set; }

        public string OutcomeId { get; set; }

        public Guid EntityCacheId { get; set; }

        public override string EntityName
        {
            get
            {
                return Entities.EntityName.EntityCacheMessage;
            }
        }
    }
}
