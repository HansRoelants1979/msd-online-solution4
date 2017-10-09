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

	public enum CustomerStatusCode
	{
		Unknown = 950000002,
		Active = 1,
		Blacklisted = 950000003,
		Deceased = 950000004,
		Inactive = 950000005
	}

	public enum Gender
	{
		Male = 950000000,
		Female = 950000001,
		Unknown = 950000002
	}

	public enum PhoneType
	{
		Mobile = 950000000,
		Home = 950000001,
		Business = 950000003,
		Unknown = 950000002
	}

	public enum EmailType
	{
		Primary = 950000000,
		Promotion = 950000001,
		Unknown = 950000002
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
		Pending = 950000002,
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
        SuccessfullySentToIL = 2,
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
        Head,
        Patch
    }
}
