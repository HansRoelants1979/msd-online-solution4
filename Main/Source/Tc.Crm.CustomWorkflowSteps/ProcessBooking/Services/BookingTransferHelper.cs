using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;
using Microsoft.Xrm.Sdk;

namespace Tc.Crm.CustomWorkflowSteps.ProcessBooking.Services
{
    class BookingTransferHelper
    {
        public static EntityCollection GetBookingTransferEntityFromPayload(Booking bookinginfo, Guid bookingId, ITracingService trace)
        {

            if (trace == null) throw new InvalidPluginExecutionException("Tracing service is null;");
            trace.Trace("Transfer populate records - start");
            string bookingNumber = bookinginfo.BookingIdentifier.BookingNumber;
            var transfer = bookinginfo.Services.Transfer;
            if (bookingNumber == null || string.IsNullOrWhiteSpace(bookingNumber))
                throw new InvalidPluginExecutionException("Booking Number should not be null.");
            EntityCollection entityCollectiontransfer = new EntityCollection();
            if (transfer != null && transfer.Length > 0)
            {
                trace.Trace("Processing " + transfer.Length.ToString() + " Booking transfer records - start");
                Entity transferEntity = null;
                for (int i = 0; i < transfer.Length; i++)
                {
                    trace.Trace("Processing Booking Transfer " + i.ToString() + " - start");
                    transferEntity = PrepareBookingTransfer(bookinginfo, transfer[i], bookingId, trace);
                    entityCollectiontransfer.Entities.Add(transferEntity);
                    trace.Trace("Processing Booking accommodation " + i.ToString() + " - end");
                }
                trace.Trace("Processing " + transfer.Length.ToString() + " Booking Transfer records - end");
            }
            trace.Trace("Transfer populate records - end");
            return entityCollectiontransfer;
        }

        private static Entity PrepareBookingTransfer(Booking bookinginfo, Transfer transfer, Guid bookingId, ITracingService trace)
        {
            trace.Trace("Preparing Booking Transfer information - Start");
            var transferEntity = new Entity(EntityName.BookingTransfer);

            if (!string.IsNullOrWhiteSpace(transfer.TransferCode))
                transferEntity[Attributes.BookingTransfer.TransferCode] = transfer.TransferCode;

            SetNameFor(transfer, bookinginfo, transferEntity);

            transferEntity[Attributes.BookingTransfer.Order] = transfer.Order;
            if (!string.IsNullOrWhiteSpace(transfer.StartDate))
                transferEntity[Attributes.BookingTransfer.StartDateandTime] = DateTime.Parse(transfer.StartDate);
            if (!string.IsNullOrWhiteSpace(transfer.EndDate))
                transferEntity[Attributes.BookingTransfer.EndDateTime] = DateTime.Parse(transfer.EndDate);

            transferEntity[Attributes.BookingTransfer.TransferType] = CommonXrm.GetTransferType(transfer.TransferType);
            transferEntity[Attributes.BookingTransfer.Category] = transfer.Category;
            if (!string.IsNullOrWhiteSpace(transfer.DepartureAirport))
                transferEntity[Attributes.BookingTransfer.DepartureGatewayId] = new EntityReference(EntityName.Gateway, new Guid(transfer.DepartureAirport));
            if (!string.IsNullOrWhiteSpace(transfer.ArrivalAirport))
                transferEntity[Attributes.BookingTransfer.ArrivalGatewayId] = new EntityReference(EntityName.Gateway, new Guid(transfer.ArrivalAirport));

            transferEntity[Attributes.BookingTransfer.Participants] = BookingHelper.PrepareTravelParticipantsInfoForChildRecords(bookinginfo.TravelParticipant, trace, transfer.TravelParticipantAssignment);

            transferEntity[Attributes.BookingTransfer.BookingId] = new EntityReference(EntityName.Booking, bookingId);
            transferEntity[Attributes.Booking.Remarks] = RemarksHelper.GetRemarksTextFromPayload(transfer.Remark);

            trace.Trace("Preparing Booking Transfer information - End");

            return transferEntity;
        }

        private static void SetNameFor(Transfer transfer, Booking bookinginfo, Entity transferEntity)
        {
            var bookingNumber = bookinginfo.BookingIdentifier.BookingNumber;
            transferEntity[Attributes.BookingTransfer.Name] = $"{transfer.TransferDescription} - {bookingNumber}";
        }
    }
}
