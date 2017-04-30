using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using System.Text;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;

namespace Tc.Crm.Plugins
{
    // (balamurali-s) PluginBase class is not used due to the limitations in the FakeXrmEasy unit test framework.


    public class PreCaseLine : IPlugin
    {

        /// <summary>
        /// Main entry point for the business logic that the plug-in is to execute.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> which contains the
        /// <see cref="IPluginExecutionContext"/>,
        /// <see cref="IOrganizationService"/>
        /// and <see cref="ITracingService"/>
        /// </param>
        /// <remarks>
        /// For improved performance, Microsoft Dynamics CRM caches plug-in instances.
        /// The plug-in's Execute method should be written to be stateless as the constructor
        /// is not called for every invocation of the plug-in. Also, multiple system threads
        /// could execute the plug-in at the same time. All per invocation state information
        /// is stored in the context. This means that you should not use global variables in plug-ins.
        /// </remarks>
        public void Execute(IServiceProvider serviceProvider)
        {
            //Extract the tracing service for use in debugging sandboxed plug-ins.
            ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the organization service reference.
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            SetCase24HoursPromiseFlag(service, context, tracingService);
        }


        /// <summary>
        /// Business Logic to set Case 24 Hours Promise to 24 Hours
        /// </summary>
        /// <param name="orgService"></param>
        /// <param name="pluginExecutionContext"></param>
        /// <param name="tracingService"></param>
        protected void SetCase24HoursPromiseFlag(IOrganizationService orgService,
                                                    IPluginExecutionContext pluginExecutionContext,
                                                    ITracingService tracingService)
        {
            tracingService.Trace("Plug-in execution started...");

            // The InputParameters collection contains all the data passed in the message request.
            if (pluginExecutionContext.InputParameters.Contains("Target") &&
                pluginExecutionContext.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.
                var caseLineEntity = (Entity)pluginExecutionContext.InputParameters["Target"];

                // Verify that the post image is configured.
                if (pluginExecutionContext.MessageName == "Update" && !pluginExecutionContext.PostEntityImages.Contains("PostImage"))
                {
                    tracingService.Trace("No post update image has been configured.");
                    return;
                }

                // Verify that the target entity represents Case Line entity
                if (caseLineEntity.LogicalName != "tc_caseline")
                    return;

                try
                {
                    EntityReference incidentEntityReference = null;
                    OptionSetValue caseLineServiceType = null;

                    switch (pluginExecutionContext.MessageName)
                    {
                        case "Create":
                            if (!caseLineEntity.Contains("tc_caseid") || caseLineEntity["tc_caseid"] == null ||
                                !caseLineEntity.Contains("tc_servicetype"))
                            {
                                tracingService.Trace("No Case / Service Type / Case Line Name is specified.");
                                return;
                            }
                            incidentEntityReference = caseLineEntity["tc_caseid"] as EntityReference;
                            caseLineServiceType = caseLineEntity["tc_servicetype"] as OptionSetValue;
                            break;
                        case "Update":
                            var caseLinePostImage = (Entity)pluginExecutionContext.PostEntityImages["PostImage"];
                            if (!caseLinePostImage.Contains("tc_caseid") || caseLinePostImage["tc_caseid"] == null ||
                                !caseLinePostImage.Contains("tc_servicetype"))
                            {
                                tracingService.Trace("No Case / Service Type / Case Line Name is specified.");
                                return;
                            }
                            incidentEntityReference = caseLinePostImage["tc_caseid"] as EntityReference;
                            caseLineServiceType = caseLinePostImage.Attributes["tc_servicetype"] as OptionSetValue;
                            break;
                        default:
                            tracingService.Trace("Plug-in is not registered for Create/ Update");
                            return;
                    }

                    if (caseLineServiceType == null || caseLineServiceType.Value != 950000000) // CaseLine.ServiceType != 'Accommodation'
                    {
                        tracingService.Trace(string.Format("Case Line Service Type is not equal to 'Accommodation': {0} ({1}) \n", caseLineEntity.ToEntityReference().Name,
                                                    caseLineEntity.Id));
                        return;
                    }

                    Entity incident = null;
                    if (!ValidateIncident(orgService, tracingService, incidentEntityReference, caseLineEntity.ToEntityReference(), ref incident) ||
                        !ValidateBookingAccommodation(orgService, tracingService, incident))
                        return;

                    //Set 24 Hours Promise on the case
                    incident.Attributes["tc_24hourpromise"] = new OptionSetValue(950000000); // CaseLine.24hourpromise != 'Yes'

                    orgService.Update(incident);
                    tracingService.Trace(string.Format("Successfully 24 Hours Promise on the Case record has been set: {0}\n", incident.ToEntityReference().Name));
                }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in PreCreateCaseLine plug-in.", ex);
                }
                catch (Exception ex)
                {
                    tracingService.Trace("PreCreateCaseLine Plug-in: {0}", ex.ToString());
                    throw;
                }
            }
        }

