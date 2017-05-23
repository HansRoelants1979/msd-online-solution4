namespace Tc.Crm.Service.BusinessServices
{
    interface IUserService
    {
        int Authenticate(string userName,string password);
    }
}
