using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.Common.Models
{
    public abstract class EntityModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Owner Owner { get; set; }
        public abstract string EntityName {get;}

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var item = obj as EntityModel;
            if (item == null)
                return false;
            return Id == item.Id;
        }
    }
}
