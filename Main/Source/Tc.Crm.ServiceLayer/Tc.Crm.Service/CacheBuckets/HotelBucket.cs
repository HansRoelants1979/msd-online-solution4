using System.Collections.Generic;
using Tc.Crm.Service.Models;
using Tc.Crm.Service.Services;

namespace Tc.Crm.Service.CacheBuckets
{
	public class HotelBucket : ReferenceBucket<Hotel>
	{
		public HotelBucket(ICrmService crmService) : base(crmService) { }

		protected override IEnumerable<Hotel> GetEntities()
		{
			return CrmService.GetHotels();
		}
	}
}