using Microsoft.Xrm.Sdk;
using System;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;
using System.Collections.Generic;

namespace Tc.Crm.CustomWorkflowSteps.ProcessBooking.Services
{
    public static class BookingAccommodationHelper
    {
        public static EntityCollection GetBookingAccommodationEntityFromPayload(Booking bookingInfo, Guid bookingId, ITracingService trace)
        {
            if (bookingInfo == null) return null;
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Accommodation populate records - start");
            string bookingNumber = bookingInfo.BookingIdentifier.BookingNumber;
            var accommodation = bookingInfo.Services.Accommodation;
            if (bookingNumber == null || string.IsNullOrWhiteSpace(bookingNumber))
                throw new InvalidPluginExecutionException("Booking Number should not be null.");

            EntityCollection entityCollectionaccommodation = new EntityCollection();
            if (accommodation == null || accommodation.Length == 0) return entityCollectionaccommodation;

            trace.Trace("Processing " + accommodation.Length.ToString() + " Booking accommodation records - start");
            Entity accommodationEntity = null;
            for (int i = 0; i < accommodation.Length; i++)
            {
                trace.Trace("Processing Booking accommodation " + i.ToString() + " - start");
                accommodationEntity = PrepareBookingAccommodation(bookingInfo, accommodation[i], bookingId, trace);
                entityCollectionaccommodation.Entities.Add(accommodationEntity);
                trace.Trace("Processing Booking accommodation " + i.ToString() + " - end");
            }
            trace.Trace("Processing " + accommodation.Length.ToString() + " Booking accommodation records - end");

            trace.Trace("Accommodation populate records - end");
            return entityCollectionaccommodation;
        }

        private static Entity PrepareBookingAccommodation(Booking bookinginfo, Accommodation accommodation, Guid bookingId, ITracingService trace)
        {
            trace.Trace("Preparing Booking Accommodation information - Start");
            var accommodationEntity = new Entity(EntityName.BookingAccommodation);

            if (accommodation.AccommodationCode != null)
                accommodationEntity[Attributes.BookingAccommodation.SourceMarketHotelCode] = accommodation.AccommodationCode;
            if (!string.IsNullOrEmpty(accommodation.GroupAccommodationCode))
                accommodationEntity[Attributes.BookingAccommodation.HotelId] = new EntityReference(EntityName.Hotel, new Guid(accommodation.GroupAccommodationCode));
            SetNameFor(accommodation, bookinginfo, accommodationEntity);

            accommodationEntity[Attributes.BookingAccommodation.Order] = accommodation.Order.ToString();
            if (!string.IsNullOrWhiteSpace(accommodation.StartDate))
                accommodationEntity[Attributes.BookingAccommodation.StartDateAndTime] = DateTime.Parse(accommodation.StartDate);
            if (!string.IsNullOrWhiteSpace(accommodation.EndDate))
                accommodationEntity[Attributes.BookingAccommodation.EndDateAndTime] = DateTime.Parse(accommodation.EndDate);
            if (!string.IsNullOrWhiteSpace(accommodation.RoomType))
                accommodationEntity[Attributes.BookingAccommodation.RoomType] = accommodation.RoomType;
            accommodationEntity[Attributes.BookingAccommodation.BoardType] = CommonXrm.GetBoardType(accommodation.BoardType);
            accommodationEntity[Attributes.BookingAccommodation.HasSharedRoom] = accommodation.HasSharedRoom;
            accommodationEntity[Attributes.BookingAccommodation.NumberOfParticipants] = accommodation.NumberOfParticipants;
            accommodationEntity[Attributes.BookingAccommodation.NumberOfRooms] = accommodation.NumberOfRooms;
            accommodationEntity[Attributes.BookingAccommodation.WithTransfer] = accommodation.WithTransfer;
            accommodationEntity[Attributes.BookingAccommodation.IsExternalService] = accommodation.IsExternalService;
            accommodationEntity[Attributes.BookingAccommodation.ExternalServiceCode] = CommonXrm.GetExternalServiceCode(accommodation.ExternalServiceCode);
            accommodationEntity[Attributes.BookingAccommodation.NotificationRequired] = accommodation.NotificationRequired;
            accommodationEntity[Attributes.BookingAccommodation.NeedTourGuideAssignment] = accommodation.NeedsTourGuideAssignment;
            accommodationEntity[Attributes.BookingAccommodation.ExternalTransfer] = accommodation.IsExternalTransfer;
            accommodationEntity[Attributes.BookingAccommodation.TransferServiceLevel] = CommonXrm.GetTransferServiceLevel(accommodation.TransferServiceLevel);
            if (!string.IsNullOrWhiteSpace(accommodation.AccommodationDescription))
                accommodationEntity[Attributes.BookingAccommodation.SourceMarketHotelName] = accommodation.AccommodationDescription;
            accommodationEntity[Attributes.BookingAccommodation.BookingId] = new EntityReference(EntityName.Booking, bookingId);
            accommodationEntity[Attributes.BookingAccommodation.Participants] = BookingHelper.PrepareTravelParticipantsInfoForChildRecords(bookinginfo.TravelParticipant, trace, accommodation.TravelParticipantAssignment);
            accommodationEntity[Attributes.Booking.StateCode] = new OptionSetValue((int)Statecode.Active);
            accommodationEntity[Attributes.Booking.StatusCode] = CommonXrm.GetAccommodationStatus(accommodation.Status);
            accommodationEntity[Attributes.Booking.Remarks] = RemarksHelper.GetRemarksTextFromPayload(accommodation.Remark);
            trace.Trace("Preparing Booking Transport information - End");

            return accommodationEntity;
        }

        private static void SetNameFor(Accommodation accommodation, Booking bookinginfo, Entity accommodationEntity)
        {
            var bookingNumber = bookinginfo.BookingIdentifier.BookingNumber;
            accommodationEntity[Attributes.BookingAccommodation.Name] = $"{accommodation.AccommodationDescription} - {bookingNumber}";
        }

    }
}
