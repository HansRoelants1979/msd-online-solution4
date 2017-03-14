using Microsoft.Xrm.Sdk;
using System;
using System.Text;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;
using System.Linq;

namespace Tc.Crm.CustomWorkflowSteps.ProcessBooking.Services
{
    public static class BookingHelper
    {
        public static void PopulateIdentifier(Entity booking, BookingIdentifier identifier, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Booking Identifier - start");
            if (booking == null) throw new InvalidPluginExecutionException("Booking entity is null.");
            if (identifier == null) throw new InvalidPluginExecutionException("Booking identifier is null.");
            if (identifier != null)
            {
                trace.Trace("Booking populate identifier - start");
                booking[Attributes.Booking.Name] = identifier.BookingNumber;
                booking[Attributes.Booking.OnTourVersion] = (identifier.BookingVersionOnTour != null) ? identifier.BookingVersionOnTour : string.Empty;
                booking[Attributes.Booking.TourOperatorVersion] = (identifier.BookingVersionTourOperator != null) ? identifier.BookingVersionTourOperator : string.Empty;
                booking[Attributes.Booking.OnTourUpdatedDate] = (identifier.BookingUpdateDateOnTour != null) ? Convert.ToDateTime(identifier.BookingUpdateDateOnTour) : (DateTime?)null;
                booking[Attributes.Booking.TourOperatorUpdatedDate] = (identifier.BookingUpdateDateTourOperator != null) ? Convert.ToDateTime(identifier.BookingUpdateDateTourOperator) : (DateTime?)null;
                trace.Trace("Booking populate identifier - end");
            }
            trace.Trace("Booking Identifier - end");
        }

        public static void PopulateIdentity(Entity booking, BookingIdentity identity, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Booking Identity - start");
            if (booking == null) throw new InvalidPluginExecutionException("Booking entity is null.");
            if (identity == null || identity.Booker == null) { ClearBookerInformation(booking, trace); trace.Trace("Booking Identity - end"); return; }
            trace.Trace("Booking populate identity - start");
            booking[Attributes.Booking.BookerPhone1] = (identity.Booker.Phone != null) ? identity.Booker.Phone : string.Empty;
            booking[Attributes.Booking.BookerPhone2] = (identity.Booker.Mobile != null) ? identity.Booker.Mobile : string.Empty;
            booking[Attributes.Booking.BookerEmergencyPhone] = (identity.Booker.EmergencyNumber != null) ? identity.Booker.EmergencyNumber : string.Empty;
            booking[Attributes.Booking.BookerEmail] = (identity.Booker.Email != null) ? identity.Booker.Email : string.Empty;
            trace.Trace("Booking populate identity - end");
            trace.Trace("Booking Identity - end");
        }

        private static Attributes.Booking ClearBookerInformation(Entity booking, ITracingService trace)
        {
            trace.Trace("Booking clear identity - start");
            booking[Attributes.Booking.BookerPhone1] = string.Empty;
            booking[Attributes.Booking.BookerPhone2] = string.Empty;
            booking[Attributes.Booking.BookerEmergencyPhone] = string.Empty;
            booking[Attributes.Booking.BookerEmail] = string.Empty;
            trace.Trace("Booking clear identity - end");
            return null;
        }


