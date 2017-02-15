using Microsoft.Xrm.Sdk;
using System;
using System.Text;
using Tc.Crm.CustomWorkflowSteps.ProcessBooking.Models;

namespace Tc.Crm.CustomWorkflowSteps.ProcessBooking.Services
{
    public static class BookingTransportHelper
    {
        public static EntityCollection GetTransportEntityForBookingPayload(Booking booking, Guid bookingId, ITracingService trace)
        {
            if (trace == null) throw new InvalidPluginExecutionException("Tracing Service is null.");
            trace.Trace("Transport populate records - start");
            if (booking.Services == null) throw new InvalidPluginExecutionException("Booking services payload is null.");
            if (booking.Services.Transport == null)
                throw new InvalidPluginExecutionException("Transport could not be retrieved from payload.");
            if (booking.BookingIdentifier.BookingNumber == null || string.IsNullOrWhiteSpace(booking.BookingIdentifier.BookingNumber))                
                throw new InvalidPluginExecutionException("Booking Number should not be null.");
            EntityCollection entityCollectionTransport = new EntityCollection();
            if (booking.Services.Transport != null)
            {
                Transport[] transport = booking.Services.Transport;
                Entity transportEntity = null;
                trace.Trace("Processing " + transport.Length.ToString() + " transport records - start");
                for (int i = 0; i < transport.Length; i++)
                {
                    trace.Trace("Processing transport record " + i.ToString() + " - start");
                    transportEntity = PrepareBookingTransport(booking, transport[i], bookingId, trace);
                    entityCollectionTransport.Entities.Add(transportEntity);
                    trace.Trace("Processing transport record " + i.ToString() + " - end");
                }
                trace.Trace("Processing " + transport.Length.ToString() + " transport records - end");               
            }
            trace.Trace("Transport populate records - end");
            return entityCollectionTransport;
        }

        private static Entity PrepareBookingTransport(Booking booking, Transport transport, Guid bookingId, ITracingService trace)
        {
            trace.Trace("Transport populate fields - start");
            var transportEntity = new Entity(EntityName.BookingTransport);
            transportEntity[Attributes.BookingTransport.Name] = booking.BookingIdentifier.BookingNumber + General.Concatenator + transport.DepartureAirport + General.Concatenator + transport.ArrivalAirport;
            if (transport.TransportCode != null)
                transportEntity[Attributes.BookingTransport.TransportCode] = transport.TransportCode;
            if (transport.TransportDescription != null)
                transportEntity[Attributes.BookingTransport.Description] = transport.TransportDescription;
            transportEntity[Attributes.BookingTransport.Order] = transport.Order;
            if (transport.StartDate != null)
                transportEntity[Attributes.BookingTransport.StartDateandTime] = DateTime.Parse(transport.StartDate);
            if (transport.EndDate != null)
                transportEntity[Attributes.BookingTransport.EndDateandTime] = DateTime.Parse(transport.EndDate);
            transportEntity[Attributes.BookingTransport.TransferType] = CommonXrm.GetOptionSetValue(transport.TransferType.ToString(), Attributes.BookingTransport.TransferType);
            if (transport.DepartureAirport != null)
                transportEntity[Attributes.BookingTransport.DepartureGatewayId] = new EntityReference(EntityName.Gateway, Attributes.Gateway.IATA, transport.DepartureAirport);
            if (transport.ArrivalAirport != null)
                transportEntity[Attributes.BookingTransport.ArrivalGatewayId] = new EntityReference(EntityName.Gateway, Attributes.Gateway.IATA, transport.ArrivalAirport);
            if (transport.CarrierCode != null)
                transportEntity[Attributes.BookingTransport.CarrierCode] = transport.CarrierCode;
            if (transport.FlightNumber != null)
                transportEntity[Attributes.BookingTransport.FlightNumber] = transport.FlightNumber;
            if (transport.FlightIdentifier != null)
                transportEntity[Attributes.BookingTransport.FlightIdentifier] = transport.FlightIdentifier;
            transportEntity[Attributes.BookingTransport.NumberofParticipants] = transport.NumberOfParticipants;
            transportEntity[Attributes.BookingTransport.BookingId] = new EntityReference(EntityName.Booking, bookingId);
            trace.Trace("Transport populate fields - end");
            return transportEntity;
        }
    }
}
