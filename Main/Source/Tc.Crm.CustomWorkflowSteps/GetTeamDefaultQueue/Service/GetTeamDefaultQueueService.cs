using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tc.Crm.CustomWorkflowSteps.GetTeamDefaultQueue.Service
{
    public class GetTeamDefaultQueueService
    {
        public EntityReference GetTeamDefaultQueue(EntityReference sourceMarket, IOrganizationService service, ITracingService trace)
        {
            EntityReference teamDefaultQueueDetails = null;

            if (sourceMarket == null) return null;
            if (service == null) return null;
            if (trace == null) return null;

            trace.Trace("GetTeamDefaultQueue - start");
            teamDefaultQueueDetails = GetteamDefaultQueueDetails(sourceMarket, service, trace);
            if (teamDefaultQueueDetails == null)
            {
                trace.Trace("teamDefaultQueueDetails are null.");
                return null;
            }

            trace.Trace("GetTeamDefaultQueue - end");
            return teamDefaultQueueDetails;
        }

        private EntityReference GetteamDefaultQueueDetails(EntityReference sourceMarket, IOrganizationService service, ITracingService trace)
        {
            EntityReference teamDefaultQueue = null;
            if (sourceMarket == null) return null;
            if (service == null) return null;
            if (trace == null) return null;

            trace.Trace("GetteamDefaultQueueDetails - start");
            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                           <entity name='team'>
                            <attribute name='name' />
                            <attribute name='businessunitid' />
                            <attribute name='teamid' />
                            <attribute name='teamtype' />
                            <attribute name='isdefault' />
                            <attribute name='queueid' />
                              <order attribute='name' descending='false' />
                            <filter type='and'>
                              <condition attribute='isdefault' operator='eq' value='1' />
                            </filter>
                            <link-entity name='businessunit' from='businessunitid' to='businessunitid' alias='ac'>
                            <link-entity name='tc_country' from='tc_sourcemarketbusinessunitid' to='businessunitid' alias='ad'>
                            <filter type='and'>
                             <condition attribute='tc_countryid' operator='eq' uiname='{0}' uitype='tc_country' value='{1}' />
                            </filter>
                            </link-entity>
                            </link-entity>
                           </entity>
                          </fetch>";
            fetchXml = string.Format(fetchXml, sourceMarket.Name, sourceMarket.Id);
            trace.Trace(fetchXml);
            var query = new FetchExpression(fetchXml);
            trace.Trace("Calling retrieve multiple of service.");
            var response = service.RetrieveMultiple(query);

            if (response == null || response.Entities == null || response.Entities.Count == 0)
            {
                trace.Trace("response is null or response.Entities is null or count is null");
                return null;
            }
            
            trace.Trace("Team record retrieved.");

            if (response.Entities[0].Contains("queueid") && response.Entities[0]["queueid"] != null)
            {
                teamDefaultQueue = ((EntityReference)(response.Entities[0]["queueid"]));
            }

            trace.Trace("GetteamDefaultQueueDetails - end");
            return teamDefaultQueue;
        }
    }
}