        /// <summary>
        /// Validate Incident
        /// </summary>
        /// <param name="orgService"></param>
        /// <param name="tracingService"></param>
        /// <param name="incidentEntityReference"></param>
        /// <param name="caseLineEntityReference"></param>
        /// <param name="incident"></param>
        /// <returns></returns>
        protected bool ValidateIncident(IOrganizationService orgService,
                                                    ITracingService tracingService,
                                                    EntityReference incidentEntityReference,
                                                    EntityReference caseLineEntityReference,
                                                    ref Entity incident)
        {
            StringBuilder incidentQuery = new StringBuilder(string.Format(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                                  <entity name='incident'>
                                                    <attribute name='ticketnumber' />
                                                    <attribute name='createdon' />
                                                    <attribute name='incidentid' />
                                                    <attribute name='tc_bookingid' />
                                                    <attribute name='tc_bookingreference' />
                                                    <attribute name='tc_24hourpromise' />
                                                    <order attribute='ticketnumber' descending='false' />
                                                    <filter type='and'>                                                      
                                                      <condition attribute='incidentid' operator='eq' value='{0}' />                                                      
                                                    </filter>
                                                    <link-entity name='tc_booking' from='tc_bookingid' to='tc_bookingid' visible='false' link-type='outer' alias='Booking'>
                                                          <attribute name='tc_brandid' />
                                                          <filter>
                                                            <condition attribute='tc_brandid' operator='not-null' />
                                                          </filter>
                                                    </link-entity>
                                                  </entity>
                                                </fetch>", incidentEntityReference.Id));

            //Get the booking details of the case.
            incident = orgService.RetrieveMultiple(new FetchExpression(incidentQuery.ToString())).Entities.FirstOrDefault();

            if (incident == null)
            {
                tracingService.Trace(string.Format("No Case associated with Case Line: {0} ({1}) \n\n Query Used: {2}\n", caseLineEntityReference.Name,
                                            caseLineEntityReference.Id,
                                            incidentQuery.ToString()));
                return false;
            }

            if (!incident.Contains("tc_bookingreference") || !(bool)incident["tc_bookingreference"] ||
                !incident.Contains("tc_24hourpromise") || (incident["tc_24hourpromise"] as OptionSetValue).Value != 950000001)
            {
                tracingService.Trace(string.Format("Case's Booking Reference field is not set to 'Yes' or 24 Hours Promise field is not equal to 'No': {0} ({1}) \n\n Query Used: {2}\n", caseLineEntityReference.Name,
                                            caseLineEntityReference.Id,
                                            incidentQuery.ToString()));
                return false;
            }

            var bookingEntityReference = incident.Contains("tc_bookingid") ? incident["tc_bookingid"] as EntityReference : null;
            if (bookingEntityReference == null)
            {
                tracingService.Trace(string.Format("No Booking associated with Case: {0} ({1}) \n\n Query Used: {2}\n", incident.ToEntityReference().Name,
                                            incident.Id,
                                            incidentQuery.ToString()));
                return false;
            }

            var bookingBrandEntityReference = incident.Contains("Booking.tc_brandid") ? (incident["Booking.tc_brandid"] as AliasedValue).Value as EntityReference : null;
            if (bookingBrandEntityReference == null)
            {
                tracingService.Trace(string.Format("No Brand associated with Booking: {0} ({1}) \n\n Query Used: {2}\n", bookingEntityReference.Name,
                                            bookingEntityReference.Id,
                                            incidentQuery.ToString()));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validate Booking Accommodation
        /// </summary>
        /// <param name="orgService"></param>
        /// <param name="tracingService"></param>
        /// <param name="incident"></param>
        /// <returns></returns>
        private bool ValidateBookingAccommodation(IOrganizationService orgService,
                                                    ITracingService tracingService,
                                                    Entity incident)
        {

            var bookingEntityReference = incident.Attributes["tc_bookingid"] as EntityReference;
            var bookingBrandEntityReference = (incident.Attributes["Booking.tc_brandid"] as AliasedValue).Value as EntityReference;
            StringBuilder bookingAccommodationReferenceQuery = new StringBuilder(string.Format(@"<fetch>
                                                                                          <entity name='tc_bookingaccommodation' >
                                                                                            <attribute name='tc_startdateandtime' />
                                                                                            <attribute name='tc_hotelid' />
                                                                                            <filter>
                                                                                              <condition attribute='tc_bookingid' operator='eq' value='{0}' />
                                                                                            </filter>
                                                                                                <link-entity name='tc_hotel' from='tc_hotelid' to='tc_hotelid' alias='Hotel' >
                                                                                                  <attribute name='tc_name' />
                                                                                                  <link-entity name='tc_hotelpromises' from='tc_hotelid' to='tc_hotelid' alias='HotelPromises' >
                                                                                                    <attribute name='tc_promisetype' />
                                                                                                    <filter>
                                                                                                      <condition attribute='tc_brandid' operator='eq' value='{1}' />
                                                                                                    </filter>
                                                                                                  </link-entity>
                                                                                                </link-entity> 
                                                                                          </entity>
                                                                                        </fetch>", bookingEntityReference.Id, bookingBrandEntityReference.Id));

            //Get the booking accommodation/ hotel/ hotel promise of the case booking.
            var bookingAccommodation = orgService.RetrieveMultiple(new FetchExpression(bookingAccommodationReferenceQuery.ToString())).Entities.FirstOrDefault();

            if (bookingAccommodation == null)
            {
                tracingService.Trace(string.Format("No Booking Accommodation associated with Case Booking [{0} ({1})]  for Brand [{2} ({3})] \n\n Query Used: {4}\n", bookingEntityReference.Name,
                                            bookingEntityReference.Id, bookingBrandEntityReference.Name, bookingBrandEntityReference.Id,
                                            bookingAccommodationReferenceQuery.ToString()));
                return false;
            }

            var hotelPromiseType = bookingAccommodation.Contains("HotelPromises.tc_promisetype") ? (bookingAccommodation["HotelPromises.tc_promisetype"] as AliasedValue).Value as OptionSetValue : null;
            if (hotelPromiseType == null || hotelPromiseType.Value != 950000000) // CaseLine.ServiceType != '24 Hours'
            {
                tracingService.Trace(string.Format("No Hotel Promise Type set or not equal to '24 Hours': {0} ({1}) \n\n Query Used: {2}\n", bookingAccommodation.ToEntityReference().Name,
                                            bookingAccommodation.Id,
                                            bookingAccommodationReferenceQuery.ToString()));
                return false;
            }

            var bookingAccommodationStartDateTime = bookingAccommodation.Contains("tc_startdateandtime") ? (DateTime)bookingAccommodation["tc_startdateandtime"] : DateTime.MinValue;

            //Add a day to booking accommodation start date for calculation for 24 Hours promise business logic
            bookingAccommodationStartDateTime = bookingAccommodationStartDateTime.AddDays(1);

            if (bookingAccommodationStartDateTime.CompareTo((DateTime)incident.Attributes["createdon"]) == -1) // Case.CreatedOn <= BookingAccommodation.StartDateTime
            {
                tracingService.Trace(string.Format(@"No Booking Accommodation Start Date or it's less than the Case CreatedOn. \n\n " +
                                        "Booking Accommodation Start Date: {0:MM/dd/yy H:mm:ss} \n\n Query Used: {1}\n", bookingAccommodationStartDateTime,
                                            bookingAccommodationReferenceQuery.ToString()));
                return false;
            }

            return true;
        }
    }
}
