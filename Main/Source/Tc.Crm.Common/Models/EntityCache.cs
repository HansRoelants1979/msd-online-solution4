using Entities = Tc.Crm.Common.Constants;

namespace Tc.Crm.Common.Models
{
    public class EntityCache : EntityModel
    {
        public string Data { get; set; }
        public string SourceMarket { get; set; }
        public string RecordId { get; set; }
        public string Type { get; set; }
        public int? Operation { get; set; }

        public override string EntityName
        {
            get
            {
                return Entities.EntityName.EntityCache;
            }
        }
    }
}
