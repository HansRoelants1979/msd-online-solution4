using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.WebJob.DeallocateResortTeam.Services;
using Microsoft.Practices.Unity;
using System.ServiceModel;

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
                    logger.LogInformation("Tc.Crm.WebJob.AllocateResortTeam Job Starts");
                    deallocateResortTeamService.Run();
                    logger.LogInformation("Tc.Crm.WebJob.AllocateResortTeam Job End");
                }
                using (var deallocateResortTeamService = unitycontainer.Resolve<IDeallocateResortTeamService>())
                {
                    deallocateResortTeamService.Run();
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
