using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;
using Microsoft.Xrm.Sdk;

namespace Tc.Crm.CustomWorkflowSteps.ProcessBooking.Services
{
    class BookingExtraServiceHelper
    {
        public static EntityCollection GetBookingExtraServicerEntityFromPayload(Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models.Booking bookinginfo, Guid bookingId, ITracingService trace)
        {

            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("ExtraService populate records - start");
            string bookingNumber = bookinginfo.BookingIdentifier.BookingNumber;
            var extraService = bookinginfo.Services.ExtraService;
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
                    extraServiceEntity = PrepareBookingExtraService(bookinginfo, extraService[i], bookingId, trace);
                    entityCollectionextraService.Entities.Add(extraServiceEntity);
                    trace.Trace("Processing Booking accommodation " + i.ToString() + " - end");
                }
                trace.Trace("Processing " + extraService.Length.ToString() + " Booking Transfer records - end");
            }
            trace.Trace("Transfer populate records - end");
            return entityCollectionextraService;
        }

        private static Entity PrepareBookingExtraService(Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models.Booking bookinginfo, ExtraService extraService, Guid bookingId, ITracingService trace)
        {
            trace.Trace("Preparing Booking Transfer information - Start");
            string bookingNumber = bookinginfo.BookingIdentifier.BookingNumber;
            var extraServiceEntity = new Entity(EntityName.BookingExtraService);

            if (!string.IsNullOrWhiteSpace(extraService.ExtraServiceCode ))
                extraServiceEntity[Attributes.BookingExtraService.ExtraServiceCode] = extraService.ExtraServiceCode;
            if (!string.IsNullOrWhiteSpace(extraService.ExtraServiceDescription ))
            {
                extraServiceEntity[Attributes.BookingExtraService.Name] = extraService.ExtraServiceDescription;

            }
            else
            {
                extraServiceEntity[Attributes.BookingExtraService.Name] = bookingNumber;

            }
            extraServiceEntity[Attributes.BookingExtraService.Order] = extraService.Order;
            if (!string.IsNullOrWhiteSpace(extraService.StartDate ))
                extraServiceEntity[Attributes.BookingExtraService.StartDateandTime] = DateTime.Parse(extraService.StartDate);
            if (!string.IsNullOrWhiteSpace(extraService.EndDate ))
                extraServiceEntity[Attributes.BookingExtraService.EndDateTime] = DateTime.Parse(extraService.EndDate);


            extraServiceEntity[Attributes.BookingExtraService.Participants] = BookingHelper.PrepareTravelParticipantsInfoForChildRecords(bookinginfo.TravelParticipant, trace, extraService.TravelParticipantAssignment);

            extraServiceEntity[Attributes.BookingExtraService.BookingId] = new EntityReference(EntityName.Booking, bookingId);

            trace.Trace("Preparing Booking Transfer information - End");

            return extraServiceEntity;
        }


    }
}
