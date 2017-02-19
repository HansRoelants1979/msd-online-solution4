using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.WebJob.AllocateResortTeam.Services;
using Microsoft.Practices.Unity;

namespace Tc.Crm.WebJob.AllocateResortTeam
{
    class Program
    {
        static void Main(string[] args)
        {
            //setup our DI
            IUnityContainer unitycontainer = new UnityContainer();
            unitycontainer.RegisterType<ILogger, Logger>(new ContainerControlledLifetimeManager());
            unitycontainer.RegisterType<IConfigurationService, ConfigurationService>(new ContainerControlledLifetimeManager());
            unitycontainer.RegisterType<ICrmService, CrmService>(new ContainerControlledLifetimeManager());
            unitycontainer.RegisterType<IAllocationService, AllocationService>(new ContainerControlledLifetimeManager());
            unitycontainer.RegisterType<IAllocateResortTeamService, AllocateResortTeamService>(new ContainerControlledLifetimeManager());

            var logger = unitycontainer.Resolve<ILogger>();            
            using (var allocateResortTeamService = unitycontainer.Resolve<IAllocateResortTeamService>())
            {
                logger.LogInformation("Tc.Crm.WebJob.AllocateResortTeam Job Starts");
                allocateResortTeamService.Run();
                logger.LogInformation("Tc.Crm.WebJob.AllocateResortTeam Job End");
            }

                
            
        }
    }
}
