using System;
using System.Collections.Generic;
using Tc.Crm.Service.Services;
using Tc.Crm.Service.Models;
using Constants = Tc.Crm.Service.Constants;
namespace Tc.Crm.Service.CacheBuckets
{
    public class GatewayBucket:IBucket
    {
        ICrmService crmService;
        public GatewayBucket(ICrmService crmService)
        {
            this.crmService = crmService;
            //FillBucket();
        }
        public string MapTransportType(TransportType Type)
        {
            string gateWayType = string.Empty;
             
            switch (Type){
                case TransportType.CharterFlight:
                    gateWayType = Constants.GatewayType.Airport;
                    break;
                case TransportType.ScheduledFlight:
                    gateWayType = Constants.GatewayType.Airport;
                    break;
                case TransportType.Motorail:
                    gateWayType = Constants.GatewayType.TrainStation;
                    break;
                case TransportType.Rail:
                    gateWayType = Constants.GatewayType.TrainStation;
                    break;
                case TransportType.Ferry:
                    gateWayType = Constants.GatewayType.Port;
                    break;
                case TransportType.Coach:
                    gateWayType = Constants.GatewayType.Other;
                    break;
                default:
                    gateWayType = Constants.GatewayType.Airport;
                    break;
            }
            return gateWayType;
        }
        public Dictionary<string, string> Items { get; set; }
        public void FillBucket()
        {
            Items = new Dictionary<string, string>();
            Items= crmService.GetGateways();
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