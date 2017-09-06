using System;
using System.Collections.Generic;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.Xrm.Sdk;
using Tc.Crm.Common.Constants;
using Tc.Crm.Common.Services;
using Tc.Crm.OutboundSynchronisation.Customer.Model;


namespace Tc.Crm.OutboundSynchronisation.Customer.Services
{
    public class OutboundSynchronisationService : IOutboundSynchronisationService
    {
        private bool disposed;
        private readonly IOutboundSynchronisationDataService outboundSynchronisationDataService;
        private readonly IConfigurationService configurationService;
        private readonly ILogger logger;

        public OutboundSynchronisationService(ILogger logger, IOutboundSynchronisationDataService outboundSynchronisationService, IConfigurationService configurationService)
        {
            this.outboundSynchronisationDataService = outboundSynchronisationService;
            this.logger = logger;
            this.configurationService = configurationService;
        }

        public string SecretKey { get; set; }

        public void Run()
        {
            ProcessEntityCache();

        }

        public void ProcessEntityCache()
        {
            var entityCacheCollection = outboundSynchronisationDataService.GetEntityCacheToProcess(EntityName.Contact, 1000);


        }

        public void UpdateEntityCacheStatus(int StateCode, int StatusReason)
        {
            throw new NotImplementedException();
        }

        public void CreateEntityCacheMessage(EntityCacheMessageModel entityCacheMessageModel)
        {
           var recordId = outboundSynchronisationDataService.CreateEntityCacheMessage(entityCacheMessageModel);
        }

        public void UpdateEntityCacheMessageStatus(int StateCode, int StatusReason)
        {
            throw new NotImplementedException();
        }

        #region Displosable members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                DisposeObject(outboundSynchronisationDataService);
                DisposeObject(logger);
                DisposeObject(configurationService);
            }

            disposed = true;
        }

        private void DisposeObject(object obj)
        {
            if (obj != null)
            {
                if (obj is IDisposable)
                    ((IDisposable)obj).Dispose();
                else
                    obj = null;
            }
        }

       

        #endregion

        #region Private methods

        private string CreateJWTToken(Dictionary<string, object> payload, ITracingService trace, IOrganizationService service)
        {
            //if (trace == null) throw new ArgumentException(ValidationMessages.TraceIsNull);
            //if (payload == null) throw new ArgumentException(ValidationMessages.CachingParameterIsNull);
            //if (service == null) throw new ArgumentException(ValidationMessages.OrganizationServiceIsNull);

            //trace.Trace("Start - CreateJWTToken");

            //if (string.IsNullOrWhiteSpace(this.SecretKey))
            //   throw new ArgumentException(ValidationMessages.CachingSecretKeyIsNullOrEmpty);

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            return encoder.Encode(payload, this.SecretKey);
        }

        #endregion Private methods
    }
}