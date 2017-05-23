using System;
using System.Collections.Generic;
using Tc.Crm.Service.Services;
using FakeXrmEasy;
using Tc.Crm.Service.Models;
using System.Collections.ObjectModel;
using System.Net;

namespace Tc.Crm.ServiceTests
{
    public enum DataSwitch
    {
        Created,
        Updated,
        Response_NULL,
        Response_Failed,
        Return_NULL,
        ActionThrowsError
    }
    public class TestCrmService : ICrmService
    {
        XrmFakedContext context;

        public DataSwitch Switch { get; set; }

        public TestCrmService(XrmFakedContext context)
        {
            this.context = context;
        }
        public Tc.Crm.Service.Models.UpdateResponse ExecuteActionForBookingUpdate(string data)
        {
            object Constants = null;
            if (Switch == DataSwitch.Created)
                return new Tc.Crm.Service.Models.UpdateResponse { Created = true, Id = Guid.NewGuid().ToString() };

            else if (Switch == DataSwitch.Updated)
                return new Tc.Crm.Service.Models.UpdateResponse { Created = false, Id = Guid.NewGuid().ToString() };

            else if (Switch == DataSwitch.Response_NULL)
                throw new InvalidOperationException(Tc.Crm.Service.Constants.Messages.ResponseFromCrmIsNull);

            else if (Switch == DataSwitch.Response_Failed)
                return new Tc.Crm.Service.Models.UpdateResponse { Created = false, Id = null};
            else if (Switch == DataSwitch.Return_NULL)
                return null;
            else if (Switch == DataSwitch.ActionThrowsError)
                throw new Exception("Action faulted");
            return null;
        }

        public SurveyReturnResponse ExecuteActionForSurveyCreate(string data)
        {
            if (Switch == DataSwitch.Created)
                return new Tc.Crm.Service.Models.SurveyReturnResponse { FailedSurveys=new List<FailedSurvey>() };
        
            else if (Switch == DataSwitch.Response_NULL)
                throw new InvalidOperationException(Tc.Crm.Service.Constants.Messages.ResponseFromCrmIsNull);

            else if (Switch == DataSwitch.Response_Failed)
                throw new InvalidOperationException(Tc.Crm.Service.Constants.Messages.ResponseFromCrmIsNull);
            else if (Switch == DataSwitch.Return_NULL)
                return null;
            else if (Switch == DataSwitch.ActionThrowsError)
                throw new Exception("Action faulted");
            return null;
        }

        public Collection<Brand> GetBrands()
        {
            Collection<Brand> brands = new Collection<Brand>();
            brands.Add(new Brand { Code = "BUC", Id = "E1A4B6EE-1282-4913-B460-F80EA939529A" });
            return brands;
        }

        public Collection<Country> GetCountries()
        {
            Collection<Country> countries = new Collection<Country>();
            countries.Add(new Country { Code = "DE", Id = "9EC77C9B-A5D8-41BF-A363-F2C018E10305" });
            return countries;
        }

        public Collection<Currency> GetCurrencies()
        {
            Collection<Currency> currencies = new Collection<Currency>();
            currencies.Add(new Currency { Code = "EUR", Id = "B4AB639E-A230-4479-95EA-604401E092CB" });
            return currencies;
        }

        public Collection<Gateway> GetGateways()
        {
            Collection<Gateway> gateways = new Collection<Gateway>();
            gateways.Add(new Gateway { Code = "HGR", Id = "54E0C5FE-D522-4148-9192-9C885BA3CED6" });
            return gateways;
        }

        public Collection<TourOperator> GetTourOperators()
        {
            Collection<TourOperator> tos = new Collection<TourOperator>();
            tos.Add(new TourOperator { Code = "TO001", Id = "A228B648-418A-4BCE-8680-AD33740AAB79" });
            return tos;
        }

        public Collection<SourceMarket> GetSourceMarkets()
        {
            Collection<SourceMarket> countries = new Collection<SourceMarket>();
            countries.Add(new SourceMarket { Code = "DE", Id = "9EC77C9B-A5D8-41BF-A363-F2C018E10305", BusinessUnitId = "A3C4C04F-42F4-42EF-8AC6-E24746E1CAA2", TeamId= "99B4323E-5607-4FAE-88AB-5D2D82EB5078" });
            return countries;
        }

        public Collection<Hotel> GetHotels()
        {
            Collection<Hotel> countries = new Collection<Hotel>();
            countries.Add(new Hotel { Code = "hot001", Id = "273ABA3A-6B88-40B7-AF4E-E8215BB009FB", DestinationId = "7A93B49A-48B4-44C3-A0FA-A04F2872D811" });
            return countries;
        }

        public bool PingCRM()
        {
            if (Switch == DataSwitch.Response_Failed)
                return false;
            else
                return true;
        }
    }
}
