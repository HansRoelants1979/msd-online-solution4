using System;
using System.Collections.Generic;
using Tc.Crm.Service.Models;
using Tc.Crm.Service.Services;

namespace Tc.Crm.Service.CacheBuckets
{
    public class HotelBucket:IHotelBucket
    {
        ICrmService crmService;
        public HotelBucket(ICrmService crmService)
        {
            this.crmService = crmService;
            //FillBucket();
        }

        public Dictionary<string, Hotel> Items { get; set; }


        public void FillBucket()
        {
            Items = new Dictionary<string, Hotel>();
            var hotels = crmService.GetHotels();
            if (hotels == null) return;
            foreach (var h in hotels)
            {
                Items.Add(h.Code, h);
            }
        }

        public Hotel GetBy(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return null;
            Hotel h = null;
            if (Items.TryGetValue(code, out h))
                return h;
            return null;
        }

        public void Init()
        {
            throw new NotImplementedException();
        }
    }
}