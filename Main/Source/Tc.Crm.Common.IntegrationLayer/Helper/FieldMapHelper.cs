using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.Common.IntegrationLayer.Model;

namespace Tc.Crm.Common.IntegrationLayer.Helper
{
    public static class FieldMapHelper
    {
        public static bool TryMapField(string attributeName, Field field, Action<string> setter)
        {
            string value;
            if (string.Equals(attributeName, field.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                if (field.Value != null)
                {
                    switch (field.Type)
                    {
                        case FieldType.Guid:
                            value = field.Value.ToString();
                            break;
                        case FieldType.DateTime:
                            value = ((DateTime)field.Value).ToShortDateString();
                            break;
                        case FieldType.String:
                            value = field.Value as string;
                            break;
                        default:
                            throw new NotImplementedException("Implement converting value");
                    }
                    setter(value);
                }
                return true;
            }
            return false;
        }
    }
}
