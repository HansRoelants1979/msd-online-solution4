using System;
using Tc.Crm.WebJob.AllocateResortTeam.Services;
using System.ServiceModel;
using Microsoft.Practices.Unity;
using Tc.Crm.Common.Services;

namespace Tc.Crm.WebJob.AllocateResortTeam
{
    class Program
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Tc")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "AllocateResortTeam")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Tc.Crm.WebJob.AllocateResortTeam.Services.ILogger.LogInformation(System.String)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "args")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "WebJob")]
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
                    logger.LogInformation("\r\n\r\nTc.Crm.WebJob.AllocateResortTeam Job End");
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
