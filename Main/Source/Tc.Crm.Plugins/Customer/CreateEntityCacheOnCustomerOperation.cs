using Tc.Crm.Plugins.MultipleEntities;
using Tc.Crm.Plugins.Customer.BusinessLogic;


namespace Tc.Crm.Plugins.Customer 
{
    public class CreateEntityCacheOnCustomerOperation : CreateEntityCacheOnEntityOperation
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="unsecureConfig"></param>
        /// <param name="secureConfig"></param>
        public CreateEntityCacheOnCustomerOperation(string unsecureConfig, string secureConfig) : base(unsecureConfig, secureConfig)
        {
            BusinessLogic = new CreateEntityCacheOnCustomerOperationService();
        }

        public override string EntityName
        {
            get
            {
                return Entities.Contact;
            }
        }

        public override string PluginName
        {
            get
            {
                return this.GetType().Name;
            }
        }

       
    }
}
