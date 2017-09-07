using Microsoft.Xrm.Sdk;

namespace Tc.Crm.Common.Helper
{
    public class EntityHelper
    {
        public static bool HasAttribute(Entity entity, string attributeName)
        {
            return entity != null && entity.Attributes.Contains(attributeName);
        }

        public static bool HasAttributeNotNull(Entity entity, string attributeName)
        {
            return entity != null && entity.Attributes.Contains(attributeName) && entity.Attributes[attributeName] != null;
        }
    }
}
