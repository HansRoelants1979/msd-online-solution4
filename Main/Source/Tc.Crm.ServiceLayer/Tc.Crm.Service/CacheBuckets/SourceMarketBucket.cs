using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tc.Crm.Service.Models;
using Tc.Crm.Service.Services;

namespace Tc.Crm.Service.CacheBuckets
{
    public class SourceMarketBucket:ISourceMarketBucket
    {
        ICrmService crmService;
        public SourceMarketBucket(ICrmService crmService)
        {
            this.crmService = crmService;
            //FillBucket();
        }

        public Dictionary<string, SourceMarket> Items { get; set; }


        public void FillBucket()
        {
            Items = new Dictionary<string, SourceMarket>();
            var sourceMarkets = crmService.GetSourceMarkets();
            if (sourceMarkets == null) return;
            foreach (var sm in sourceMarkets)
            {
                Items.Add(sm.Code, sm);
            }
        }

        public SourceMarket GetBy(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return null; 
            SourceMarket sm = null;
            if (Items.TryGetValue(code, out sm))
                return sm;
            return null;
        }

        public void Init()
        {
            throw new NotImplementedException();
        }
    }
}