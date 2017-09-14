using System;
using System.ServiceModel;
using Microsoft.Practices.Unity;
using Tc.Crm.Common.IntegrationLayer.Jti.Service;
using Tc.Crm.Common.IntegrationLayer.Service.Synchronisation;
using Tc.Crm.Common.IntegrationLayer.Service.Synchronisation.Outbound;
using Tc.Crm.Common.Services;

namespace Tc.Crm.OutboundSynchronisation.Customer
{
    public class Program
    {
        static void Main()
        {
            ILogger logger = null;

            try
            {
                IUnityContainer unitycontainer = new UnityContainer();
                unitycontainer.RegisterType<ILogger, Logger>(new ContainerControlledLifetimeManager());
                unitycontainer.RegisterType<IConfigurationService, ConfigurationService>(new ContainerControlledLifetimeManager());
                unitycontainer.RegisterType<ICrmService, CrmService>(new ContainerControlledLifetimeManager());
                unitycontainer.RegisterType<IJwtService, JwtService>(new ContainerControlledLifetimeManager());
                unitycontainer.RegisterType<IRequestPayloadCreator, CreateCustomerRequestPayloadCreator>(new ContainerControlledLifetimeManager());
                unitycontainer.RegisterType<IOutboundSynchronisationDataService, OutboundSynchronisationDataService>(new ContainerControlledLifetimeManager());
                unitycontainer.RegisterType<IOutboundSynchronisationService, OutboundSynchronisationService>(new ContainerControlledLifetimeManager());

                logger = unitycontainer.Resolve<ILogger>();
                using (var outboundSynchronisationCustomerService = unitycontainer.Resolve<IOutboundSynchronisationService>())
                {
                    logger.LogInformation("Tc.Crm.OutboundSynchronisation.Customer Job Starts");
                    outboundSynchronisationCustomerService.Run();
                    logger.LogInformation("\r\n\r\nTc.Crm.OutboundSynchronisation.Customer Job Ends");
                }
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                logger.LogError(ex.ToString());
            }
            catch (TimeoutException ex)
            {
                logger.LogError(ex.ToString());
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
            }
        }
    }
}