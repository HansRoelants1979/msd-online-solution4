using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace Tc.Crm.Common.Services
{
    public class GeneralMethods
    {
        public static bool IsAttributeExist(Entity entity, string attributeName)
        {
            return entity != null && entity.Attributes.Contains(attributeName);
        }

        public static bool IsAttributeExistAndNotNull(Entity entity, string attributeName)
        {
            return entity != null && entity.Attributes.Contains(attributeName) && entity.Attributes[attributeName] != null;
        }
    }
}
