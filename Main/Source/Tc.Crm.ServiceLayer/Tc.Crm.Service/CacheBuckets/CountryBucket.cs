using System.Collections.Generic;
using Tc.Crm.Service.Models;
using Tc.Crm.Service.Services;

namespace Tc.Crm.Service.CacheBuckets
{
	public class CountryBucket : ReferenceBucket<Country>
	{
		public CountryBucket(ICrmService crmService) : base(crmService) { }

		protected override IEnumerable<Country> GetEntities()
		{
			return CrmService.GetCountries();
		}
	}
}