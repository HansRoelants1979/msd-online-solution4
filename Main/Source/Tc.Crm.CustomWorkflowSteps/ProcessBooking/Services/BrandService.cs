using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.CustomWorkflowSteps.ProcessBooking.Services
{
    public class BrandService
    {
        public static Dictionary<string, Guid> Brands;
        public static void Init()
        {
            if (Brands != null && Brands.Count == 28) return;
            Brands = new Dictionary<string, Guid>();
            Brands.Add("AIR", new Guid("f7929ff9-460a-e711-8108-3863bb34da28"));
            Brands.Add("AIR1", new Guid("f9929ff9-460a-e711-8108-3863bb34da28"));
            Brands.Add("AQT", new Guid("fb929ff9-460a-e711-8108-3863bb34da28"));
            Brands.Add("BUC", new Guid("fd929ff9-460a-e711-8108-3863bb34da28"));
            Brands.Add("CLU1", new Guid("ff929ff9-460a-e711-8108-3863bb34da28"));
            Brands.Add("CRS", new Guid("01939ff9-460a-e711-8108-3863bb34da28"));
            Brands.Add("FLX", new Guid("09939ff9-460a-e711-8108-3863bb34da28"));
            Brands.Add("H4U", new Guid("0b939ff9-460a-e711-8108-3863bb34da28"));
            Brands.Add("NEF", new Guid("13939ff9-460a-e711-8108-3863bb34da28"));
            Brands.Add("MEDH", new Guid("0f939ff9-460a-e711-8108-3863bb34da28"));
            Brands.Add("NVB", new Guid("19939ff9-460a-e711-8108-3863bb34da28"));
            Brands.Add("NVN", new Guid("1b939ff9-460a-e711-8108-3863bb34da28"));
            Brands.Add("CZE", new Guid("03939ff9-460a-e711-8108-3863bb34da28"));
            Brands.Add("NNS", new Guid("15939ff9-460a-e711-8108-3863bb34da28"));
            Brands.Add("NPL", new Guid("17939ff9-460a-e711-8108-3863bb34da28"));
            Brands.Add("NEC", new Guid("11939ff9-460a-e711-8108-3863bb34da28"));
            Brands.Add("HUN", new Guid("0d939ff9-460a-e711-8108-3863bb34da28"));
            Brands.Add("OGE", new Guid("1f939ff9-460a-e711-8108-3863bb34da28"));
            Brands.Add("OTO", new Guid("21939ff9-460a-e711-8108-3863bb34da28"));
            Brands.Add("PEG", new Guid("23939ff9-460a-e711-8108-3863bb34da28"));
            Brands.Add("TCU1", new Guid("29939ff9-460a-e711-8108-3863bb34da28"));
            Brands.Add("OES", new Guid("1d939ff9-460a-e711-8108-3863bb34da28"));
            Brands.Add("ESC", new Guid("05939ff9-460a-e711-8108-3863bb34da28"));
            Brands.Add("TOC", new Guid("2b939ff9-460a-e711-8108-3863bb34da28"));
            Brands.Add("TCS", new Guid("27939ff9-460a-e711-8108-3863bb34da28"));
            Brands.Add("SIG1", new Guid("25939ff9-460a-e711-8108-3863bb34da28"));
            Brands.Add("FLO", new Guid("07939ff9-460a-e711-8108-3863bb34da28"));
            Brands.Add("VUN", new Guid("2d939ff9-460a-e711-8108-3863bb34da28"));


        }

        public static Guid GetBy(string brandCode)
        {
            var id = Guid.Empty;
            if (Brands.TryGetValue(brandCode, out id))
                return id;
            return Guid.Empty;
        }
    }
}
