using System;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.Services
{
    public class BookingService : IBookingService
    {
        public BookingUpdateResponse Update(string bookingData, ICrmService crmService)
        {
            if (string.IsNullOrWhiteSpace(bookingData)) throw new ArgumentNullException(Constants.Parameters.BookingData);
            if (crmService == null) throw new ArgumentNullException(Constants.Parameters.CrmService);

            var response = crmService.ExecuteActionForBookingUpdate(bookingData);
            if (response == null) throw new InvalidOperationException(Constants.Messages.ResponseNull);
            if (!response.Success)
                throw new InvalidOperationException(response.ErrorMessage);
            if (response.Created) return new BookingUpdateResponse { Created = true, Id = response.Id.ToString() };
            return new BookingUpdateResponse { Created = false, Id = response.Id.ToString() };
        }

       
    }
}