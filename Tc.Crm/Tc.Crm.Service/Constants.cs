using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tc.Crm.Service
{
    public static class Constants
    {
        public static class Messages
        {
            public const string RESPONSE_NULL = "Response is Null.";
            public const string SOURCE_KEY_NOT_PRESENT = "Source key is empty or null.";
        }
        public static class Crm
        {
            public static class Booking
            {
                public const string LOGICAL_NAME = "new_booking";
                public static class Fields
                {
                    public const string SOURCE_KEY = "new_sourcekey";
                    public const string CUSTOMER_ID = "new_customerid";
                    public const string TOTAL = "new_total";
                }
            }
            public static class Contact
            {
                public const string LOGICAL_NAME = "contact";
                public static class Fields
                {
                    public const string FIRST_NAME = "firstname";
                    public const string LAST_NAME = "lastname";
                    public const string EMAIL = "emailaddress1";
                    public const string SOURCE_KEY = "new_sourcekey";
                    public const string CONTACT_ID = "contactid";
                }
            }
        }
    }
}