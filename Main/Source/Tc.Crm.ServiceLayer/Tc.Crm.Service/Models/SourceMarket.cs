using Tc.Crm.Service.CacheBuckets;

namespace Tc.Crm.Service.Models
{
    public class SourceMarket : ReferenceBucketModel
    {
        public string TeamId { get; set; }

        public string BusinessUnitId { get; set; }
    }
}