using System;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
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
                    value = CalculateValue(field);
                    setter(value);
                }
                return true;
            }
            return false;
        }

        public static bool TryMapField(string attributeName, Field field, string op, string path, Action<string, string, string> setter)
        {
            string value = null;
            if (string.Equals(attributeName, field.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                if (field.Value != null)
                    value = CalculateValue(field);
                setter(op, path, value);
                return true;
            }
            return false;
        }

        private static string CalculateValue(Field field)
        {
            string value;
            switch (field.Type)
            {
                case FieldType.Guid:
                    value = field.Value.ToString();
                    break;
                case FieldType.DateTime:
                    value = ((DateTime) field.Value).ToShortDateString();
                    break;
                case FieldType.String:
                    value = field.Value as string;
                    break;
                case FieldType.OptionSet:
                    value = ((OptionSet)field.Value).Value.ToString();
                    break;
                default:
                    throw new NotImplementedException("Implement converting value");
            }
            return value;
        }
    }
}