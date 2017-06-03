using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Net;
using System.Web;

namespace Tc.Crm.Service.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public string GetPublicKey(string fileName)
        {
            var path = HttpContext.Current.Server.MapPath(@"~/" + fileName);

            using (var webClient = new WebClient())
            {
                return webClient.DownloadString(path);
            }
        }
        public Collection<string> GetPublicKeyFileNames(Api contextApi)
        {
            if (contextApi == Api.Nothing)
                throw new InvalidOperationException("Not a valid api for this method.");
            string fileNames = string.Empty;

            if (contextApi == Api.Booking)
            {
                fileNames = ConfigurationManager.AppSettings[Constants.Configuration.AppSettings.BookingPublicKeyFileNames];
                return GetFileNamesFromCsv(fileNames);
            }
            else if (contextApi == Api.Survey)
            {
                fileNames = ConfigurationManager.AppSettings[Constants.Configuration.AppSettings.SurveyPublicKeyFileNames];
                return GetFileNamesFromCsv(fileNames);
            }
            else
                return null;
        }
        public Collection<string> GetFileNamesFromCsv(string fileNames)
        {
            var fileNameCollection = new Collection<string>();

            if (string.IsNullOrWhiteSpace(fileNames))
                return null;

            var fileList = fileNames.Split(',');

            if (fileList == null || fileList.Length == 0) return null;

            foreach (var item in fileList)
            {
                fileNameCollection.Add(item);
            }
            return fileNameCollection;
        }
        public string GetSecretKey()
        {
            return ConfigurationManager.AppSettings[Constants.Configuration.AppSettings.JsonWebTokenSecret];
        }
        public string GetIssuedAtTimeExpiryInSeconds()
        {
            return ConfigurationManager.AppSettings[Constants.Configuration.AppSettings.IssuedAtTimeExpiryInSeconds];
        }

       
    }
}