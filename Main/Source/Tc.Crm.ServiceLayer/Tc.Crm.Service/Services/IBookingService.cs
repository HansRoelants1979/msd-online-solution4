using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.Services
{
    public interface IBookingService
    {
        BookingUpdateResponse Update(string bookingData,ICrmService crmService);
        void ResolveReferences(Booking booking);
    }
}