        public static void PopulateGeneralFields(Entity booking, BookingGeneral general, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Booking general - start");
            if (booking == null) throw new InvalidPluginExecutionException("Booking entity is null.");
            if (general == null) throw new InvalidPluginExecutionException("Booking general information is null.");
            if (general != null)
            {
                trace.Trace("Booking populate general - start");
                booking[Attributes.Booking.BookingDate] = (!string.IsNullOrWhiteSpace(general.BookingDate)) ? Convert.ToDateTime(general.BookingDate) : (DateTime?)null;
                booking[Attributes.Booking.DepartureDate] = (!string.IsNullOrWhiteSpace(general.DepartureDate)) ? Convert.ToDateTime(general.DepartureDate) : (DateTime?)null;
                booking[Attributes.Booking.ReturnDate] = (!string.IsNullOrWhiteSpace(general.ReturnDate)) ? Convert.ToDateTime(general.ReturnDate) : (DateTime?)null;
                booking[Attributes.Booking.Duration] = Convert.ToInt32(general.Duration);
                booking[Attributes.Booking.DestinationGatewayId] = (!string.IsNullOrWhiteSpace(general.Destination)) ? new EntityReference(EntityName.Gateway, Attributes.Gateway.IATA, general.Destination) : null;
                booking[Attributes.Booking.TourOperatorId] = (!string.IsNullOrWhiteSpace(general.ToCode)) ? new EntityReference(EntityName.TourOperator, Attributes.TourOperator.TourOperatorCode, general.ToCode) : null;
                booking[Attributes.Booking.BrandId] = (!string.IsNullOrWhiteSpace(general.Brand)) ? new EntityReference(EntityName.Brand, Attributes.Brand.BrandCode, general.Brand) : null;
                booking[Attributes.Booking.BrochureCode] = (!string.IsNullOrWhiteSpace(general.BrochureCode)) ? general.BrochureCode : string.Empty;
                booking[Attributes.Booking.IsLateBooking] = general.IsLateBooking;
                booking[Attributes.Booking.NumberofParticipants] = general.NumberOfParticipants;
                booking[Attributes.Booking.NumberofAdults] = general.NumberOfAdults;
                booking[Attributes.Booking.NumberofChildren] = general.NumberOfChildren;
                booking[Attributes.Booking.NumberofInfants] = general.NumberOfInfants;
                booking[Attributes.Booking.TravelAmount] = new Money(general.TravelAmount);
                booking[Attributes.Booking.TransactionCurrencyId] = (!string.IsNullOrWhiteSpace(general.Currency)) ? new EntityReference(EntityName.Currency, Attributes.Currency.Name, general.Currency) : null;
                booking[Attributes.Booking.HasSourceMarketComplaint] = general.HasComplaint;
                trace.Trace("Booking populate general - end");
            }
            trace.Trace("Booking general - end");

        }

        public static Entity DeActivateBooking(Booking booking, Guid bookingId, ITracingService trace)
        {
            Entity bookingEntity = null;
            if (booking.BookingGeneral.BookingStatus == BookingStatus.Booked || booking.BookingGeneral.BookingStatus == BookingStatus.Cancelled)
            {
                trace.Trace("Booking record Deactivation - start");
                bookingEntity = new Entity(EntityName.Booking, bookingId);
                bookingEntity[Attributes.Booking.StateCode] = new OptionSetValue((int)Statecode.InActive);
                bookingEntity[Attributes.Booking.StatusCode] = CommonXrm.GetOptionSetValue(booking.BookingGeneral.BookingStatus.ToString(), Attributes.Booking.StatusCode);
                trace.Trace("Booking record Deactivation - end");
            }
            else
            {
                //throw new InvalidPluginExecutionException("Booking status provided in payload is invalid. It an be eiher B or C");
            }
            return bookingEntity;
        }

        public static void PopulateServices(Entity booking, BookingServices service, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            if (service == null) throw new InvalidPluginExecutionException("Booking service is null.");
            if (booking == null) throw new InvalidPluginExecutionException("Booking entiy is null.");
            trace.Trace("Booking populate service - start");
            trace.Trace("Booking populate service - end");
        }

