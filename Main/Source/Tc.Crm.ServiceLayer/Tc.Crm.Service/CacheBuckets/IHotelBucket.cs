using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
