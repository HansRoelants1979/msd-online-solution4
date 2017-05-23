using System;
using Tc.Crm.Common.Constants;

namespace Tc.Crm.Common.Models
{
    public class Owner
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public OwnerType OwnerType { get; set; }
        public string OwnerEntityName
        {
            get
            {
                switch (OwnerType)
                {
                    case OwnerType.Team:
                        return EntityName.Team;
                    case OwnerType.User:
                        return EntityName.Team;
                    default:
                        throw new InvalidOperationException("Only team and user are supported as owners of entity");
                }
            }
        }
    }
}
