using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using System.ServiceModel;
using Tc.Crm.Plugins.MultipleEntities.BusinessLogic;

namespace Tc.Crm.Plugins.MultipleEntities
{
    public abstract class CreateEntityCacheOnEntityOperation : IPlugin
    {
        string[] serviceAccountsToIgnore;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="unsecureConfig"></param>
        /// <param name="secureConfig"></param>
        public CreateEntityCacheOnEntityOperation(string unsecureConfig, string secureConfig)
        {
            if (!string.IsNullOrWhiteSpace(unsecureConfig))
                serviceAccountsToIgnore = unsecureConfig.Split(',');
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);
            try
            {
                if (!IsContextValid(context)) return;
                trace.Trace("Context is valid");
                trace.Trace("Begin - " + PluginName);
                BusinessLogic.DoActionsOnEntityOperation(context, trace, service);                
                trace.Trace("End -  " + PluginName);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new InvalidPluginExecutionException(ex.ToString());
            }
            catch (TimeoutException ex)
            {
                throw new InvalidPluginExecutionException(ex.ToString());
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.ToString());
            }
        }

        /// <summary>
        /// To check whether context is valid to execute or not
        /// </summary>
        /// <returns></returns>
        private bool IsContextValid(IPluginExecutionContext context)
        {
            if (!IsValidEntity(context)) return false;
            if (!context.MessageName.Equals(Messages.Create, StringComparison.OrdinalIgnoreCase)) return false;
            if (context.Stage != (int)PluginStage.Postoperation) return false;
            if (context.Mode != (int)PluginMode.Asynchronous) return false;
            if (ServiceAccountsToIgnoreHasValue() && serviceAccountsToIgnore.Contains(FormatGuid(context.InitiatingUserId.ToString()))) return false;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool IsValidEntity(IPluginExecutionContext context)
        {
            return context.PrimaryEntityName == EntityName;
        }

        /// <summary>
        /// To check whether global variable serviceAccountstoignore is not null and has value
        /// </summary>
        /// <returns></returns>
        private bool ServiceAccountsToIgnoreHasValue()
        {
            if (serviceAccountsToIgnore != null && serviceAccountsToIgnore.Length > 0)
            {
                this.serviceAccountsToIgnore = serviceAccountsToIgnore.Select(s => FormatGuid(s)).ToArray();
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// To format guid in a way by replacing '{','}' to empty and convert to lowercase
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        private string FormatGuid(string guid)
        {
            return guid.Replace(SpecialCharacters.OpenBrace, string.Empty).Replace(SpecialCharacters.ClosedBrace, string.Empty).ToLower();
        }

        public abstract string PluginName { get; }

        public CreateEntityCacheOnEntityOperationService BusinessLogic { get; set; } 

        public abstract string EntityName { get;  }
    }
}
