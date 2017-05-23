using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;
using Tc.Crm.Plugins.Hotel.BusinessLogic;

namespace Tc.Crm.Plugins.Hotel
{
    public class PrevalidationCreateHotel : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);

            try
            {
                trace.Trace("Begin - AddOwner");
                HotelOwnerService hotelOwnerService = new HotelOwnerService(context, trace, service);
                hotelOwnerService.AddOwner();
                trace.Trace("End - AddOwner");

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

    }
}
