using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
