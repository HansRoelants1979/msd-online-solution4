using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.CustomWorkflowSteps
{
    public static class EntityName
    {

        public const string Account = "account";
        public const string Contact = "contact";
    }

    public static class Attributes
    {
        public class Contact
        {
            public const string LastName = "lastname";
            public const string FirstName = "firstname";
        }

        public class Account
        {
            public const string Name = "name";
            
        }
    }
}
