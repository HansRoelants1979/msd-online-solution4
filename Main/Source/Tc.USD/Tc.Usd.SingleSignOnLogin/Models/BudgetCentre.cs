using System;

namespace Tc.Usd.SingleSignOnLogin.Models
{
    public class BudgetCentre
    {
        public Guid StoreId { get; set; }

        public string Name { get; set; }

        public string Region { get; set; }

        public string Abta { get; set; }

        public string BudgetCentreName { get; set; }

        public string Cluster { get; set; }

        public string SearchString => $"{Name} {Region} {BudgetCentreName} {Abta} {Cluster}";
    }
}