        /// <summary>
        /// To prepare travel participants information
        /// </summary>        
        /// <returns></returns>
        static string PrepareTravelParticipantsInfo(TravelParticipant[] travelParticipants, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Travel Participants information - start");
            if (travelParticipants == null || travelParticipants.Length == 0) return null;
            StringBuilder participantsBuilder = new StringBuilder();
            trace.Trace("Processing " + travelParticipants.Length.ToString() + " Travel Participants information - start");
            for (int i = 0; i < travelParticipants.Length; i++)
            {
                trace.Trace("Processing Travel Participant " + i.ToString() + " information - start");
                StringBuilder participantBuilder = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(travelParticipants[i].TravelParticipantIdOnTour))
                    participantBuilder.Append(travelParticipants[i].TravelParticipantIdOnTour + General.Seperator);
                if (!string.IsNullOrWhiteSpace(travelParticipants[i].FirstName))
                    participantBuilder.Append(travelParticipants[i].FirstName + General.Seperator);
                if (!string.IsNullOrWhiteSpace(travelParticipants[i].LastName))
                    participantBuilder.Append(travelParticipants[i].LastName + General.Seperator);
                participantBuilder.Append(travelParticipants[i].Age.ToString() + General.Seperator);
                if (!string.IsNullOrWhiteSpace(travelParticipants[i].Birthdate))
                    participantBuilder.Append(travelParticipants[i].Birthdate + General.Seperator);
                participantBuilder.Append(travelParticipants[i].Gender.ToString() + General.Seperator);
                participantBuilder.Append(travelParticipants[i].Relation.ToString() + General.Seperator);
                if (!string.IsNullOrWhiteSpace(travelParticipants[i].Language))
                    participantBuilder.Append(travelParticipants[i].Language);
                participantsBuilder.AppendLine(participantBuilder.ToString());
                trace.Trace("Processing Travel Participant " + i.ToString() + " information - end");
            }
            trace.Trace("Processing " + travelParticipants.Length.ToString() + " Travel Participants information - end");
            trace.Trace("Travel Participants information - end");
            return participantsBuilder.ToString();
        }
        public static string PrepareTravelParticipantsInfoForChildRecords(TravelParticipant[] travelParticipants, ITracingService trace, TravelParticipantAssignment[] travelParticipantsAssignment)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Travel Participants information - start");
            if (travelParticipants == null || travelParticipants.Length == 0) return null;
            StringBuilder participantsBuilder = new StringBuilder();
            for (int i = 0; i < travelParticipantsAssignment.Length; i++)
            {
                var Participants = travelParticipants.Where(item => item.TravelParticipantIdOnTour == travelParticipantsAssignment[i].TravelParticipantId)
                                           .Select(item => item).ToArray();

                trace.Trace("Processing " + Participants.Length.ToString() + " Travel Participants information - start");
                for (int j = 0; j < Participants.Length; j++)
                {
                    trace.Trace("Processing Travel Participant " + j.ToString() + " information - start");
                    StringBuilder participantBuilder = new StringBuilder();

                    if (!string.IsNullOrWhiteSpace(Participants[j].FirstName))
                        participantBuilder.Append(Participants[j].FirstName + General.Space);
                    if (!string.IsNullOrWhiteSpace(Participants[j].LastName))
                        participantBuilder.Append(Participants[j].LastName + General.Seperator);

                    participantsBuilder.AppendLine(participantBuilder.ToString());


                    trace.Trace("Processing Travel Participant " + j.ToString() + " information - end");
                }
            }
            trace.Trace("Processing " + travelParticipants.Length.ToString() + " Travel Participants information - end");
            trace.Trace("Travel Participants information - end");
            return participantsBuilder.ToString().Remove(participantsBuilder.Length - 3);
        }

        /// <summary>
        /// To prepare travel participants remarks information
        /// </summary>       
        /// <returns></returns>
        static string PrepareTravelParticipantsRemarks(TravelParticipant[] travelParticipants, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Booking populate participants remarks - start");
            if (travelParticipants == null || travelParticipants.Length == 0) return string.Empty;
            StringBuilder remarksbuilder = new StringBuilder();
            trace.Trace("Processing " + travelParticipants.Length.ToString() + " Travel Participant Remarks information - start");
            for (int i = 0; i < travelParticipants.Length; i++)
            {
                if (travelParticipants[i].Remark == null || travelParticipants[i].Remark.Length == 0) continue;
                trace.Trace("Processing " + i.ToString() + " travel participant and its " + travelParticipants[i].Remark.Length.ToString() + " travel participant remarks - start");
                for (int j = 0; j < travelParticipants[i].Remark.Length; j++)
                {
                    if (travelParticipants[i].Remark[j] == null) continue;
                    StringBuilder remarkbuilder = new StringBuilder();
                    trace.Trace("Processing Remark " + j.ToString() + " information - start");
                    if (!string.IsNullOrWhiteSpace(travelParticipants[i].TravelParticipantIdOnTour))
                        remarkbuilder.Append(travelParticipants[i].TravelParticipantIdOnTour + General.Seperator);
                    remarkbuilder.Append(travelParticipants[i].Remark[j].RemarkType.ToString() + General.Seperator);
                    if (!string.IsNullOrWhiteSpace(travelParticipants[i].Remark[j].Text))
                        remarkbuilder.Append(travelParticipants[i].Remark[j].Text);

                    remarksbuilder.AppendLine(remarkbuilder.ToString());
                    trace.Trace("Processing Remark " + j.ToString() + " information - end");
                }
                trace.Trace("Processing " + i.ToString() + " travel participant and its " + travelParticipants[i].Remark.Length.ToString() + " travel participant remarks - end");

            }
            trace.Trace("Processing " + travelParticipants.Length.ToString() + " Travel Participant Remarks information - start");
            trace.Trace("Booking populate participants remarks - end");
            return remarksbuilder.ToString();
        }

        static EntityReference SetBookingDestination(Accommodation[] accommodatations, ITracingService trace, IOrganizationService service)
        {
            EntityReference Destination = null;
            EntityCollection RegionCollection = null;
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace(" Booking Destination information - start");
            if (accommodatations == null || accommodatations.Length == 0) return null;


            var FirstAccomdation = accommodatations.Where(item => item.Order == 1)
                                       .Select(item => item).ToArray();
            if (FirstAccomdation != null && FirstAccomdation.Length == 1 && FirstAccomdation[0].GroupAccommodationCode != null && FirstAccomdation[0].GroupAccommodationCode != "")
            {
                var query = string.Format(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                                <entity name='tc_region'>
                               <attribute name='tc_name' />
                               <attribute name='tc_regioncode' />
                               <attribute name='tc_countryid' />
                               <attribute name='tc_regionid' />
                               <order attribute='tc_name' descending='false' />
                              <link-entity name='tc_location' from='tc_regionid' to='tc_regionid' alias='af'>
                            <link-entity name='tc_hotel' from='tc_locationid' to='tc_locationid' alias='ag'>
                             <filter type='and'>
                             <condition attribute='tc_masterhotelid' operator='eq' value='{0}' />
                          </filter>
                          </link-entity>
                          </link-entity>
                          </entity>
                          </fetch>",
                                         new object[] { FirstAccomdation[0].GroupAccommodationCode,
                                                      });


                RegionCollection = CommonXrm.RetrieveMultipleRecordsFetchXml(query, service);
            }

            if (RegionCollection != null && RegionCollection.Entities != null && RegionCollection.Entities.Count > 0)
            {

                Destination = new EntityReference(EntityName.Region, RegionCollection.Entities[0].Id);


            }

            return Destination;
        }
        static EntityReference SetBookingOwner(string sourceMarket, ITracingService trace, IOrganizationService service)
        {
            EntityReference Owner = null;
            EntityCollection Team = null;

            if ((!string.IsNullOrEmpty(sourceMarket)))
            {
                var query = string.Format(@"<fetch distinct='true' mapping='logical' output-format='xml-platform' version='1.0'>


                          <entity name='team'>

                          <attribute name='name'/>

                         <attribute name='businessunitid'/>

                        <attribute name='teamid'/>

                      <attribute name='teamtype'/>

                     <order descending='false' attribute='name'/>
                      <filter type='and'>

                 <condition attribute='isdefault' value='1' operator='eq'/>

                </filter>

            <link-entity name='businessunit' alias='ac' to='businessunitid' from='businessunitid'>
          <link-entity name='tc_country' alias='ad' to='businessunitid' from='tc_sourcemarketbusinessunitid'>
               <filter type='and'>
               <condition attribute='tc_iso2code' value='{0}' operator='eq'/>

               </filter>

           </link-entity>

           </link-entity>

            </entity>

            </fetch>", new object[] { sourceMarket,
             });

                Team = CommonXrm.RetrieveMultipleRecordsFetchXml(query, service);

            }

            if (Team != null && Team.Entities != null && Team.Entities.Count > 0)
            {

                Owner = new EntityReference(EntityName.Team, Team.Entities[0].Id);


            }


            return Owner;
        }
        /// <summary>
        /// To prepare transfer information
        /// </summary>     
        /// <returns></returns>
        static string PrepareTransferInfo(Transfer[] transfers, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Booking populate transfers - start");
            if (transfers == null || transfers.Length == 0) return string.Empty;

            StringBuilder transfersBuilder = new StringBuilder();
            trace.Trace("Processing " + transfers.Length.ToString() + " transfers - start");
            for (int i = 0; i < transfers.Length; i++)
            {
                StringBuilder transferBuilder = new StringBuilder();
                trace.Trace("Processing transfer " + i.ToString() + " - start");
                if (!string.IsNullOrWhiteSpace(transfers[i].TransferCode))
                    transferBuilder.Append(transfers[i].TransferCode + General.Seperator);
                if (!string.IsNullOrWhiteSpace(transfers[i].TransferDescription))
                    transferBuilder.Append(transfers[i].TransferDescription + General.Seperator);
                transferBuilder.Append(transfers[i].Order.ToString() + General.Seperator);
                if (!string.IsNullOrWhiteSpace(transfers[i].StartDate))
                    transferBuilder.Append(transfers[i].StartDate + General.Seperator);
                if (!string.IsNullOrWhiteSpace(transfers[i].EndDate))
                    transferBuilder.Append(transfers[i].EndDate + General.Seperator);
                if (!string.IsNullOrWhiteSpace(transfers[i].Category))
                    transferBuilder.Append(transfers[i].Category + General.Seperator);
                transferBuilder.Append(transfers[i].TransferType.ToString() + General.Seperator);
                if (!string.IsNullOrWhiteSpace(transfers[i].DepartureAirport))
                    transferBuilder.Append(transfers[i].DepartureAirport + General.Seperator);
                if (!string.IsNullOrWhiteSpace(transfers[i].ArrivalAirport))
                    transferBuilder.Append(transfers[i].ArrivalAirport);

                transfersBuilder.AppendLine(transferBuilder.ToString());
                trace.Trace("Processing transfer " + i.ToString() + " - end");
            }
            trace.Trace("Processing " + transfers.Length.ToString() + " transfers - end");

            trace.Trace("Booking populate transfers - end");
            return transfersBuilder.ToString();
        }

        /// <summary>
        /// To prepare trasnfer remarks information
        /// </summary>  
        /// <returns></returns>
        static string PrepareTransferRemarks(Transfer[] transfers, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Booking populate transfer remarks - start");
            if (transfers == null || transfers.Length == 0) return string.Empty;

            StringBuilder remarksBuilder = new StringBuilder();

            for (int i = 0; i < transfers.Length; i++)
            {
                if (transfers[i].Remark == null) continue;
                trace.Trace("Processing " + i.ToString() + " Transfer and its " + transfers[i].Remark.Length.ToString() + " Transfer service remarks - start");
                for (int j = 0; j < transfers[i].Remark.Length; j++)
                {
                    StringBuilder remarkBuilder = new StringBuilder();
                    trace.Trace("Processing Transfer Remark " + j.ToString() + " information - start");
                    remarkBuilder.Append(transfers[i].Remark[j].RemarkType.ToString() + General.Seperator);
                    if (!string.IsNullOrWhiteSpace(transfers[i].Remark[j].Text))
                        remarkBuilder.Append(transfers[i].Remark[j].Text);
                    remarksBuilder.AppendLine(remarkBuilder.ToString());
                    trace.Trace("Processing Transfer Remark " + j.ToString() + " information - end");
                }
                trace.Trace("Processing " + i.ToString() + " Transfer and its " + transfers[i].Remark.Length.ToString() + " Transfer service remarks - end");
            }
            trace.Trace("Booking populate transfer remarks - end");
            return remarksBuilder.ToString();
        }


        /// <summary>
        /// To prepare extraservice information
        /// </summary>       
        /// <returns></returns>
        static string PrepareExtraServicesInfo(ExtraService[] extraServices, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Booking populate extra services - start");
            if (extraServices == null || extraServices.Length == 0) return string.Empty;

            StringBuilder extraServicesBuilder = new StringBuilder();
            trace.Trace("Booking populate " + extraServices.Length.ToString() + " extra services - start");
            for (int i = 0; i < extraServices.Length; i++)
            {
                StringBuilder extraServiceBuilder = new StringBuilder();
                trace.Trace("Processing extra service " + i.ToString() + " - start");
                if (!string.IsNullOrWhiteSpace(extraServices[i].ExtraServiceCode))
                    extraServiceBuilder.Append(extraServices[i].ExtraServiceCode + General.Seperator);
                if (!string.IsNullOrWhiteSpace(extraServices[i].ExtraServiceDescription))
                    extraServiceBuilder.Append(extraServices[i].ExtraServiceDescription + General.Seperator);
                extraServiceBuilder.Append(extraServices[i].Order.ToString() + General.Seperator);
                if (!string.IsNullOrWhiteSpace(extraServices[i].StartDate))
                    extraServiceBuilder.Append(extraServices[i].StartDate + General.Seperator);
                if (!string.IsNullOrWhiteSpace(extraServices[i].EndDate))
                    extraServiceBuilder.Append(extraServices[i].EndDate);

                extraServicesBuilder.AppendLine(extraServiceBuilder.ToString());
                trace.Trace("Processing extra service " + i.ToString() + " - End");
            }
            trace.Trace("Booking populate " + extraServices.Length.ToString() + " extra services - end");
            trace.Trace("Booking populate extra services - end");
            return extraServicesBuilder.ToString();
        }

        /// <summary>
        /// To prepare extra service remarks information
        /// </summary>     
        /// <returns></returns>
        static string PrepareExtraServiceRemarks(ExtraService[] extraServices, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Booking populate extra service remarks - start");
            if (extraServices == null || extraServices.Length == 0) return string.Empty;
            StringBuilder remarksBuilder = new StringBuilder();
            for (int i = 0; i < extraServices.Length; i++)
            {
                if (extraServices[i].Remark == null) continue;
                trace.Trace("Processing " + i.ToString() + " extra service and its " + extraServices[i].Remark.Length.ToString() + " extra service remarks - start");
                for (int j = 0; j < extraServices[i].Remark.Length; j++)
                {
                    StringBuilder remarkBuilder = new StringBuilder();
                    trace.Trace("Processing Extra Service Remark " + j.ToString() + " information - start");
                    remarkBuilder.Append(extraServices[i].Remark[j].RemarkType.ToString() + General.Seperator);
                    if (!string.IsNullOrWhiteSpace(extraServices[i].Remark[j].Text ))
                        remarkBuilder.Append(extraServices[i].Remark[j].Text);
                    remarksBuilder.AppendLine(remarkBuilder.ToString());
                    trace.Trace("Processing Extra Service Remark " + j.ToString() + " information - end");
                }
                trace.Trace("Processing " + i.ToString() + " extra service and its " + extraServices[i].Remark.Length.ToString() + " extra service remarks - end");

            }
            trace.Trace("Booking populate extra service remarks - start");
            return remarksBuilder.ToString();
        }

        public static Entity GetBookingEntityFromPayload(Booking booking, ITracingService trace, IOrganizationService service)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Booking populate fields - start");
            if (booking == null) throw new InvalidPluginExecutionException("Booking is null.");
            if (booking.BookingIdentifier == null) throw new InvalidPluginExecutionException("Booking identifier is null.");
            if (booking.BookingIdentifier.BookingNumber == null || string.IsNullOrWhiteSpace(booking.BookingIdentifier.BookingNumber))
                throw new InvalidPluginExecutionException("Booking Number should not be null.");

            Entity bookingEntity = new Entity(EntityName.Booking, Attributes.Booking.Name, booking.BookingIdentifier.BookingNumber);

            PopulateIdentifier(bookingEntity, booking.BookingIdentifier, trace);
            PopulateIdentity(bookingEntity, booking.BookingIdentity, trace);
            PopulateGeneralFields(bookingEntity, booking.BookingGeneral, trace);
            bookingEntity[Attributes.Booking.Participants] = PrepareTravelParticipantsInfo(booking.TravelParticipant, trace);
            bookingEntity[Attributes.Booking.ParticipantRemarks] = PrepareTravelParticipantsRemarks(booking.TravelParticipant, trace);
            bookingEntity[Attributes.Booking.DestinationId] = SetBookingDestination(booking.Services.Accommodation, trace, service);
            bookingEntity[Attributes.Booking.Owner] = SetBookingOwner(booking.BookingIdentifier.SourceMarket, trace, service);
            PopulateServices(bookingEntity, booking.Services, trace);

            bookingEntity[Attributes.Booking.SourceMarketId] = (booking.BookingIdentifier.SourceMarket != null) ? new EntityReference(EntityName.Country
                                                                                    , Attributes.Country.ISO2Code
                                                                                    , booking.BookingIdentifier.SourceMarket)
                                                                                    : null;

            trace.Trace("Booking populate fields - end");

            return bookingEntity;

        }
    }
}

