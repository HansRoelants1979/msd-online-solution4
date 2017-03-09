using System;
using Tc.Crm.WebJob.DeallocateResortTeam.Services;
using Microsoft.Practices.Unity;
using System.ServiceModel;
using Tc.Crm.Common.Services;

namespace Tc.Crm.WebJob.DeallocateResortTeam
{
    class Program
    {
        static void Main(string[] args)
        {
            //setup our DI
            ILogger logger = null;
            try
            {
                IUnityContainer unitycontainer = new UnityContainer();
                unitycontainer.RegisterType<ILogger, Logger>(new ContainerControlledLifetimeManager());
                unitycontainer.RegisterType<IConfigurationService, ConfigurationService>(new ContainerControlledLifetimeManager());
                unitycontainer.RegisterType<ICrmService, CrmService>(new ContainerControlledLifetimeManager());
                unitycontainer.RegisterType<IDeallocationService, DeallocationService>(new ContainerControlledLifetimeManager());
                unitycontainer.RegisterType<IDeallocateResortTeamService, DeallocateResortTeamService>(new ContainerControlledLifetimeManager());

                logger = unitycontainer.Resolve<ILogger>();
                using (var deallocateResortTeamService = unitycontainer.Resolve<IDeallocateResortTeamService>())
                {
                    logger.LogInformation("Tc.Crm.WebJob.DeallocateResortTeam Job Starts");
                    deallocateResortTeamService.Run();
                    logger.LogInformation("\r\n\r\nTc.Crm.WebJob.DeallocateResortTeam Job End");
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
