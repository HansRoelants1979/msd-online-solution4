using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tc.Crm.Service.Services;

namespace Tc.Crm.Service.CacheBuckets
{
    public class CountryBucket:IBucket
    {
        ICrmService crmService;
        public CountryBucket(ICrmService crmService)
        {
            this.crmService = crmService;
            FillBucket();
        }

        public Dictionary<string, string> Items { get; set; }


        public void FillBucket()
        {
            Items = new Dictionary<string, string>();
            var countries = crmService.GetCountries();
            if (countries == null) return;
            foreach (var country in countries)
            {
                Items.Add(country.Code, country.Id);
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