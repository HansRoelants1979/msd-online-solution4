using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.CustomWorkflowSteps.ProcessBooking.Services
{
    public class CurrencyService
    {
        public static Dictionary<string, Guid> Currencies;
        public static void Init()
        {
            if (Currencies != null && Currencies.Count == 3) return;
            Currencies = new Dictionary<string, Guid>();
            Currencies.Add("EUR", new Guid("ea8cea3b-87ef-e611-8106-3863bb340dd8"));
            Currencies.Add("GBP", new Guid("2ecffb4d-33c7-e611-80fe-3863bb34da28"));
            Currencies.Add("USD", new Guid("ca5cb28f-9d0f-e711-810a-3863bb340dd8"));
        }

        public static Guid GetBy(string currencyCode)
        {
            var id = Guid.Empty;
            if (Currencies.TryGetValue(currencyCode, out id))
                return id;
            return Guid.Empty;
        }
    }
}
