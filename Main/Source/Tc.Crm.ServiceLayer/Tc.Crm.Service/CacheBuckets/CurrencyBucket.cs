using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tc.Crm.Service.Services;

namespace Tc.Crm.Service.CacheBuckets
{
    public class CurrencyBucket:IBucket
    {
        ICrmService crmService;
        public CurrencyBucket(ICrmService crmService)
        {
            this.crmService = crmService;
            //FillBucket();
        }

        public Dictionary<string, string> Items { get; set; }
        

        public void FillBucket()
        {
            Items = new Dictionary<string, string>();
            var currencies = crmService.GetCurrencies();
            if (currencies == null) return;
            foreach (var currency in currencies)
            {
                Items.Add(currency.Code, currency.Id);
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