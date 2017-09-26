using System;
using Tc.Crm.Common;
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
        Guid ProcessEntityCacheMessage(Guid entityCacheMessageId, string outComeId, Status status, EntityCacheMessageStatusReason statusReason);
        Guid ProcessEntityCacheMessage(Guid entityCacheMessageId, string outComeId, Status status, EntityCacheMessageStatusReason statusReason, string notes);
        void ProcessEntityCache(Guid entityCacheId, Status status, EntityCacheStatusReason statusReason);
    }
}
