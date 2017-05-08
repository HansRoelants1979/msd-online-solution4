using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tc.Crm.Service.CacheBuckets;

namespace Tc.Crm.Service.Services
{

    public class CachingService:ICachingService
    {
        BrandBucket brandBucket;
        CountryBucket countryBucket;
        CurrencyBucket currencyBucket;
        GatewayBucket gatewayBucket;
        SourceMarketBucket sourceMarketBucket;
        TourOperatorBucket tourOperatorBucket;
        HotelBucket hotelBucket;

        public CachingService(BrandBucket brandBucket
                                , CountryBucket countryBucket
                                , CurrencyBucket currencyBucket
                                , GatewayBucket gatewayBucket
                                , SourceMarketBucket sourceMarketBucket
                                , TourOperatorBucket tourOperatorBucket
                                , HotelBucket hotelBucket)
        {
            this.brandBucket = brandBucket;
            this.countryBucket = countryBucket;
            this.currencyBucket = currencyBucket;
            this.gatewayBucket = gatewayBucket;
            this.sourceMarketBucket = sourceMarketBucket;
            this.tourOperatorBucket = tourOperatorBucket;
            this.hotelBucket = hotelBucket;
        }

        public void Cache(string key)
        {
            if (key.Equals("BRAND", StringComparison.OrdinalIgnoreCase))
                this.brandBucket.FillBucket();
            else if (key.Equals("COUNTRY", StringComparison.OrdinalIgnoreCase))
                this.countryBucket.FillBucket();
            else if (key.Equals("CURRENCY", StringComparison.OrdinalIgnoreCase))
                this.currencyBucket.FillBucket();
            else if (key.Equals("GATEWAY", StringComparison.OrdinalIgnoreCase))
                this.gatewayBucket.FillBucket();
            else if (key.Equals("SOURCEMARKET", StringComparison.OrdinalIgnoreCase))
                this.sourceMarketBucket.FillBucket();
            else if (key.Equals("TOUROPERATOR", StringComparison.OrdinalIgnoreCase))
                this.tourOperatorBucket.FillBucket();
            else if (key.Equals("HOTEL", StringComparison.OrdinalIgnoreCase))
                this.hotelBucket.FillBucket();
        }
    }
}