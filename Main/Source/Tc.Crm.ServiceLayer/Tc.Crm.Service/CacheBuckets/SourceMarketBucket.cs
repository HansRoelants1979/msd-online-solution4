using System.Collections.Generic;
using Tc.Crm.Service.Models;
using Tc.Crm.Service.Services;

namespace Tc.Crm.Service.CacheBuckets
{
	public class SourceMarketBucket : ReferenceBucket<SourceMarket>
	{
		public SourceMarketBucket(ICrmService crmService) : base(crmService) { }

		protected override IEnumerable<SourceMarket> GetEntities()
		{
			return CrmService.GetSourceMarkets();
		}
	}
}