using System;
using Tc.Crm.WebJob.AllocateResortTeam.Services;
using System.ServiceModel;
using Microsoft.Practices.Unity;

namespace Tc.Crm.WebJob.AllocateResortTeam
{
    class Program
    {
        static void Main(string[] args)
        {
            ILogger logger = null;
            try
            {
                //setup our DI
                IUnityContainer unitycontainer = new UnityContainer();
                unitycontainer.RegisterType<ILogger, Logger>(new ContainerControlledLifetimeManager());
                unitycontainer.RegisterType<IConfigurationService, ConfigurationService>(new ContainerControlledLifetimeManager());
                unitycontainer.RegisterType<ICrmService, CrmService>(new ContainerControlledLifetimeManager());
                unitycontainer.RegisterType<IAllocationService, AllocationService>(new ContainerControlledLifetimeManager());
                unitycontainer.RegisterType<IAllocateResortTeamService, AllocateResortTeamService>(new ContainerControlledLifetimeManager());

                logger = unitycontainer.Resolve<ILogger>();
                using (var allocateResortTeamService = unitycontainer.Resolve<IAllocateResortTeamService>())
                {
                    logger.LogInformation("Tc.Crm.WebJob.AllocateResortTeam Job Starts");
                    allocateResortTeamService.Run();
                    logger.LogInformation("Tc.Crm.WebJob.AllocateResortTeam Job End");
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
