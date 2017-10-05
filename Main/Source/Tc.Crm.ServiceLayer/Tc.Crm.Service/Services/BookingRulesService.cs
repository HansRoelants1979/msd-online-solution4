using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.Services
{
    public class BookingRulesService
    {
        public bool IsOnTourorTCVAtCoreBooking(BookingIdentifier bookingIdentifier)
        {
            if(bookingIdentifier.SourceSystemId == SourceSystemId.OnTour ||
                (bookingIdentifier.SourceSystemId == SourceSystemId.TCV && bookingIdentifier.BookingSystem == BookingSystem.AtCore))
            {
                return true;
            }
            return false;
        }

        public bool IsOnTourBooking(BookingIdentifier bookingIdentifier)
        {
            if (bookingIdentifier.SourceSystemId == SourceSystemId.OnTour) return true;
            return false;
        }

        public bool IsBookingConsultationEmpty(BookingIdentifier bookingIdentifier)
        {
            if (string.IsNullOrEmpty(bookingIdentifier.ConsultationReference)) return true;
            return false;
        }

        public Entity MatchingBooking(BookingIdentifier bookingIdentifier)
        {

            return new Entity();
        }

        public bool IsSameCustomer(Booking booking,Entity msdBooking)
        {
            return false;
        }

        public Entity MatchingCustomer(Booking booking)
        {
            return new Entity();
        }


    }
}