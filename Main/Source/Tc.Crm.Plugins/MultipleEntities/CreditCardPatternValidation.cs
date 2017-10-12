using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;
using Tc.Crm.Plugins.MultipleEntities.BusinessLogic;

namespace Tc.Crm.Plugins.MultipleEntities
{
    public abstract class CreditCardPatternValidation : IPlugin
    {   
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
                var businessLogic = GetBusinessLogic(trace, service);
                businessLogic.ValidateEntity(context);                
                trace.Trace("End -  " + PluginName);
            }
            catch (InvalidPluginExecutionException ex)
            {
                throw new InvalidPluginExecutionException(ex.Message.ToString());
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
            if (!context.MessageName.Equals(Messages.Create, StringComparison.OrdinalIgnoreCase) && !context.MessageName.Equals(Messages.Update, StringComparison.OrdinalIgnoreCase)) return false;
            if (context.Stage != (int)PluginStage.Preoperation) return false;
            if (context.Mode != (int)PluginMode.Synchronous) return false;
            
            return true;
        }

        /// <summary>
        /// To validate entity name
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool IsValidEntity(IPluginExecutionContext context)
        {
            return context.PrimaryEntityName == EntityName;
        }

        protected abstract string PluginName { get; }

        protected abstract string EntityName { get; }        

        protected abstract CreditCardPatternValidationService GetBusinessLogic(ITracingService trace, IOrganizationService service);
    }
}
