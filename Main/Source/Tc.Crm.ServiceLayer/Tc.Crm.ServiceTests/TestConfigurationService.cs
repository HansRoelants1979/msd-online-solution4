using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using Tc.Crm.Service;
using Tc.Crm.Service.Services;

namespace Tc.Crm.ServiceTests
{
    public class TestConfigurationService : IConfigurationService
    {
        public bool CorrectSignaure;

        

        public string GetIssuedAtTimeExpiryInSeconds()
        {
            return ConfigurationManager.AppSettings[Service.Constants.Configuration.AppSettings.IssuedAtTimeExpiryInSeconds];
        }

        public string GetPublicKey(string fileName)
        {
            if (CorrectSignaure)
            {
                fileName = ConfigurationManager.AppSettings["privateKeyFileName"];
                return File.ReadAllText(fileName);
            }
            else
            {
                fileName = ConfigurationManager.AppSettings["privateKeyFileNameWrong"];
                return File.ReadAllText(fileName);
            }
        }

        public Collection<string> GetPublicKeyFileNames(Api contextApi)
        {
            return new Collection<string> { "Tc.PublicKey.xml" };
        }

        public string GetSecretKey()
        {
            return ConfigurationManager.AppSettings[Service.Constants.Configuration.AppSettings.JsonWebTokenSecret];
        }
    }
}
