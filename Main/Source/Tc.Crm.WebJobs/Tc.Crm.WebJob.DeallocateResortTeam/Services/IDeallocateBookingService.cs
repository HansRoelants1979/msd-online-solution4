using System;

namespace Tc.Crm.WebJob.DeallocateResortTeam.Services
{
    interface IDeallocateBookingService : IDisposable
    {
        /// <summary>
        /// Execute booking deallocation
        /// </summary>
        void Run();
    }
}
