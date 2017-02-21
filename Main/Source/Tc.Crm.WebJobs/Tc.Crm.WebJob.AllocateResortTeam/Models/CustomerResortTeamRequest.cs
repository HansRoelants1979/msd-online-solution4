using Tc.Crm.Common.Models;

namespace Tc.Crm.WebJob.AllocateResortTeam.Models
{
    public class CustomerResortTeamRequest
    {
        public Customer Customer { get; set; }
        public Owner Owner { get; set; }
    }
}
