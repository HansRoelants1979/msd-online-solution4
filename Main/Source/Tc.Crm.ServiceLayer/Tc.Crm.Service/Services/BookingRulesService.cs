using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tc.Crm.Common.Models;

namespace Tc.Crm.Service.Services
{
    public class BookingRulesService
    {
        public bool IsOnTourorTCVAtCoreBooking(Booking booking)
        {
            return false;
        }

        public bool IsOnTourBooking(Booking booking)
        {
            return false;
        }

        public bool IsBookingConsultationEmpty(Booking booking)
        {
            return false;
        }

        public Entity MatchingBooking(Booking booking)
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