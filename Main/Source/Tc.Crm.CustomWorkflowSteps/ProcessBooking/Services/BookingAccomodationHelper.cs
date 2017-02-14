using Microsoft.Xrm.Sdk;
using System;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;
using System.Collections.Generic;

namespace Tc.Crm.CustomWorkflowSteps.ProcessBooking.Services
{
    public static class BookingAccomodationHelper
    {
        public static EntityCollection GetBookingAccomodationEntityFromPayload(Booking booking, Guid bookingId, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Booking Transport get enity main mehod - start");
            if (booking.Services == null) throw new InvalidPluginExecutionException("Booking Services is null.");
            if (booking.Services.Accommodation == null) throw new InvalidPluginExecutionException("Booking Services Accomodation is null.");
            if (booking.BookingIdentifier.BookingNumber == null || string.IsNullOrWhiteSpace(booking.BookingIdentifier.BookingNumber))
                throw new InvalidPluginExecutionException("Booking Number should not be null.");

            Accommodation[] accomodation = booking.Services.Accommodation;
            EntityCollection entityCollectionAccomodation = new EntityCollection();
            Entity accomodationEntity = null;
            for (int i = 0; i < accomodation.Length; i++)
            {
                accomodationEntity = PrepareBookingAccomodation(booking.BookingIdentifier.BookingNumber, accomodation[i], bookingId, trace);
                entityCollectionAccomodation.Entities.Add(accomodationEntity);
            }
            return entityCollectionAccomodation;
        }

        private static Entity PrepareBookingAccomodation(string bookingNumber, Accommodation accomodation, Guid bookingId, ITracingService trace)
        {
            trace.Trace("Preparing Booking Transport information - Start");

            var accomodationEntity = new Entity(EntityName.BookingAccommodation);
            //BN - HotelId 
            if (accomodation.GroupAccommodationCode != null)
            {
                accomodationEntity[Attributes.BookingAccommodation.Name] = bookingNumber + " - " + accomodation.GroupAccommodationCode;
                accomodationEntity[Attributes.BookingAccommodation.HotelId] = new EntityReference(EntityName.Hotel, Attributes.Hotel.MasterHotelID, accomodation.GroupAccommodationCode);
            }
            accomodationEntity[Attributes.BookingAccommodation.Order] = accomodation.Order.ToString();
            if(accomodation.StartDate != null)
            accomodationEntity[Attributes.BookingAccommodation.StartDateandTime] = DateTime.Parse(accomodation.StartDate);
            if(accomodation.EndDate != null)
            accomodationEntity[Attributes.BookingAccommodation.EndDateandTime] = DateTime.Parse(accomodation.EndDate);
            if(accomodation.RoomType != null)
            accomodationEntity[Attributes.BookingAccommodation.RoomType] = accomodation.RoomType;
            accomodationEntity[Attributes.BookingAccommodation.BoardType] = CommonXrm.GetOptionSetValue(accomodation.BoardType.ToString(), Attributes.BookingAccommodation.BoardType);
            accomodationEntity[Attributes.BookingAccommodation.HasSharedRoom] = accomodation.HasSharedRoom;
            accomodationEntity[Attributes.BookingAccommodation.NumberofParticipants] = accomodation.NumberOfParticipants;
            accomodationEntity[Attributes.BookingAccommodation.NumberofRooms] = accomodation.NumberOfRooms;
            accomodationEntity[Attributes.BookingAccommodation.WithTransfer] = accomodation.WithTransfer;
            accomodationEntity[Attributes.BookingAccommodation.IsExternalService] = accomodation.IsExternalService;
            if(accomodation.ExternalServiceCode != null)
            accomodationEntity[Attributes.BookingAccommodation.ExternalServiceCode] = CommonXrm.GetOptionSetValue(accomodation.ExternalServiceCode.ToString(), Attributes.BookingAccommodation.ExternalServiceCode);           
            accomodationEntity[Attributes.BookingAccommodation.NotificationRequired] = accomodation.NotificationRequired;
            accomodationEntity[Attributes.BookingAccommodation.NeedTourGuideAssignment] = accomodation.NeedsTourGuideAssignment;
            accomodationEntity[Attributes.BookingAccommodation.ExternalTransfer] = accomodation.IsExternalTransfer;
            if(accomodation.TransferServiceLevel != null)
            accomodationEntity[Attributes.BookingAccommodation.TransferServiceLevel] = CommonXrm.GetOptionSetValue(accomodation.TransferServiceLevel, Attributes.BookingAccommodation.TransferServiceLevel);
            if(accomodation.AccommodationDescription != null)
            accomodationEntity[Attributes.BookingAccommodation.SourceMarketHotelName] = accomodation.AccommodationDescription;
            accomodationEntity[Attributes.BookingAccommodation.BookingId] = new EntityReference(EntityName.Booking, bookingId);

            trace.Trace("Preparing Booking Transport information - End");

            return accomodationEntity;
        }

    }
}
