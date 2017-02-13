using Microsoft.Xrm.Sdk;
using System;
using System.Text;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;

namespace Tc.Crm.CustomWorkflowSteps.ProcessBooking.Services
{
    public static class BookingHelper
    {
        public static void PopulateIdentifier(Entity booking, BookingIdentifier identifier, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Booking populate identifier - start");
            if (booking == null) throw new InvalidPluginExecutionException("Booking entity is null.");
            if (identifier == null) throw new InvalidPluginExecutionException("Booking identifier is null.");
            booking[Attributes.Booking.Name] = identifier.BookingNumber;
            booking[Attributes.Booking.OnTourVersion] = identifier.BookingVersionOnTour;
            booking[Attributes.Booking.TourOperatorVersion] = identifier.BookingVersionTourOperator;
            booking[Attributes.Booking.OnTourUpdatedDate] = Convert.ToDateTime(identifier.BookingUpdateDateOnTour);
            booking[Attributes.Booking.TourOperatorUpdatedDate] = Convert.ToDateTime(identifier.BookingUpdateDateTourOperator);
            trace.Trace("Booking populate identifier - end");
        }

        public static void PopulateIdentity(Entity booking, BookingIdentity identity, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Booking populate identity - start");
            if (booking == null) throw new InvalidPluginExecutionException("Booking entity is null.");
            if (identity == null) throw new InvalidPluginExecutionException("Booking identity information is null.");
            if (identity.Booker == null) { ClearBookerInformation(booking); return; }
            booking[Attributes.Booking.BookerPhone1] = identity.Booker.Phone;
            booking[Attributes.Booking.BookerPhone2] = identity.Booker.Mobile;
            booking[Attributes.Booking.BookerEmergencyPhone] = identity.Booker.EmergencyNumber;
            booking[Attributes.Booking.BookerEmail] = identity.Booker.Email;
            trace.Trace("Booking populate identity - end");
        }

        private static Attributes.Booking ClearBookerInformation(Entity booking)
        {
            booking[Attributes.Booking.BookerPhone1] = string.Empty;
            booking[Attributes.Booking.BookerPhone2] = string.Empty;
            booking[Attributes.Booking.BookerEmergencyPhone] = string.Empty;
            booking[Attributes.Booking.BookerEmail] = string.Empty;

            return null;
        }


        public static void PopulateGeneralFields(Entity booking, BookingGeneral general, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Booking populate general - start");
            if (booking == null) throw new InvalidPluginExecutionException("Booking entity is null.");
            if (general == null) throw new InvalidPluginExecutionException("Booking general information is null.");
            booking[Attributes.Booking.BookingDate] = Convert.ToDateTime(general.BookingDate);
            booking[Attributes.Booking.DepartureDate] = Convert.ToDateTime(general.DepartureDate);
            booking[Attributes.Booking.ReturnDate] = Convert.ToDateTime(general.ReturnDate);
            booking[Attributes.Booking.Duration] = Convert.ToInt64(general.Duration);
            if (general.Destination != null)
            {
                booking[Attributes.Booking.DestinationGatewayId] = new EntityReference(EntityName.Gateway, Attributes.Gateway.IATA, general.Destination);
            }
            if (general.ToCode != null)
            {
                booking[Attributes.Booking.TourOperatorId] = new EntityReference(EntityName.TourOperator, Attributes.TourOperator.TourOperatorCode, general.ToCode);
            }
            if (general.Brand != null)
            {
                booking[Attributes.Booking.BrandId] = new EntityReference(EntityName.Brand, Attributes.Brand.BrandCode, general.Brand);
            }
            booking[Attributes.Booking.BrochureCode] = general.BrochureCode;
            booking[Attributes.Booking.IsLateBooking] = general.IsLateBooking;
            booking[Attributes.Booking.NumberofParticipants] = general.NumberOfParticipants;
            booking[Attributes.Booking.NumberofAdults] = general.NumberOfAdults;
            booking[Attributes.Booking.NumberofChildren] = general.NumberOfChildren;
            booking[Attributes.Booking.NumberofInfants] = general.NumberOfInfants;
            booking[Attributes.Booking.TravelAmount] = new Money(general.TravelAmount);
            booking[Attributes.Booking.TransactionCurrencyId] = new EntityReference(EntityName.Currency, Attributes.Currency.Name, general.Currency);
            booking[Attributes.Booking.HasSourceMarketComplaint] = general.HasComplaint;
            trace.Trace("Booking populate general - end");

        }

