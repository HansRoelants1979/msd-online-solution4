using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;

namespace Tc.Crm.CustomWorkflowSteps.ProcessBooking.Services
{

    public class SourceMarketService
    {
        public static Dictionary<string, SourceMarket> SourceMarkets;

        public static void Init()
        {
            if (SourceMarkets != null && SourceMarkets.Count == 8) return;
            SourceMarkets = new Dictionary<string, SourceMarket>();
            SourceMarkets.Add("BE", new SourceMarket
            {
                BusinessUnitId = new Guid("34b957e0-51ff-e611-8100-3863bb354ff0")
                                                    ,
                SourceMarketId = new Guid("6a766a7d-410a-e711-8108-3863bb34da28")

            });
            SourceMarkets.Add("CZ", new SourceMarket
            {
                BusinessUnitId = new Guid("4f2c8aee-51ff-e611-8100-3863bb354ff0")
                                                   ,
                SourceMarketId = new Guid("8e766a7d-410a-e711-8108-3863bb34da28")
                
            });
            SourceMarkets.Add("FR", new SourceMarket
            {
                BusinessUnitId = new Guid("31b2fdd9-51ff-e611-8100-3863bb354ff0")
                                                   ,
                SourceMarketId = new Guid("a3766a7d-410a-e711-8108-3863bb34da28")
            });
            SourceMarkets.Add("DE", new SourceMarket
            {
                BusinessUnitId = new Guid("29b2fdd9-51ff-e611-8100-3863bb354ff0")
                                                   ,
                SourceMarketId = new Guid("90766a7d-410a-e711-8108-3863bb34da28")
            });
            SourceMarkets.Add("HU", new SourceMarket
            {
                BusinessUnitId = new Guid("e5dc79e8-51ff-e611-8100-3863bb354ff0")
                                                   ,
                SourceMarketId = new Guid("b5766a7d-410a-e711-8108-3863bb34da28")
            });
            SourceMarkets.Add("NL", new SourceMarket
            {
                BusinessUnitId = new Guid("3ab957e0-51ff-e611-8100-3863bb354ff0")
                                                   ,
                SourceMarketId = new Guid("03776a7d-410a-e711-8108-3863bb34da28")
            });
            SourceMarkets.Add("PL", new SourceMarket
            {
                BusinessUnitId = new Guid("dbdc79e8-51ff-e611-8100-3863bb354ff0")
                                                   ,
                SourceMarketId = new Guid("13776a7d-410a-e711-8108-3863bb34da28")
            });
            SourceMarkets.Add("GB", new SourceMarket
            {
                BusinessUnitId = new Guid("c1d1d6a9-7cf4-e611-80ff-3863bb354ff0")
                                                   ,
                SourceMarketId = new Guid("a5766a7d-410a-e711-8108-3863bb34da28")
            });
        }

        public static SourceMarket GetBy(string isoCode)
        {
            SourceMarket sourceMarket = null;
            if (SourceMarkets.TryGetValue(isoCode, out sourceMarket))
                return sourceMarket;
            return null;
        }
    }

    
}
