using System;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;
using Microsoft.Xrm.Sdk;

namespace Tc.Crm.CustomWorkflowSteps.ProcessBooking.Services
{
    public static class BookingExtraServiceHelper
    {
        public static EntityCollection GetBookingExtraServicerEntityFromPayload(Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models.Booking bookingInfo, Guid bookingId, ITracingService trace)
        {
            if (bookingInfo == null) return null;
            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("ExtraService populate records - start");
            string bookingNumber = bookingInfo.BookingIdentifier.BookingNumber;
            var extraService = bookingInfo.Services.ExtraService;
            if (bookingNumber == null || string.IsNullOrWhiteSpace(bookingNumber))
                throw new InvalidPluginExecutionException("Booking Number should not be null.");
            EntityCollection entityCollectionextraService = new EntityCollection();
            if (extraService != null && extraService.Length > 0)
            {
                trace.Trace("Processing " + extraService.Length.ToString() + " Booking transfer records - start");
                Entity extraServiceEntity = null;
                for (int i = 0; i < extraService.Length; i++)
                {
                    trace.Trace("Processing Booking Transfer " + i.ToString() + " - start");
                    extraServiceEntity = PrepareBookingExtraService(bookingInfo, extraService[i], bookingId, trace);
                    entityCollectionextraService.Entities.Add(extraServiceEntity);
                    trace.Trace("Processing Booking accommodation " + i.ToString() + " - end");
                }
                trace.Trace("Processing " + extraService.Length.ToString() + " Booking Transfer records - end");
            }
            trace.Trace("Transfer populate records - end");
            return entityCollectionextraService;
        }

        private static Entity PrepareBookingExtraService(Booking bookinginfo, ExtraService extraService, Guid bookingId, ITracingService trace)
        {
            trace.Trace("Preparing Booking Transfer information - Start");
            var extraServiceEntity = new Entity(EntityName.BookingExtraService);

            if (!string.IsNullOrWhiteSpace(extraService.ExtraServiceCode ))
                extraServiceEntity[Attributes.BookingExtraService.ExtraServiceCode] = extraService.ExtraServiceCode;

            SetNameFor(extraService, bookinginfo, extraServiceEntity);

            extraServiceEntity[Attributes.BookingExtraService.Order] = extraService.Order;
            if (!string.IsNullOrWhiteSpace(extraService.StartDate ))
                extraServiceEntity[Attributes.BookingExtraService.StartDateAndTime] = DateTime.Parse(extraService.StartDate);
            if (!string.IsNullOrWhiteSpace(extraService.EndDate ))
                extraServiceEntity[Attributes.BookingExtraService.EndDateTime] = DateTime.Parse(extraService.EndDate);


            extraServiceEntity[Attributes.BookingExtraService.Participants] = BookingHelper.PrepareTravelParticipantsInfoForChildRecords(bookinginfo.TravelParticipant, trace, extraService.TravelParticipantAssignment);

            extraServiceEntity[Attributes.BookingExtraService.BookingId] = new EntityReference(EntityName.Booking, bookingId);
            extraServiceEntity[Attributes.Booking.Remarks] = RemarksHelper.GetRemarksTextFromPayload(extraService.Remark);

            trace.Trace("Preparing Booking Transfer information - End");

            return extraServiceEntity;
        }

        private static void SetNameFor(ExtraService extraService, Booking bookinginfo, Entity extraServiceEntity)
        {
            var bookingNumber = bookinginfo.BookingIdentifier.BookingNumber;
            extraServiceEntity[Attributes.BookingExtraService.Name] = $"{extraService.ExtraServiceDescription} - {bookingNumber}";
        }
    }
}
