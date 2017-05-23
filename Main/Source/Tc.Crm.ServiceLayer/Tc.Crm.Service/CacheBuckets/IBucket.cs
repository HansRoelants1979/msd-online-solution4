using System.Collections.Generic;

namespace Tc.Crm.Service.CacheBuckets
{
    public interface IBucket
    {
        Dictionary<string, string> Items { get; set; }
        void Init();
        string GetBy(string code);
        void FillBucket();
    }
}
