using System.Collections.Generic;
using Tc.Crm.Service.Models;
using Tc.Crm.Service.Services;

namespace Tc.Crm.Service.CacheBuckets
{
	public class BrandBucket : ReferenceBucket<Brand>
    {
        public BrandBucket(ICrmService crmService) : base(crmService) { }

        protected override IEnumerable<Brand> GetEntities()
        {
            return CrmService.GetBrands();
        }
    }
}