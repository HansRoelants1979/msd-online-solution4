using System.Collections.Generic;
using Tc.Crm.Service.Models;
using Tc.Crm.Service.Services;
using Tc.Crm.Service.Models;
using Constants = Tc.Crm.Service.Constants;
namespace Tc.Crm.Service.CacheBuckets
{
	public class GatewayBucket : ReferenceBucket<Gateway>
	{
		public GatewayBucket(ICrmService crmService) : base(crmService) { }

		protected override IEnumerable<Gateway> GetEntities()
		{
			return CrmService.GetGateways();
		}

		public string MapTransportType(TransportType Type)
		{
			string gateWayType = string.Empty;

			switch (Type)
			{
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
	}
}