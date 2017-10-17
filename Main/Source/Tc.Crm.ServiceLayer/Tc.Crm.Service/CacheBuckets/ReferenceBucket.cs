using System.Collections.Generic;
using Tc.Crm.Service.Services;

namespace Tc.Crm.Service.CacheBuckets
{
    public abstract class ReferenceBucket<T> : IReferenceBucket<T> where T : ReferenceBucketModel
    {
        protected ICrmService CrmService { get; }

        public Dictionary<string, T> Items { get; set; }

        protected abstract IEnumerable<T> GetEntities();

	    protected ReferenceBucket(ICrmService crmService)
	    {
		    CrmService = crmService;
		    FillBucket();
	    }

		public T GetBy(string code)
        {
	        T result = null;

	        if (!string.IsNullOrEmpty(code))
	        {
		        Items.TryGetValue(code, out result);
	        }

	        return result;
        }

        public void FillBucket()
        {
            Items = new Dictionary<string, T>();

            var entities = GetEntities();

	        if (entities == null)
	        {
		        return;
	        }

            foreach (var entity in entities)
            {
                Items.Add(entity.Code, entity);
            }
        }
	}
}