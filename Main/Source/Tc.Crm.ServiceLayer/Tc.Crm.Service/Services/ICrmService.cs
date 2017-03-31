using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tcm = Tc.Crm.Service.Models;

namespace Tc.Crm.Service.Services
{
    public interface ICrmService
    {
        tcm.UpdateResponse ExecuteActionForBookingUpdate(string data);

        tcm.SurveyReturnResponse ExecuteActionForSurveyCreate(string data);

        Collection<tcm.Brand>    GetBrands();
        Collection<tcm.Country> GetCountries();
        Collection<tcm.Currency> GetCurrencies();
        Collection<tcm.Gateway> GetGateways();
        Collection<tcm.TourOperator> GetTourOperators();
        Collection<tcm.SourceMarket> GetSourceMarkets();

    }
}
