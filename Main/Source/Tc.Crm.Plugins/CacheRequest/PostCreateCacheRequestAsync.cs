using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.Plugins.CacheRequest.BusinessLogic;

namespace Tc.Crm.Plugins.CacheRequest
{
    public class PostCreateCacheRequestAsync : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);
            var cachingService = new CachingService();
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
