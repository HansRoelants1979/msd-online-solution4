using System.Collections.ObjectModel;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.Services
{
    public interface IBookingService
    {
        BookingUpdateResponse Update(string bookingData,ICrmService crmService);
        void ResolveReferences(Booking booking);
        Collection<string> Validate(BookingInformation bookingInformation);
        string GetStringFrom(Collection<string> strings);
    }
}
