﻿using System;
using System.Collections.Generic;
using Tc.Crm.WebJob.AllocateResortTeam.Models;
using Tc.Crm.Common;

namespace Tc.Crm.WebJob.AllocateResortTeam.Services
{
    public interface IAllocateResortTeamService:IDisposable
    {
        void Run();
        void GetBookingAllocations();                
        IList<BookingAllocationResortTeamRequest> ProcessAllocationResponse(IList<BookingAllocationResponse> bookingAllocationResponses);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        IList<Guid> GetDestinationGateways();
        void AddResortTeamRequest(BookingAllocationResponse bookingResponse, IList<BookingAllocationResortTeamRequest> bookingAllocationResortTeamRequest, ResortTeamRequestType resortTeamRequestType);
        string WriteAllocationResponseLog(BookingAllocationResponse bookingAllocationResponse);
        CustomerResortTeamRequest PrepareCustomerResortTeamRequest(BookingAllocationResponse bookingResponse);        
        BookingResortTeamRequest PrepareBookingResortTeamRequest(BookingAllocationResponse bookingResponse);
        bool ValidForProcessing(BookingAllocationResponse bookingResponse, List<Guid> processedCustomers, string responseLog);
        bool IsBookingAllocated(BookingAllocationResponse bookingResponse);
        bool IsCustomerAllocated(BookingAllocationResponse bookingResponse);
        string AllocateToChildHotelTeam(BookingAllocationResponse bookingResponse, IList<BookingAllocationResortTeamRequest> bookingAllocationResortTeamRequest, bool differentBooking, bool sameBookingDifferentCustomer, bool bookingAllocated, bool customerAllocated);
        BookingAllocationResortTeamRequest GetResortTeamRequest(BookingAllocationResponse bookingResponse);
        BookingAllocationResortTeamRequest GetBookingResortTeamRequest(BookingAllocationResponse bookingResponse);
        BookingAllocationResortTeamRequest GetCustomerResortTeamRequest(BookingAllocationResponse bookingResponse);

    }
}
