using System;
using Tc.Crm.Common.Models;

namespace Tc.Crm.WebJob.DeallocateResortTeam.Models
{
    public class BookingResortTeamRequest
    {
        public Guid Id { get; set; }
        public Owner Owner { get; set; }
    }
}
