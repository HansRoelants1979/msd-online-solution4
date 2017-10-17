﻿using System;
using Tc.Crm.Common;
using System.Collections.ObjectModel;
using tcm = Tc.Crm.Service.Models;
using static Tc.Crm.Service.Constants.Crm.Actions;

namespace Tc.Crm.Service.Services
{
	public interface ICrmService
    {
        tcm.UpdateResponse ExecuteActionForBookingUpdate(string data);

        tcm.SurveyReturnResponse ExecuteActionForSurveyCreate(string data);

        tcm.CustomerResponse ExecuteActionOnCustomerEvent(string data, OperationType operation);
			
		Collection<tcm.Brand> GetBrands();
        Collection<tcm.Country> GetCountries();
        Collection<tcm.Currency> GetCurrencies();
        Collection<tcm.Gateway> GetGateways();
        Collection<tcm.TourOperator> GetTourOperators();
        Collection<tcm.SourceMarket> GetSourceMarkets();
        Collection<tcm.Hotel> GetHotels();
        bool PingCRM();
        Guid ProcessEntityCacheMessage(Guid entityCacheMessageId, string outComeId, Status status, EntityCacheMessageStatusReason statusReason, string notes = null);

		/// <summary>
		/// Process EntityCache record: set status and status of latest entitycache message operation
		/// </summary>
		/// <param name="entityCacheId"></param>
		/// <param name="status"></param>
		/// <param name="statusReason"></param>
		/// <param name="WasLastOperationSuccessful"></param>
		void ProcessEntityCache(Guid entityCacheId, Status status, EntityCacheStatusReason statusReason, bool WasLastOperationSuccessful = false, DateTime? eligibleRetryTime = null);

		/// <summary>
		/// Activate earliest EntityCache with same recordId and in Pending status
		/// </summary>
		/// <param name="entityCacheId">Guid of processed EntityCache record</param>
		void ActivateRelatedPendingEntityCache(Guid entityCacheId);

		string GetConfiguration(string name);

		int GetEntityCacheMessageCount(Guid entityCacheId);
    }
}
