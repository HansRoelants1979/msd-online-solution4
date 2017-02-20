using Microsoft.Xrm.Sdk;
using System;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;
using System.Collections.Generic;

namespace Tc.Crm.CustomWorkflowSteps.ProcessBooking.Services
{
    public static class BookingAccommodationHelper
    {
        public static EntityCollection GetBookingAccommodationEntityFromPayload(Accommodation[] accommodation, string bookingNumber, Guid bookingId, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Accommodation populate records - start");           
            if (bookingNumber == null || string.IsNullOrWhiteSpace(bookingNumber))
                throw new InvalidPluginExecutionException("Booking Number should not be null.");
            EntityCollection entityCollectionaccommodation = new EntityCollection();
            if (accommodation != null && accommodation.Length > 0)
            {               
                trace.Trace("Processing "+ accommodation.Length .ToString()+ " Booking accommodation records - start");
                Entity accommodationEntity = null;
                for (int i = 0; i < accommodation.Length; i++)
                {
                    trace.Trace("Processing Booking accommodation " + i.ToString() + " - start");
                    accommodationEntity = PrepareBookingAccommodation(bookingNumber, accommodation[i], bookingId, trace);
                    entityCollectionaccommodation.Entities.Add(accommodationEntity);
                    trace.Trace("Processing Booking accommodation " + i.ToString() + " - end");
                }
                trace.Trace("Processing " + accommodation.Length.ToString() + " Booking accommodation records - end");
            }
            trace.Trace("Accommodation populate records - end");
            return entityCollectionaccommodation;
        }

        private static Entity PrepareBookingAccommodation(string bookingNumber, Accommodation accommodation, Guid bookingId, ITracingService trace)
        {
            trace.Trace("Preparing Booking Accommodation information - Start");

            var accommodationEntity = new Entity(EntityName.BookingAccommodation);
            //BN - HotelId 
            if (accommodation.AccommodationCode != null)
                accommodationEntity[Attributes.BookingAccommodation.SourceMarketHotelCode] = accommodation.AccommodationCode;
            if (accommodation.GroupAccommodationCode != null && accommodation.GroupAccommodationCode != "")
            {
                accommodationEntity[Attributes.BookingAccommodation.Name] = bookingNumber + General.Concatenator + accommodation.GroupAccommodationCode;
                accommodationEntity[Attributes.BookingAccommodation.HotelId] = new EntityReference(EntityName.Hotel, Attributes.Hotel.MasterHotelID, accommodation.GroupAccommodationCode);
            }
            else
            {
                accommodationEntity[Attributes.BookingAccommodation.Name] = bookingNumber;

            }
            accommodationEntity[Attributes.BookingAccommodation.Order] = accommodation.Order.ToString();
            if(accommodation.StartDate != null)
            accommodationEntity[Attributes.BookingAccommodation.StartDateandTime] = DateTime.Parse(accommodation.StartDate);
            if(accommodation.EndDate != null)
            accommodationEntity[Attributes.BookingAccommodation.EndDateandTime] = DateTime.Parse(accommodation.EndDate);
            if(accommodation.RoomType != null)
            accommodationEntity[Attributes.BookingAccommodation.RoomType] = accommodation.RoomType;
            accommodationEntity[Attributes.BookingAccommodation.BoardType] = CommonXrm.GetOptionSetValue(accommodation.BoardType.ToString(), Attributes.BookingAccommodation.BoardType);
            accommodationEntity[Attributes.BookingAccommodation.HasSharedRoom] = accommodation.HasSharedRoom;
            accommodationEntity[Attributes.BookingAccommodation.NumberofParticipants] = accommodation.NumberOfParticipants;
            accommodationEntity[Attributes.BookingAccommodation.NumberofRooms] = accommodation.NumberOfRooms;
            accommodationEntity[Attributes.BookingAccommodation.WithTransfer] = accommodation.WithTransfer;
            accommodationEntity[Attributes.BookingAccommodation.IsExternalService] = accommodation.IsExternalService;
            if(accommodation.ExternalServiceCode != null)
            accommodationEntity[Attributes.BookingAccommodation.ExternalServiceCode] = CommonXrm.GetOptionSetValue(accommodation.ExternalServiceCode.ToString(), Attributes.BookingAccommodation.ExternalServiceCode);           
            accommodationEntity[Attributes.BookingAccommodation.NotificationRequired] = accommodation.NotificationRequired;
            accommodationEntity[Attributes.BookingAccommodation.NeedTourGuideAssignment] = accommodation.NeedsTourGuideAssignment;
            accommodationEntity[Attributes.BookingAccommodation.ExternalTransfer] = accommodation.IsExternalTransfer;
            if(accommodation.TransferServiceLevel != null)
            accommodationEntity[Attributes.BookingAccommodation.TransferServiceLevel] = CommonXrm.GetOptionSetValue(accommodation.TransferServiceLevel, Attributes.BookingAccommodation.TransferServiceLevel);
            if(accommodation.AccommodationDescription != null)
            accommodationEntity[Attributes.BookingAccommodation.SourceMarketHotelName] = accommodation.AccommodationDescription;
            accommodationEntity[Attributes.BookingAccommodation.BookingId] = new EntityReference(EntityName.Booking, bookingId);

            trace.Trace("Preparing Booking Transport information - End");

            return accommodationEntity;
        }

        public static EntityCollection DeActivateBookingAccommodation(Accommodation[] accommodation, List<XrmResponse> xrmResponseList, ITracingService trace)
        {
            EntityCollection bookingAccommodationEntityList = new EntityCollection();
            trace.Trace("Booking Accommodation populate Deactivation - start");
            if (accommodation != null && accommodation.Length > 0)
            {
                for (int i = 0; i < accommodation.Length; i++)
                {

                    if (accommodation[i].Status == AccommodationStatus.OK || accommodation[i].Status == AccommodationStatus.PR || accommodation[i].Status == AccommodationStatus.R)
                    {
                        trace.Trace("Booking " + i.ToString() + " Accommodation record Deactivation - start");
                        var bookingAccommodationEntity = new Entity(EntityName.BookingAccommodation, Guid.Parse(xrmResponseList[i].Id));
                        bookingAccommodationEntity[Attributes.Booking.StateCode] = new OptionSetValue((int)Statecode.InActive);
                        bookingAccommodationEntity[Attributes.Booking.StatusCode] = CommonXrm.GetOptionSetValue(accommodation[i].Status.ToString(), Attributes.Booking.StatusCode);
                        bookingAccommodationEntityList.Entities.Add(bookingAccommodationEntity);
                        trace.Trace("Booking " + i.ToString() + " Accommodation record Deactivation - end");
                    }
                    else
                    {
                        //throw new InvalidPluginExecutionException("Booking status provided in payload is invalid. It an be eiher B or C");
                    }
                }
            }
            trace.Trace("Booking Accommodation populate Deactivation - end");
            return bookingAccommodationEntityList;
        }

    }
}
