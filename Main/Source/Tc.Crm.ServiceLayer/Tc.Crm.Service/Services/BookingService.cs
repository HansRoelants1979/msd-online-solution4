﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using Tc.Crm.Service.CacheBuckets;
using Tc.Crm.Service.Constants;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.Services
{
    public class BookingService : IBookingService
    {
        private BrandBucket brandBucket;
	    private CountryBucket countryBucket;
	    private CurrencyBucket currencyBucket;
	    private GatewayBucket gatewayBucket;
	    private SourceMarketBucket sourceMarketBucket;
	    private TourOperatorBucket tourOperatorBucket;
	    private HotelBucket hotelBucket;

        public BookingService(
			BrandBucket brandBucket, 
			CountryBucket countryBucket, 
			CurrencyBucket currencyBucket, 
			GatewayBucket gatewayBucket, 
			SourceMarketBucket sourceMarketBucket, 
			TourOperatorBucket tourOperatorBucket, 
			HotelBucket hotelBucket)
        {
            this.brandBucket = brandBucket;
            this.countryBucket = countryBucket;
            this.currencyBucket = currencyBucket;
            this.gatewayBucket = gatewayBucket;
            this.sourceMarketBucket = sourceMarketBucket;
            this.tourOperatorBucket = tourOperatorBucket;
            this.hotelBucket = hotelBucket;
        }

        public Collection<string> Validate(BookingInformation bookingInformation)
        {
            var validationMessages = new Collection<string>();
            if (bookingInformation == null || bookingInformation.Booking == null)
            {
                validationMessages.Add(Constants.Messages.BookingDataPassedIsNullOrCouldNotBeParsed);
                return validationMessages;
            }
            var booking = bookingInformation.Booking;
            if (booking.BookingIdentifier == null)
            {
                validationMessages.Add(Constants.Messages.BookingNumberNotPresent);
                validationMessages.Add(Constants.Messages.SourceKeyNotPresent);
                validationMessages.Add(Constants.Messages.BookingSystemIsUnknown);
                validationMessages.Add(Constants.Messages.SourceMarketMissing);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(booking.BookingIdentifier.BookingNumber))
                    validationMessages.Add(Constants.Messages.BookingNumberNotPresent);
                if (booking.BookingIdentifier.BookingSystem == BookingSystem.Unknown)
                    validationMessages.Add(Constants.Messages.BookingSystemIsUnknown);
                if (string.IsNullOrWhiteSpace(booking.BookingIdentifier.SourceMarket))
                    validationMessages.Add(Constants.Messages.SourceMarketMissing);
                if (booking.BookingIdentifier.SourceSystemId == SourceSystemId.Unknown)
                    validationMessages.Add(Constants.Messages.BookingDatabaseNotPresent);
                if (booking.BookingIdentifier.SourceSystemId == SourceSystemId.TCV)
                {
                    if (string.IsNullOrWhiteSpace(booking.BookingIdentifier.ConsultationReference))
                        validationMessages.Add(Constants.Messages.ConsultationReferenceNotPresent);
                }
            }

            if (booking.Customer != null && (booking.Customer.CustomerIdentifier != null))
            {
                if (string.IsNullOrWhiteSpace(booking.Customer.CustomerIdentifier.CustomerId))
                    validationMessages.Add(Constants.Messages.CustomerIdIsNull);
            }
            if ((booking.Customer != null && booking.Customer.CustomerIdentifier != null))
            {
                if (string.IsNullOrWhiteSpace(booking.Customer.CustomerIdentifier.SourceMarket))
                    validationMessages.Add(Constants.Messages.CustomerSourceMarketMissing);
            }
            
            return validationMessages;
        }

        public string GetStringFrom(Collection<string> strings)
        {
            if (strings == null || strings.Count == 0) return null;
            StringBuilder message = new StringBuilder();
            for (int i = 0; i < strings.Count; i++)
            {
                if (i == strings.Count - 1)
                    message.Append(strings[i]);
                else
                    message.AppendLine(strings[i]);
            }
            foreach (var item in strings)
            {

            }
            return message.ToString();
        }

        public BookingUpdateResponse Update(string bookingData, ICrmService crmService)
        {
            if (string.IsNullOrWhiteSpace(bookingData)) throw new ArgumentNullException(Constants.Parameters.BookingData);
            if (crmService == null) throw new ArgumentNullException(Constants.Parameters.CrmService);

            var response = crmService.ExecuteActionForBookingUpdate(bookingData);
            if (response == null) throw new InvalidOperationException(Constants.Messages.ResponseNull);
            return new BookingUpdateResponse { Created = response.Created, Id = response.Id,ResponseCode= response.ResponseCode,ResponseMessage=response.ResponseMessage };
        }

        public void ResolveReferences(Booking booking){
            ResolveCustomerReferences(booking.Customer);
            ResolveCurrency(booking);
            ResolveGeneralReferences(booking.BookingGeneral);
            if (booking.BookingIdentifier != null)
            {
                var sourceMarket = sourceMarketBucket.GetBy(booking.BookingIdentifier.SourceMarket);
                if (sourceMarket != null)
                {
                    booking.BookingIdentifier.SourceMarket = sourceMarket.Id;
                    if (!string.IsNullOrWhiteSpace(sourceMarket.TeamId))
                        booking.Owner = sourceMarket.TeamId;
                }
                else
                {
                    booking.BookingIdentifier.SourceMarket = null;
                }
            }

            if (booking.Services != null)
            {
                ResolveTransportReferences(booking.Services.Transport);
                ResolveTransferReferences(booking.Services.Transfer);
                ResolveAccommodationReferences(booking);
            }
        }
        public void ResolveAccommodationReferences(Booking booking)
        {
            if (booking.Services.Accommodation == null || booking.Services.Accommodation.Length == 0)
                return;
            var dest = hotelBucket.GetBy(booking.Services.Accommodation[0].GroupAccommodationCode);

            if (dest == null || string.IsNullOrWhiteSpace(dest.DestinationId))
            {
                booking.DestinationId = null;
            }
            else
            {
                booking.DestinationId = dest.DestinationId;
            }

            foreach (var acc in booking.Services.Accommodation)
            {
                var hotel = hotelBucket.GetBy(acc.GroupAccommodationCode);
                if (hotel == null || string.IsNullOrWhiteSpace(hotel.Id))
                    acc.GroupAccommodationCode = null;
                else
                    acc.GroupAccommodationCode = hotel.Id;
            }
        }
        public void ResolveTransportReferences(Transport[] transports)
        {
            if (transports == null) return;
            foreach (var transport in transports){ 
                var type = gatewayBucket.MapTransportType(transport.TransportType);
                transport.ArrivalAirport = gatewayBucket.GetBy($"{transport.ArrivalAirport}_{type}").Id;
                transport.DepartureAirport = gatewayBucket.GetBy($"{transport.DepartureAirport}_{type}").Id;
            }
        }
        public void ResolveTransferReferences(Transfer[] transfers)
        {
            if (transfers == null) return;
            foreach (var transfer in transfers){
                transfer.ArrivalAirport = gatewayBucket.GetBy($"{transfer.ArrivalAirport}_{GatewayType.Airport}").Id;
                transfer.DepartureAirport = gatewayBucket.GetBy($"{transfer.DepartureAirport}_{GatewayType.Airport}").Id;
            }
        }

        public void ResolveCustomerReferences(Customer customer)
        {
            if (customer == null) return;

            if (customer.Address != null)
            {
                foreach (var address in customer.Address)
                {
                    address.Country = countryBucket.GetBy(address.Country).Id;
                }
            }

            if (customer.CustomerIdentifier != null)
            {
                var sourceMarket = sourceMarketBucket.GetBy(customer.CustomerIdentifier.SourceMarket);
                if (sourceMarket != null)
                {
                    customer.CustomerIdentifier.SourceMarket = sourceMarket.Id;
                    if (!string.IsNullOrWhiteSpace(sourceMarket.TeamId))
                        customer.Owner = sourceMarket.TeamId;
                }
                else
                    customer.CustomerIdentifier.SourceMarket = null;
            }


        }
        public void ResolveGeneralReferences(BookingGeneral general)
        {
            if (general == null) return;
            general.Brand = brandBucket.GetBy(general.Brand).Id;
            general.Destination = gatewayBucket.GetBy($"{general.Destination}_{GatewayType.Airport}").Id;
            general.ToCode = tourOperatorBucket.GetBy(general.ToCode).Id;
        }
        public void ResolveCurrency(Booking booking)
        {
            if (booking == null) return;
            if (booking.BookingGeneral == null) return;

            if (string.IsNullOrWhiteSpace(booking.BookingGeneral.Currency))
            {
                Trace.TraceWarning("Currency is empty, deriving currency from source market.");
                var sourceMarket = booking.BookingIdentifier != null ? booking.BookingIdentifier.SourceMarket : null;
                if (string.IsNullOrWhiteSpace(sourceMarket))
                {
                    Trace.TraceWarning("Source market is empty");
                    return;
                }
                switch (sourceMarket)
                {
                    case SourceMarketIsoCode.UK: booking.BookingGeneral.Currency = currencyBucket.GetBy(CurrencyCode.Pound).Id; break;
                    case SourceMarketIsoCode.France:
                    case SourceMarketIsoCode.Belgium:
                    case SourceMarketIsoCode.Germany:
                    case SourceMarketIsoCode.Netherlands: booking.BookingGeneral.Currency = currencyBucket.GetBy(CurrencyCode.Euro).Id; break;
                    case SourceMarketIsoCode.CzechRepublic: booking.BookingGeneral.Currency = currencyBucket.GetBy(CurrencyCode.CzechKoruna).Id; break;
                    case SourceMarketIsoCode.Hungary: booking.BookingGeneral.Currency = currencyBucket.GetBy(CurrencyCode.HungarianForint).Id; break;
                    case SourceMarketIsoCode.Poland: booking.BookingGeneral.Currency = currencyBucket.GetBy(CurrencyCode.PolishZłoty).Id; break;
                    default:
                        break;
                }
            }
            else
            {
                booking.BookingGeneral.Currency = currencyBucket.GetBy(booking.BookingGeneral.Currency).Id;
            }
        }
    }
}