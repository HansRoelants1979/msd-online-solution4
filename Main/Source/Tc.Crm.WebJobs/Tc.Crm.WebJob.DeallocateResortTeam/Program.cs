using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.WebJob.DeallocateResortTeam.Services;
using Microsoft.Practices.Unity;

namespace Tc.Crm.WebJob.DeallocateResortTeam
{
    class Program
    {
        static void Main(string[] args)
        {
            //setup our DI
            IUnityContainer unitycontainer = new UnityContainer();
            unitycontainer.RegisterType<IConfigurationService, ConfigurationService>(new ContainerControlledLifetimeManager());
            unitycontainer.RegisterType<ICrmService, CrmService>(new ContainerControlledLifetimeManager());
            unitycontainer.RegisterType<IDeallocateResortTeamService, DeallocateResortTeamService>(new ContainerControlledLifetimeManager());


            using (var deallocateResortTeamService = unitycontainer.Resolve<IDeallocateResortTeamService>())
            {
                deallocateResortTeamService.Run();
            }

        }
    }
}
