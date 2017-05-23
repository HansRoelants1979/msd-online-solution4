using System;
using System.Collections.Generic;
using Tc.Crm.Service.Services;

namespace Tc.Crm.Service.CacheBuckets
{
    public class TourOperatorBucket:IBucket
    {
        ICrmService crmService;
        public TourOperatorBucket(ICrmService crmService)
        {
            this.crmService = crmService;
            //FillBucket();
        }

        public Dictionary<string, string> Items { get; set; }


        public void FillBucket()
        {
            Items = new Dictionary<string, string>();
            var tourOperators = crmService.GetTourOperators();
            if (tourOperators == null) return;
            foreach (var to in tourOperators)
            {
                Items.Add(to.Code, to.Id);
            }
        }

        public string GetBy(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return null;
            var id = string.Empty;
            if (Items.TryGetValue(code, out id))
                return id;
            return string.Empty;
        }

        public void Init()
        {
            throw new NotImplementedException();
        }
    }
}