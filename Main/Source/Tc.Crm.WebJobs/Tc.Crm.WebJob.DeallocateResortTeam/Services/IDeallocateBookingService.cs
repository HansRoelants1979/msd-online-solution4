using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.WebJob.DeallocateResortTeam.Models;

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
