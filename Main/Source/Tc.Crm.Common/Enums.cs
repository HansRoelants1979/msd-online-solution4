using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.Common
{
    public enum OwnerType
    {
        User,
        Team
    }


    public enum CustomerType
    {        
        Contact,
        Account 
    }

    public enum CaseStatusCode
    {
        InProgress = 1,
        AssignedToLocalSourceMarket = 950000003,
        EscalatedToLocalSourceMarket = 950000004
    }

    public enum CaseState
    {
        Active = 0,
        Inactive = 1
    }

    public enum ResortTeamRequestType
    {
        BookingRequest = 0,
        CustomerRequest = 1,
        Both=2
    }
}
