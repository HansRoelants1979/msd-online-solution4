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

    public enum Status
    {
        Active = 0,
        Inactive = 1
    }

    public enum EntityCacheStatusReason
    {
        Active = 1,
        InProgress = 950000000,
        Failed = 950000001,
        Succeeded = 2
    }

    public enum EntityCacheOperation
    {
        Create = 950000000,
        Update = 950000001
    }

    public enum EntityCacheMessageStatusReason
    {
        Active = 1,
        SuccessfullySenttoIL = 2,
        EndtoEndSuccess = 950000000,
        Failed = 950000001
    }

    public enum HttpMethod
    {
        Post,
        Get,
        Put,
        Delete,
        Options,
        Trace,
        Head
    }
}
