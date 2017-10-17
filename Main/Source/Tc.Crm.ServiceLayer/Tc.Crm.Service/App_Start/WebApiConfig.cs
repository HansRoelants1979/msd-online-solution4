﻿using System.Web.Http;
using JsonPatch;
using JsonPatch.Formatting;
using JsonPatch.Paths.Resolvers;
using Microsoft.Practices.Unity;
using Newtonsoft.Json.Converters;
using Tc.Crm.Service.CacheBuckets;
using Tc.Crm.Service.MessageHandlers;
using Tc.Crm.Service.Models;
using Tc.Crm.Service.Resolver;
using Tc.Crm.Service.Services;

namespace Tc.Crm.Service
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Api")]
    public static class WebApiConfig
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static void Register(HttpConfiguration config)
        {
            config.MessageHandlers.Add(new BookingMessageHandler());
            // Web API configuration and services
            var container = new UnityContainer();
            container.RegisterType<IPatchParameterService, PatchParameterService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IBookingService, BookingService>();
            container.RegisterType<ICustomerService, CustomerService>();
            container.RegisterType<ISurveyService, SurveyService>();
            container.RegisterType<ICachingService, CachingService>();
			container.RegisterType<ICrmService, CrmService>(new ContainerControlledLifetimeManager());
			container.RegisterType<IReferenceBucket<Brand>, BrandBucket>(new ContainerControlledLifetimeManager());
			container.RegisterType<IReferenceBucket<Country>, CountryBucket>(new ContainerControlledLifetimeManager());
			container.RegisterType<IReferenceBucket<Currency>, CurrencyBucket>(new ContainerControlledLifetimeManager());
			container.RegisterType<IReferenceBucket<Gateway>, GatewayBucket>(new ContainerControlledLifetimeManager());
			container.RegisterType<IReferenceBucket<TourOperator>, TourOperatorBucket>(new ContainerControlledLifetimeManager());
			container.RegisterType<IReferenceBucket<SourceMarket>, SourceMarketBucket>(new ContainerControlledLifetimeManager());
			container.RegisterType<IReferenceBucket<Hotel>, HotelBucket>(new ContainerControlledLifetimeManager());
			container.RegisterType<IConfigurationService, ConfigurationService>();
            container.RegisterType<IConfirmationService, ConfirmationService>();

            config.DependencyResolver = new UnityResolver(container);
            
            // Web API routes
            config.MapHttpAttributeRoutes();
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());
            config.Formatters.Add(new JsonPatchFormatter(new JsonPatchSettings { PathResolver = new FlexiblePathResolver() }));
            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }

    }
}
