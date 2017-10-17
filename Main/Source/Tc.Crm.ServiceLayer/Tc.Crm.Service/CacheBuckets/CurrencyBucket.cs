using System.Collections.Generic;
using Tc.Crm.Service.Models;
using Tc.Crm.Service.Services;

namespace Tc.Crm.Service.CacheBuckets
{
	public class CurrencyBucket : ReferenceBucket<Currency>
	{
		public CurrencyBucket(ICrmService crmService) : base(crmService) { }

		protected override IEnumerable<Currency> GetEntities()
		{
			return CrmService.GetCurrencies();
		}
	}
}