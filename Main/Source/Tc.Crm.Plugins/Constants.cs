using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.Plugins
{

    public static class General
    {
        public const string Target = "Target";
        public const string TeamRoleName = "Tc.Ids.Base";
    }
    public static class Entities
    {
        public const string Team = "team";
        public const string BusinessUnit = "businessunit";
        public const string Role = "role";
    }

    public static class Attributes
    {
        public class Team
        {
            public const string Name = "name";
            public const string HotelTeam = "tc_hotelteam";
            public const string BusinessUnitId = "businessunitid";
        }

        public class Hotel
        {
            public const string Owner = "ownerid";
        }

        public class Role
        {
            public const string Name = "name";
            public const string BusinessUnitId = "businessunitid";
        }
    }

    public static class Relationships
    {
        public const string TeamRoles = "teamroles_association";
    }
}
