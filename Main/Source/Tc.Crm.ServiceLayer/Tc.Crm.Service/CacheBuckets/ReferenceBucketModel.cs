using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tc.Crm.Service.CacheBuckets
{
	public class ReferenceBucketModel : IReferenceBucketModel
	{
		public string Id { get; set; }

		public string Code { get; set; }
	}
}