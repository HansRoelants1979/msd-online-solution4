using System;
using Tc.Crm.Service.CacheBuckets;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.Services
{
    public class BookingService : IBookingService
    {
        BrandBucket brandBucket;
        CountryBucket countryBucket;
        CurrencyBucket currencyBucket;
        GatewayBucket gatewayBucket;
        SourceMarketBucket sourceMarketBucket;
        TourOperatorBucket tourOperatorBucket;
        HotelBucket hotelBucket;

        public BookingService(BrandBucket brandBucket
                                , CountryBucket countryBucket
                                , CurrencyBucket currencyBucket
                                , GatewayBucket gatewayBucket
                                , SourceMarketBucket sourceMarketBucket
                                , TourOperatorBucket tourOperatorBucket
                                , HotelBucket hotelBucket)
        {
            this.brandBucket = brandBucket;
            this.countryBucket = countryBucket;
            this.currencyBucket = currencyBucket;
            this.gatewayBucket = gatewayBucket;
            this.sourceMarketBucket = sourceMarketBucket;
            this.tourOperatorBucket = tourOperatorBucket;
            this.hotelBucket = hotelBucket;
        }
        public BookingUpdateResponse Update(string bookingData, ICrmService crmService)
        {
            if (string.IsNullOrWhiteSpace(bookingData)) throw new ArgumentNullException(Constants.Parameters.BookingData);
            if (crmService == null) throw new ArgumentNullException(Constants.Parameters.CrmService);

            var response = crmService.ExecuteActionForBookingUpdate(bookingData);
            if (response == null) throw new InvalidOperationException(Constants.Messages.ResponseNull);
            return new BookingUpdateResponse { Created = response.Created, Id = response.Id.ToString() };
        }

        public void ResolveReferences(Booking booking)
        {
            ResolveCustomerReferences(booking.Customer);
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
            }

            if(booking.Services!=null)
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
            foreach (var transport in transports)
            {
                transport.ArrivalAirport = gatewayBucket.GetBy(transport.ArrivalAirport);
                transport.DepartureAirport = gatewayBucket.GetBy(transport.DepartureAirport);
            }
        }
        public void ResolveTransferReferences(Transfer[] transfers)
        {
            if (transfers == null) return;
            foreach (var transfer in transfers)
            {
                transfer.ArrivalAirport = gatewayBucket.GetBy(transfer.ArrivalAirport);
                transfer.DepartureAirport = gatewayBucket.GetBy(transfer.DepartureAirport);
            }
        }

        public void ResolveCustomerReferences(Customer customer)
        {
            if (customer == null) return;

            if (customer.Address != null)
            {
                foreach (var address in customer.Address)
                {
                    address.Country = countryBucket.GetBy(address.Country);
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
            }
           

        }
        public void ResolveGeneralReferences(BookingGeneral general)
        {
            if (general == null) return;
            general.Brand = brandBucket.GetBy(general.Brand);
            general.Currency = currencyBucket.GetBy(general.Currency);
            general.Destination = gatewayBucket.GetBy(general.Destination);
            general.ToCode = tourOperatorBucket.GetBy(general.ToCode);
        }
    }
}