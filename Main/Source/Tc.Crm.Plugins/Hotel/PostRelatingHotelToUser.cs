using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;
using Tc.Crm.Plugins.Hotel.BusinessLogic;

namespace Tc.Crm.Plugins.Hotel
{
    public class PostRelatingHotelToUser : IPlugin
    {
        private string[] businessUnitNames;

        public PostRelatingHotelToUser() { }

        public PostRelatingHotelToUser(string unsecureConfig, string secureConfig)
        {
            if (!string.IsNullOrWhiteSpace(unsecureConfig))
                businessUnitNames = unsecureConfig.Split(',');
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService orgService = factory.CreateOrganizationService(context.UserId);

            try
            {
                if (context.InputParameters.Contains(InputParameters.Relationship))
                {
                    trace.Trace(((Relationship)context.InputParameters[InputParameters.Relationship]).SchemaName);
                }
                var isValidContext = IsContextValid(context);
                if (isValidContext)
                {
                    PostRelatingHotelToUserService service = new PostRelatingHotelToUserService(context, trace, orgService, businessUnitNames);
                    var isAssign = context.MessageName.Equals(Messages.Associate, StringComparison.OrdinalIgnoreCase);
                    if (isAssign)
                        service.AddUserToHotelTeam();
                    else
                        service.RemoveUserFromHotelTeams();
                }
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

        private static bool IsContextValid(IPluginExecutionContext context)
        {
            if (!(context.MessageName.Equals(Messages.Associate, StringComparison.OrdinalIgnoreCase) || context.MessageName.Equals(Messages.Disassociate, StringComparison.OrdinalIgnoreCase)) ||
                context.Stage != (int)PluginStage.Postoperation ||
                !context.InputParameters.Contains(InputParameters.Relationship) ||
                !Relationships.UserHotels.Equals(((Relationship)context.InputParameters[InputParameters.Relationship]).SchemaName, StringComparison.InvariantCultureIgnoreCase) ||
                !context.InputParameters.Contains(InputParameters.Target) ||
                !(context.InputParameters[InputParameters.Target] is EntityReference) ||
                !context.InputParameters.Contains(InputParameters.RelatedEntities) ||
                !(context.InputParameters[InputParameters.RelatedEntities] is EntityReferenceCollection))
                return false;
            return true;
        }

    }
}
