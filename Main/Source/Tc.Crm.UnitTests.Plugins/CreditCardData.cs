using System;
using Microsoft.Xrm.Sdk;
using Tc.Crm.Plugins;
using Attributes = Tc.Crm.Plugins.Attributes;

namespace Tc.Crm.UnitTests.Plugins
{
    public static class CreditCardData
    {
        public static Entity GetCreditCardPattern()
        {
            var configuration = new Entity(Entities.Configuration, Guid.NewGuid());
            configuration.Attributes.Add(Attributes.Configuration.Name, Configurationkeys.CreditCardPattern);
            configuration.Attributes.Add(Attributes.Configuration.Value, "^.*(\\d[^a-zA-Z0-9]?){16,}.*$");

            return configuration;
        }

        public const string SingleLineCreditCardText = "1234567890123456";
        public const string MultiLineCreditCardText = "test \r\n 23455 \r\n 1234567890123456 \r\n end";
    }
}
