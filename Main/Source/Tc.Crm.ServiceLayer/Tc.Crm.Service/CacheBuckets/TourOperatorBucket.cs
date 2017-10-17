using System.Collections.Generic;
using Tc.Crm.Service.Models;
using Tc.Crm.Service.Services;

namespace Tc.Crm.Service.CacheBuckets
{
	public class TourOperatorBucket : ReferenceBucket<TourOperator>
	{
		public TourOperatorBucket(ICrmService crmService) : base(crmService) { }

		protected override IEnumerable<TourOperator> GetEntities()
		{
			return CrmService.GetTourOperators();
		}
	}
}