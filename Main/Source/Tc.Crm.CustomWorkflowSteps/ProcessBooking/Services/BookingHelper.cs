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
                booking[Attributes.Booking.OnTourUpdatedDate] = !string.IsNullOrWhiteSpace(identifier.BookingUpdateDateOnTour) ? Convert.ToDateTime(identifier.BookingUpdateDateOnTour) : (DateTime?)null;
                booking[Attributes.Booking.TourOperatorUpdatedDate] = !string.IsNullOrWhiteSpace(identifier.BookingUpdateDateTourOperator) ? Convert.ToDateTime(identifier.BookingUpdateDateTourOperator) : (DateTime?)null;
                booking[Attributes.Booking.SourceApplication] = identifier.SourceApplication != SourceApplication.NotSpecified ? identifier.SourceApplication.ToString() : null;
                booking[Attributes.Booking.SourceSystem] = identifier.BookingSystem.ToString();
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

        private static void ClearBookerInformation(Entity booking, ITracingService trace)
        {
            trace.Trace("Booking clear identity - start");
            booking[Attributes.Booking.BookerPhone1] = string.Empty;
            booking[Attributes.Booking.BookerPhone2] = string.Empty;
            booking[Attributes.Booking.BookerEmergencyPhone] = string.Empty;
            booking[Attributes.Booking.BookerEmail] = string.Empty;
            trace.Trace("Booking clear identity - end");
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
                booking[Attributes.Booking.DestinationGatewayId] = (!string.IsNullOrWhiteSpace(general.Destination)) ? new EntityReference(EntityName.Gateway, new Guid(general.Destination)) : null;
                booking[Attributes.Booking.TourOperatorId] = (!string.IsNullOrWhiteSpace(general.ToCode)) ? new EntityReference(EntityName.TourOperator, new Guid(general.ToCode)) : null;
                booking[Attributes.Booking.BrandId] = (!string.IsNullOrWhiteSpace(general.Brand)) ? new EntityReference(EntityName.Brand, new Guid(general.Brand)) : null;
                booking[Attributes.Booking.BrochureCode] = (!string.IsNullOrWhiteSpace(general.BrochureCode)) ? general.BrochureCode : string.Empty;
                booking[Attributes.Booking.IsLateBooking] = general.IsLateBooking;
                booking[Attributes.Booking.NumberOfParticipants] = general.NumberOfParticipants;
                booking[Attributes.Booking.NumberOfAdults] = general.NumberOfAdults;
                booking[Attributes.Booking.NumberOfChildren] = general.NumberOfChildren;
                booking[Attributes.Booking.NumberOfInfants] = general.NumberOfInfants;
                booking[Attributes.Booking.TravelAmount] = new Money(general.TravelAmount);
                if (!string.IsNullOrWhiteSpace(general.Currency))
                    booking[Attributes.Booking.TransactionCurrencyId] = new EntityReference(EntityName.Currency, new Guid(general.Currency));
                booking[Attributes.Booking.HasSourceMarketComplaint] = general.HasComplaint;
                trace.Trace("Booking populate general - end");
            }
            trace.Trace("Booking general - end");

        }

        public static void PopulateServices(Entity booking, BookingServices service, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            if (service == null) return;
            if (booking == null) throw new InvalidPluginExecutionException("Booking entity is null.");
            trace.Trace("Booking populate service - start");
            trace.Trace("Booking populate service - end");
        }

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
                    participantBuilder.Append(travelParticipants[i].TravelParticipantIdOnTour + General.Separator);
                if (!string.IsNullOrWhiteSpace(travelParticipants[i].FirstName))
                    participantBuilder.Append(travelParticipants[i].FirstName + General.Separator);
                if (!string.IsNullOrWhiteSpace(travelParticipants[i].LastName))
                    participantBuilder.Append(travelParticipants[i].LastName + General.Separator);
                if (!string.IsNullOrWhiteSpace(travelParticipants[i].Age))
                    participantBuilder.Append(travelParticipants[i].Age.ToString() + General.Separator);
                if (!string.IsNullOrWhiteSpace(travelParticipants[i].Birthdate))
                    participantBuilder.Append(travelParticipants[i].Birthdate + General.Separator);
                participantBuilder.Append(travelParticipants[i].Gender.ToString() + General.Separator);
                participantBuilder.Append(travelParticipants[i].Relation.ToString() + General.Separator);
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
            if (travelParticipantsAssignment == null || travelParticipantsAssignment.Length == 0) return string.Empty;
            trace.Trace("Travel Participants information - start");
            if (travelParticipants == null || travelParticipants.Length == 0) return null;
            StringBuilder participantsBuilder = new StringBuilder();
            for (int i = 0; i < travelParticipantsAssignment.Length; i++)
            {
                var Participants = travelParticipants.Where(item => item.TravelParticipantIdOnTour == travelParticipantsAssignment[i].TravelParticipantId)
                                           .Select(item => item).ToArray();
                if (Participants == null || Participants.Length == 0)
                {
                    trace.Trace("Possible mismatch in travel participant id");
                    continue;
                }


                trace.Trace("Processing " + Participants.Length.ToString() + " Travel Participants information - start");
                for (int j = 0; j < Participants.Length; j++)
                {
                    trace.Trace("Processing Travel Participant " + j.ToString() + " information - start");
                    StringBuilder participantBuilder = new StringBuilder();

                    if (!string.IsNullOrWhiteSpace(Participants[j].FirstName))
                        participantBuilder.Append(Participants[j].FirstName + General.Space);
                    if (!string.IsNullOrWhiteSpace(Participants[j].LastName))
                        participantBuilder.Append(Participants[j].LastName + General.Separator);

                    participantsBuilder.AppendLine(participantBuilder.ToString());


                    trace.Trace("Processing Travel Participant " + j.ToString() + " information - end");
                }
            }
            trace.Trace("Processing " + travelParticipants.Length.ToString() + " Travel Participants information - end");
            trace.Trace("Travel Participants information - end");
            if (participantsBuilder.Length == 0) return null;
            return participantsBuilder.ToString().Remove(participantsBuilder.Length - 3);
        }

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
                        remarkbuilder.Append(travelParticipants[i].TravelParticipantIdOnTour + General.Separator);
                    remarkbuilder.Append(travelParticipants[i].Remark[j].RemarkType.ToString() + General.Separator);
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

        public static Entity GetBookingEntityFromPayload(Booking booking, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Trace service is null;");
            trace.Trace("Booking populate fields - start");
            if (booking == null) throw new InvalidPluginExecutionException("Booking is null.");
            if (booking.BookingIdentifier == null) throw new InvalidPluginExecutionException("Booking identifier is null.");
            if (booking.BookingIdentifier.BookingNumber == null || string.IsNullOrWhiteSpace(booking.BookingIdentifier.BookingNumber))
                throw new InvalidPluginExecutionException("Booking Number should not be null.");

            var indexCollection = new KeyAttributeCollection();
            indexCollection.Add(Attributes.Booking.Name, booking.BookingIdentifier.BookingNumber);
            indexCollection.Add(Attributes.Booking.SourceSystem, booking.BookingIdentifier.BookingSystem.ToString());
            Entity bookingEntity = new Entity(EntityName.Booking, indexCollection);

            PopulateIdentifier(bookingEntity, booking.BookingIdentifier, trace);
            PopulateIdentity(bookingEntity, booking.BookingIdentity, trace);
            PopulateGeneralFields(bookingEntity, booking.BookingGeneral, trace);

            bookingEntity[Attributes.Booking.Participants] = PrepareTravelParticipantsInfo(booking.TravelParticipant, trace);
            bookingEntity[Attributes.Booking.ParticipantRemarks] = PrepareTravelParticipantsRemarks(booking.TravelParticipant, trace);
            if (!string.IsNullOrWhiteSpace(booking.DestinationId))
                bookingEntity[Attributes.Booking.DestinationId] = new EntityReference(EntityName.Region, new Guid(booking.DestinationId));
            else
                bookingEntity[Attributes.Booking.DestinationId] = null;

            if (booking.BookingIdentifier != null)
            {
                if (!string.IsNullOrWhiteSpace(booking.Owner))
                    bookingEntity[Attributes.Booking.Owner] = new EntityReference(EntityName.Team, new Guid(booking.Owner));
            }

            PopulateServices(bookingEntity, booking.Services, trace);

            bookingEntity[Attributes.Booking.SourceMarketId] = (booking.BookingIdentifier.SourceMarket != null) ? new EntityReference(EntityName.Country
                                                                                    , new Guid(booking.BookingIdentifier.SourceMarket))
                                                                                    : null;

            bookingEntity[Attributes.Booking.StateCode] = new OptionSetValue((int)Statecode.Active);
            bookingEntity[Attributes.Booking.StatusCode] = CommonXrm.GetBookingStatus(booking.BookingGeneral.BookingStatus);
            bookingEntity[Attributes.Booking.Remarks] = RemarksHelper.GetRemarksTextFromPayload(booking.Remark);
            trace.Trace("Booking populate fields - end");

            return bookingEntity;

        }
    }
}

