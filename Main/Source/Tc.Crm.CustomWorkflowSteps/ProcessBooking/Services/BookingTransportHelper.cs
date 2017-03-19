using Microsoft.Xrm.Sdk;
using System;
using System.Text;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;

namespace Tc.Crm.CustomWorkflowSteps.ProcessBooking.Services
{
    public static class BookingTransportHelper
    {
        public static EntityCollection GetTransportEntityForBookingPayload(Booking bookinginfo, Guid bookingId, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing Service is null.");
            trace.Trace("Transport populate records - start");
            string bookingNumber = bookinginfo.BookingIdentifier.BookingNumber;
            var transport = bookinginfo.Services.Transport;
            if (bookingNumber == null || string.IsNullOrWhiteSpace(bookingNumber))
                throw new InvalidPluginExecutionException("Booking Number should not be null.");
            EntityCollection entityCollectionTransport = new EntityCollection();
            if (transport != null && transport.Length > 0)
            {
                Entity transportEntity = null;
                trace.Trace("Processing " + transport.Length.ToString() + " transport records - start");
                for (int i = 0; i < transport.Length; i++)
                {
                    trace.Trace("Processing transport record " + i.ToString() + " - start");
                    transportEntity = PrepareBookingTransport(bookinginfo, transport[i], bookingId, trace);
                    entityCollectionTransport.Entities.Add(transportEntity);
                    trace.Trace("Processing transport record " + i.ToString() + " - end");
                }
                trace.Trace("Processing " + transport.Length.ToString() + " transport records - end");
            }
            trace.Trace("Transport populate records - end");
            return entityCollectionTransport;
        }

        private static Entity PrepareBookingTransport(Booking bookinginfo, Transport transport, Guid bookingId, ITracingService trace)
        {
            trace.Trace("Transport populate fields - start");
            string bookingNumber = bookinginfo.BookingIdentifier.BookingNumber;
            var transportEntity = new Entity(EntityName.BookingTransport);
            transportEntity[Attributes.BookingTransport.Name] = bookingNumber + General.Concatenator + transport.DepartureAirport + General.Concatenator + transport.ArrivalAirport;
            if (!string.IsNullOrWhiteSpace(transport.TransportCode))
                transportEntity[Attributes.BookingTransport.TransportCode] = transport.TransportCode;
            if (!string.IsNullOrWhiteSpace(transport.TransportDescription))
                transportEntity[Attributes.BookingTransport.Description] = transport.TransportDescription;
            transportEntity[Attributes.BookingTransport.Order] = transport.Order;
            if (!string.IsNullOrWhiteSpace(transport.StartDate))
                transportEntity[Attributes.BookingTransport.StartDateandTime] = DateTime.Parse(transport.StartDate);
            if (!string.IsNullOrWhiteSpace(transport.EndDate))
                transportEntity[Attributes.BookingTransport.EndDateandTime] = DateTime.Parse(transport.EndDate);
            transportEntity[Attributes.BookingTransport.TransferType] = CommonXrm.GetTransferType(transport.TransferType);
            if (!string.IsNullOrWhiteSpace(transport.DepartureAirport))
                transportEntity[Attributes.BookingTransport.DepartureGatewayId] = new EntityReference(EntityName.Gateway, Attributes.Gateway.IATA, transport.DepartureAirport);
            if (!string.IsNullOrWhiteSpace(transport.ArrivalAirport))
                transportEntity[Attributes.BookingTransport.ArrivalGatewayId] = new EntityReference(EntityName.Gateway, Attributes.Gateway.IATA, transport.ArrivalAirport);
            if (!string.IsNullOrWhiteSpace(transport.CarrierCode))
                transportEntity[Attributes.BookingTransport.CarrierCode] = transport.CarrierCode;
            if (!string.IsNullOrWhiteSpace(transport.FlightNumber))
                transportEntity[Attributes.BookingTransport.FlightNumber] = transport.FlightNumber;
            if (!string.IsNullOrWhiteSpace(transport.FlightIdentifier))
                transportEntity[Attributes.BookingTransport.FlightIdentifier] = transport.FlightIdentifier;
            transportEntity[Attributes.BookingTransport.NumberofParticipants] = transport.NumberOfParticipants;
            transportEntity[Attributes.BookingTransport.BookingId] = new EntityReference(EntityName.Booking, bookingId);
            transportEntity[Attributes.BookingTransport.Participants] = BookingHelper.PrepareTravelParticipantsInfoForChildRecords(bookinginfo.TravelParticipant, trace, transport.TravelParticipantAssignment);
            trace.Trace("Transport populate fields - end");
            return transportEntity;
        }
    }
}