        public static Entity DeActivateBooking(Booking booking, Guid bookingId, ITracingService trace)
        {
            Entity bookingEntity = null;
            if (booking.BookingGeneral.BookingStatus == BookingStatus.B || booking.BookingGeneral.BookingStatus == BookingStatus.C)
            {
                trace.Trace("Processing Booking record DeActivation.");
                bookingEntity = new Entity(EntityName.Booking, bookingId);
                bookingEntity[Attributes.Booking.StateCode] = new OptionSetValue((int)Statecode.InActive);
                bookingEntity[Attributes.Booking.StatusCode] = CommonXrm.GetOptionSetValue(booking.BookingGeneral.BookingStatus.ToString(), Attributes.Booking.StatusCode);
            }
            else
            {
                //throw new InvalidPluginExecutionException("Booking status provided in payload is invalid. It an be eiher B or C");
            }
            return bookingEntity;
        }

        public static void PopulateServices(Entity booking,BookingServices service, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Booking populate service - start");
            if (service == null) throw new InvalidPluginExecutionException("Booking service is null.");
            if (booking == null) throw new InvalidPluginExecutionException("Booking entiy is null.");

            booking[Attributes.Booking.Transfer] = PrepareTransferInfo(service.Transfer, trace);
            booking[Attributes.Booking.TransferRemarks] = PrepareTransferRemarks(service.Transfer, trace);
            //booking[Attributes.Booking.ExtraService] = PrepareExtraServicesInfo(service.ExtraService, trace);
            //booking[Attributes.Booking.ExtraServiceRemarks] = PrepareExtraServiceRemarks(service.ExtraService, trace);
            
            trace.Trace("Booking populate service - end");
        }
        /// <summary>
        /// To prepare travel participants information
        /// </summary>        
        /// <returns></returns>
        static string PrepareTravelParticipantsInfo(TravelParticipant[] travelParticipants, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Booking populate participants - start");
            if (travelParticipants == null || travelParticipants.Length == 0) return null;

            StringBuilder participantsBuilder = new StringBuilder();

            trace.Trace("Processing Travel Participants information");
            for (int i = 0; i < travelParticipants.Length; i++)
            {
                StringBuilder participantBuilder = new StringBuilder();
                participantBuilder.Append(travelParticipants[i].TravelParticipantIdOnTour + General.Seperator);
                participantBuilder.Append(travelParticipants[i].FirstName + General.Seperator);
                participantBuilder.Append(travelParticipants[i].LastName + General.Seperator);
                participantBuilder.Append(travelParticipants[i].Age.ToString() + General.Seperator);
                participantBuilder.Append(travelParticipants[i].Birthdate + General.Seperator);
                participantBuilder.Append(travelParticipants[i].Gender.ToString() + General.Seperator);
                participantBuilder.Append(travelParticipants[i].Relation.ToString() + General.Seperator);
                participantBuilder.Append(travelParticipants[i].Language);

                participantsBuilder.AppendLine(participantBuilder.ToString());
            }

            return participantsBuilder.ToString();
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

            trace.Trace("Processing Travel Participant Remarks information");
            for (int i = 0; i < travelParticipants.Length; i++)
            {
                if (travelParticipants[i].Remark == null || travelParticipants[i].Remark.Length == 0) continue;
                for (int j = 0; j < travelParticipants[i].Remark.Length; j++)
                {
                    if (travelParticipants[i].Remark[j] == null) continue;
                    StringBuilder remarkbuilder = new StringBuilder();

                    remarkbuilder.Append(travelParticipants[i].TravelParticipantIdOnTour + General.Seperator);
                    remarkbuilder.Append(travelParticipants[i].Remark[j].RemarkType.ToString() + General.Seperator);
                    remarkbuilder.Append(travelParticipants[i].Remark[j].Text);

                    remarksbuilder.AppendLine(remarkbuilder.ToString());
                }

            }
            trace.Trace("Booking populate participants remarks - end");
            return remarksbuilder.ToString();
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

            for (int i = 0; i < transfers.Length; i++)
            {
                StringBuilder transferBuilder = new StringBuilder();

                transferBuilder.Append(transfers[i].TransferCode + General.Seperator);
                transferBuilder.Append(transfers[i].TransferDescription + General.Seperator);
                transferBuilder.Append(transfers[i].Order.ToString() + General.Seperator);
                transferBuilder.Append(transfers[i].StartDate + General.Seperator);
                transferBuilder.Append(transfers[i].EndDate + General.Seperator);
                transferBuilder.Append(transfers[i].Category + General.Seperator);
                transferBuilder.Append(transfers[i].TransferType.ToString() + General.Seperator);
                transferBuilder.Append(transfers[i].DepartureAirport + General.Seperator);
                transferBuilder.Append(transfers[i].ArrivalAirport);

                transfersBuilder.AppendLine(transferBuilder.ToString());
            }

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
                for (int j = 0; j < transfers[i].Remark.Length; j++)
                {
                    StringBuilder remarkBuilder = new StringBuilder();
                    remarkBuilder.Append(transfers[i].Remark[j].RemarkType.ToString() + General.Seperator);
                    remarkBuilder.Append(transfers[i].Remark[j].Text);

                    remarksBuilder.AppendLine(remarkBuilder.ToString());
                }

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

            for (int i = 0; i < extraServices.Length; i++)
            {
                StringBuilder extraServiceBuilder = new StringBuilder();
                extraServiceBuilder.Append(extraServices[i].ExtraServiceCode.ToString() + General.Seperator);
                extraServiceBuilder.Append(extraServices[i].ExtraServiceDescription.ToString() + General.Seperator);
                extraServiceBuilder.Append(extraServices[i].Order.ToString() + General.Seperator);
                extraServiceBuilder.Append(extraServices[i].StartDate + General.Seperator);
                extraServiceBuilder.Append(extraServices[i].EndDate);

                extraServicesBuilder.AppendLine(extraServiceBuilder.ToString());
            }
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
                for (int j = 0; j < extraServices[i].Remark.Length; j++)
                {
                    StringBuilder remarkBuilder = new StringBuilder();
                    remarkBuilder.Append(extraServices[i].Remark[j].RemarkType.ToString() + General.Seperator);
                    remarkBuilder.Append(extraServices[i].Remark[j].Text);

                    remarksBuilder.AppendLine(remarkBuilder.ToString());

                }

            }
            trace.Trace("Booking populate extra service remarks - start");
            return remarksBuilder.ToString();
        }

