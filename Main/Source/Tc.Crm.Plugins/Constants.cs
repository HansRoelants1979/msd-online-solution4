using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.Plugins
{

    public static class General
    {        
        public const string TeamRoleName = "Tc.Ids.Base";
               
    }

    public static class Messages
    {
        public const string Associate = "Associate";
        public const string Create = "Create";
    }
    

    public static class InputParameters
    {
        public const string Target = "Target";
        public const string Relationship  = "Relationship";
        public const string RelatedEntities = "RelatedEntities";
    }

    public static class Entities
    {
        public const string Team = "team";
        public const string BusinessUnit = "businessunit";
        public const string Role = "role";
        public const string User = "systemuser";
    }

    public static class Attributes
    {
        public class Team
        {
            public const string Name = "name";
            public const string HotelTeam = "tc_hotelteam";
            public const string BusinessUnitId = "businessunitid";
            public const string TeamId = "teamid";
            public const string HotelTeamId = "tc_hotelteamid";
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

        public class BusinessUnit
        {
            public const string BusinessUnitId = "businessunitid";
            public const string Name = "name";
        }
    }

    public static class Relationships
    {
        public const string TeamRolesAssociation = "teamroles_association";
        public const string TeamMembershipAssociation = "teammembership_association.";
    }
}
