using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;

namespace Tc.Crm.Plugins
{
    public class AddUserToHotelTeam : IPlugin
    {
        public string[] businessUnitNames;
        public AddUserToHotelTeam(string unSecureConfig, string SecureConfig)
        {
            if (!string.IsNullOrWhiteSpace(unSecureConfig))
                businessUnitNames = unSecureConfig.Split(',');
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext localContext = (IPluginExecutionContext)
               serviceProvider.GetService(typeof(IPluginExecutionContext));

            ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(localContext.UserId);
            try
            {
                if (localContext.Depth > 1)
                    return;

                
                if (localContext.MessageName == Messages.Associate)
                {
                    tracingService.Trace("Begin - PluginExecution");
                    string relationshipName = string.Empty;                 
                    if (localContext.InputParameters.Contains(InputParameters.Relationship))
                    {
                        relationshipName = localContext.InputParameters[InputParameters.Relationship].ToString();
                        tracingService.Trace("Relationship " + relationshipName);
                    }
                   
                    if (relationshipName != Relationships.TeamMembershipAssociation + ".")
                        return;

                    ProcessUserHotelTeam processUserHotelTeam = new ProcessUserHotelTeam(service, localContext, tracingService, businessUnitNames);
                    processUserHotelTeam.ProcessAddUserToHotelTeam();                    
                    tracingService.Trace("End - PluginExecution");
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

       

        
    }
}