        public static Entity GetBookingEntityFromPayload(Booking booking, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Booking get enity main mehod - start");
            if (booking == null) throw new InvalidPluginExecutionException("Booking is null.");
            if (booking.BookingIdentifier == null) throw new InvalidPluginExecutionException("Booking identifier is null.");
            
            Entity bookingEntity = new Entity(EntityName.Booking, Attributes.Booking.Name, booking.BookingIdentifier.BookingNumber);

            PopulateIdentifier(bookingEntity, booking.BookingIdentifier, trace);
            PopulateIdentity(bookingEntity, booking.BookingIdentity, trace);
            PopulateGeneralFields(bookingEntity, booking.BookingGeneral, trace);
            bookingEntity[Attributes.Booking.Participants] = PrepareTravelParticipantsInfo(booking.TravelParticipant, trace);
            bookingEntity[Attributes.Booking.ParticipantRemarks] = PrepareTravelParticipantsRemarks(booking.TravelParticipant, trace);

            PopulateServices(bookingEntity, booking.Services, trace);

            bookingEntity[Attributes.Booking.SourceMarketId] = new EntityReference(EntityName.Country
                                                                                    , Attributes.Country.ISO2Code
                                                                                    , booking.BookingIdentifier.SourceMarket);

            trace.Trace("Booking get enity main mehod - end");

            return bookingEntity;

        }
    }
}
