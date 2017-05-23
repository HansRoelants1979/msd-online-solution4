using System.Collections.Generic;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.CacheBuckets
{
    public interface ISourceMarketBucket
    {
        Dictionary<string, SourceMarket> Items { get; set; }
        void Init();
        SourceMarket GetBy(string code);
        void FillBucket();
    }
}
