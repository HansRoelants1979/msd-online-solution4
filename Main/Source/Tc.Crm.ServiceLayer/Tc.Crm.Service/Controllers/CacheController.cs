using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tc.Crm.Service.CacheBuckets;

namespace Tc.Crm.Service.Controllers
{
    public class CacheController : ApiController
    {
        BrandBucket brandBucket;
        CountryBucket countryBucket;
        CurrencyBucket currencyBucket;
        GatewayBucket gatewayBucket;
        SourceMarketBucket sourceMarketBucket;
        TourOperatorBucket tourOperatorBucket;
        HotelBucket hotelBucket;

        public CacheController(BrandBucket brandBucket
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

        [Route("api/v1/cache/refresh")]
        [Route("api/cache/refresh")]
        [HttpPost]
        public HttpResponseMessage Refresh(string[] keys)
        {
            if(keys == null)
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            try
            {
                foreach (var key in keys)
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

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Unexpected error Booking.Update::Message:{0}||Trace:{1}", ex.Message, ex.StackTrace.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
    }
}
