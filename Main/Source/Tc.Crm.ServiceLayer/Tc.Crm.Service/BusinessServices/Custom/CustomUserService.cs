namespace Tc.Crm.Service.BusinessServices.Custom
{
    public class CustomUserService : IUserService
    {
        public int Authenticate(string userName, string password)
        {
            if (userName == "crmuser" && password == "pass1234$")
                return 10;
            return 0;
        }
    }
}