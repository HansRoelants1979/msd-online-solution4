using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
