using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.CustomWorkflowSteps.ProcessBooking.Services
{
    public class TourOperatorService
    {
        public static Dictionary<string, Guid> TourOperators;
        public static void Init()
        {
            if (TourOperators != null && TourOperators.Count == 23) return;
            TourOperators = new Dictionary<string, Guid>();
            TourOperators.Add("09", new Guid("ce91c676-460a-e711-8108-3863bb34da28"));
            TourOperators.Add("22", new Guid("dc91c676-460a-e711-8108-3863bb34da28"));
            TourOperators.Add("13", new Guid("d491c676-460a-e711-8108-3863bb34da28"));
            TourOperators.Add("49", new Guid("ec91c676-460a-e711-8108-3863bb34da28"));
            TourOperators.Add("29", new Guid("e091c676-460a-e711-8108-3863bb34da28"));
            TourOperators.Add("46", new Guid("e891c676-460a-e711-8108-3863bb34da28"));
            TourOperators.Add("48", new Guid("ea91c676-460a-e711-8108-3863bb34da28"));
            TourOperators.Add("45", new Guid("e691c676-460a-e711-8108-3863bb34da28"));
            TourOperators.Add("TCUK", new Guid("e491c676-460a-e711-8108-3863bb34da28"));
            TourOperators.Add("01", new Guid("c691c676-460a-e711-8108-3863bb34da28"));
            TourOperators.Add("70", new Guid("ee91c676-460a-e711-8108-3863bb34da28"));
            TourOperators.Add("71", new Guid("f091c676-460a-e711-8108-3863bb34da28"));
            TourOperators.Add("72", new Guid("f291c676-460a-e711-8108-3863bb34da28"));
            TourOperators.Add("10", new Guid("d091c676-460a-e711-8108-3863bb34da28"));
            TourOperators.Add("04", new Guid("cc91c676-460a-e711-8108-3863bb34da28"));
            TourOperators.Add("20", new Guid("da91c676-460a-e711-8108-3863bb34da28"));
            TourOperators.Add("28", new Guid("de91c676-460a-e711-8108-3863bb34da28"));
            TourOperators.Add("03", new Guid("ca91c676-460a-e711-8108-3863bb34da28"));
            TourOperators.Add("11", new Guid("d291c676-460a-e711-8108-3863bb34da28"));
            TourOperators.Add("02", new Guid("c891c676-460a-e711-8108-3863bb34da28"));
            TourOperators.Add("19", new Guid("d891c676-460a-e711-8108-3863bb34da28"));
            TourOperators.Add("34", new Guid("e291c676-460a-e711-8108-3863bb34da28"));
            TourOperators.Add("15", new Guid("d691c676-460a-e711-8108-3863bb34da28"));






        }

        public static Guid GetBy(string toCode)
        {
            var id = Guid.Empty;
            if (TourOperators.TryGetValue(toCode, out id))
                return id;
            return Guid.Empty;
        }
    }
}
