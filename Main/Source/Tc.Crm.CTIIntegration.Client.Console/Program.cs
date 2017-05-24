using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.ServiceModel;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Tc.Crm.CTIIntegration.Client.Console.Service;
using Tc.Crm.CTIIntegration.Client.Console.Models;

namespace Tc.Crm.CTIIntegration.Client.Console
{
    class Program
    {
        IOrganizationService _service;
        static void Main(string[] args)
        {
            Program objP = new Program();
            objP.Execute();
            System.Console.ReadLine();
        }
        public void Execute()
        {
            Previlage PrevilageAccessType = new Previlage();
            while (true)
            {
                try
                {
                    _service = CrmServiceHelper.InitializeOrganizationService();
                    string _securityRole = System.Configuration.ConfigurationManager.AppSettings["SecurityRoleName"];

                    while (true)
                    {
                        System.Console.WriteLine("Login into CRM as non-interactive user: y/n");
                        var option = System.Console.ReadLine();
                        option = option.ToLower();
                        if (option == "y")
                        {
                            CrmServiceHelper.checkUserLogin(_service);
                            Guid _userId = ((WhoAmIResponse)_service.Execute(new WhoAmIRequest())).UserId;
                            Guid _roleId = CTIIntegrationHelper.checkSecurityRole(_securityRole, _userId, _service);
                            if (_roleId != null)
                            {
                                while (true)
                                {
                                    System.Console.WriteLine("Check  Access for  Entities : Enter 1 for Contact or 2 for Account or 3 for Case");
                                    var entity = System.Console.ReadLine();
                                    System.Console.WriteLine("Access Type : Enter 1 for Read Access or 2 for Write Access 3 for Create Access");
                                    var accessType = System.Console.ReadLine();
                                    if (_roleId != null)
                                    {
                                        try
                                        {
                                            PrevilageAccessType = CTIIntegrationHelper.GetPrevilageType(entity, accessType);

                                            bool HasPrevilage = CTIIntegrationHelper.checkthePrevilage(_userId, PrevilageAccessType.PrevilageType, _service);
                                            if (HasPrevilage == true)
                                            {

                                                System.Console.WriteLine("Non-interactive user  has " + PrevilageAccessType.PrevilageType + " access to " + PrevilageAccessType.EntityName + " entity");
                                                System.Console.WriteLine("*****************************************************");
                                            }
                                            else
                                            {
                                                System.Console.WriteLine("Non-interactive user  does not have " + PrevilageAccessType.PrevilageType + " access to " + PrevilageAccessType.EntityName + " entity");
                                                System.Console.WriteLine("*****************************************************");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            System.Console.WriteLine("Non-interactive user security role " + _securityRole + " does not have " + PrevilageAccessType.PrevilageType + " access to " + PrevilageAccessType.EntityName + " entity");
                                            System.Console.WriteLine("*****************************************************");

                                        }
                                    }
                                    else
                                    {
                                        System.Console.WriteLine("Non-interative user does not associated with any Securityrole");
                                        System.Console.WriteLine("*****************************************************");

                                    }
                                    System.Console.Write("Do one more test(y/n):");
                                    var Ans = System.Console.ReadLine();
                                    if (Ans == "n") break;
                                }
                            }
                        }
                        else
                        {
                            System.Console.ReadLine();
                        }
                        System.Console.Write("Do one more test(y/n):");
                        var start = System.Console.ReadLine();
                        if (start == "n") break;
                    }

                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("RetrievePrivilegeForUser failed - no roles are assigned to user");
                    System.Console.WriteLine("*****************************************************");

                    //throw ex;
                }
                System.Console.Write("Do one more test(y/n):");
                var ans = System.Console.ReadLine();
                if (ans == "n") break;
            }
        }
    }
}
