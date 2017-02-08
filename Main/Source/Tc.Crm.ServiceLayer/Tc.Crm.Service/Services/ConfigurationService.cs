using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;

namespace Tc.Crm.Service.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public string GetPublicKey()
        {
            var fileName = ConfigurationManager.AppSettings[Constants.Configuration.AppSettings.PublicKeyFileName];
            var path = HttpContext.Current.Server.MapPath(@"~/" + fileName);

            using (var webClient = new WebClient())
            {
                return webClient.DownloadString(path);
            }
        }

        public string GetIssuedAtTimeExpiryInSeconds()
        {
            return ConfigurationManager.AppSettings[Constants.Configuration.AppSettings.IssuedAtTimeExpiryInSeconds];
        }
    }
}