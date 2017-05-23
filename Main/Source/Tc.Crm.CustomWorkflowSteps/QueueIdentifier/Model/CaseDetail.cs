using System;

namespace Tc.Crm.CustomWorkflowSteps.QueueIdentifier.Model
{
    public class CaseDetail
    {
        public Guid CaseSourceMarket { get; set; }
        public Guid BookingSourceMarket { get; set; }
        public Guid ContactSourceMarket { get; set; }
        public Guid AccountSourceMarket { get; set; }
        public Guid Owner { get; set; }
        public string OwnerType { get; set; }
    }
}
