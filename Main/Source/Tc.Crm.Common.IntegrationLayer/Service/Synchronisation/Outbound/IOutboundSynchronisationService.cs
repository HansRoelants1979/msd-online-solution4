using System;

namespace Tc.Crm.Common.IntegrationLayer.Service.Synchronisation.Outbound
{
	public enum Operation
	{
		Create,
		Update
	}

	public interface IOutboundSynchronisationService : IDisposable
    {
		/// <summary>
		/// Execute customer outbound synchronisation
		/// </summary>
		void ProcessEntityCacheOperation(Operation operation);
	}
}