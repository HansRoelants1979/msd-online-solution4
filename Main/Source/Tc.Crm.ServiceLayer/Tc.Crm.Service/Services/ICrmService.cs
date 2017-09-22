using System.Collections.ObjectModel;
using tcm = Tc.Crm.Service.Models;
using System.Collections.Generic;
using static Tc.Crm.Service.Constants.Crm.Actions;

namespace Tc.Crm.Service.Services
{
    public interface ICrmService
    {
        tcm.UpdateResponse ExecuteActionForBookingUpdate(string data);

        tcm.SurveyReturnResponse ExecuteActionForSurveyCreate(string data);

        tcm.CustomerResponse ExecuteActionOnCustomerEvent(string data, OperationType operation);

        Collection<tcm.Brand>    GetBrands();
        Collection<tcm.Country> GetCountries();
        Collection<tcm.Currency> GetCurrencies();
        Collection<tcm.Gateway> GetGateways();
        Collection<tcm.TourOperator> GetTourOperators();
        Collection<tcm.SourceMarket> GetSourceMarkets();
        Collection<tcm.Hotel> GetHotels();
        bool PingCRM();
    }
}
