using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Tc.Crm.Plugins.CacheRequest.BusinessLogic;

namespace Tc.Crm.Plugins.CacheRequest
{
    public class PostCreateCacheRequestAsync : IPlugin
    {
        public Dictionary<string,string> SecureSettings { get; set; }
        public PostCreateCacheRequestAsync(string unsecureConfig, string secureConfig)
        {
            if (string.IsNullOrWhiteSpace(secureConfig)) return;
            SetSecureSettings(secureConfig);
        }

        private void SetSecureSettings(string secureConfig)
        {
            if (SecureSettings == null) SecureSettings = new Dictionary<string, string>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(secureConfig);
            var settingNodes = doc.SelectNodes("//setting");
            foreach (XmlNode n in settingNodes)
            {
                var key = n.SelectSingleNode("key").InnerText;
                var value = n.SelectSingleNode("value").InnerText;
                if (SecureSettings.ContainsKey(key))
                    SecureSettings[key] = value;
                else
                    SecureSettings.Add(key, value);
            }
        }
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);

            var secretKey = SecureSettings["SecretKey"];
            ICachingApiService cachingApiService = new CachingApiService();

            var cachingService = new CachingService(cachingApiService);
            cachingService.SecretKey = secretKey;
            try
            {
                cachingService.SendCacheRequest(context, trace, service);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                cachingService.HandleError(ex,context,service);
                cachingService.UpdateStatus(false, context, service);
                throw new InvalidPluginExecutionException($"Message:{ex.Message}:: Stack Trace:{ex.StackTrace.ToString()}");
            }
            catch (TimeoutException ex)
            {
                cachingService.HandleError(ex, context, service);
                cachingService.UpdateStatus(false, context, service);
                throw new InvalidPluginExecutionException($"Message:{ex.Message}:: Stack Trace:{ex.StackTrace.ToString()}");
            }
            catch (Exception ex)
            {
                cachingService.HandleError(ex, context, service);
                cachingService.UpdateStatus(false, context, service);
                throw new InvalidPluginExecutionException($"Message:{ex.Message}:: Stack Trace:{ex.StackTrace.ToString()}");
            }
        }
    }
}
