using System;

namespace Tc.Crm.Plugins.Merge.Models
{
    public class EntityModel
    {
        protected EntityModel(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
        public string Name { get; set; }

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