using System.Collections.Generic;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.CacheBuckets
{
    public interface IHotelBucket
    {
        Dictionary<string, Hotel> Items { get; set; }
        void Init();
        Hotel GetBy(string code);
        void FillBucket();
    }
}
