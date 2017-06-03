using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.Services
{
    public class BookingJsonWebTokenService : JsonWebTokenServiceBase
    {
        public BookingJsonWebTokenService(IConfigurationService configurationService)
        {
            this.ConfigurationService = configurationService;
        }

        public override void ValidateHeader(JsonWebTokenRequest request)
        {
            base.ValidateHeader(request);
        }

        public override void ValidatePayload(JsonWebTokenRequest request)
        {
            base.ValidatePayload(request);
        }

        public override void ValidateSignature(JsonWebTokenRequest jsonWebTokenRequest)
        {
            var fileNames = ConfigurationService.GetPublicKeyFileNames(Api.Booking);
            if (fileNames == null || fileNames.Count == 0)
            {
                Trace.TraceWarning("Public key file name not present in config.");
                jsonWebTokenRequest.SignatureValid = false;
                return;
            }

            foreach (var fileName in fileNames)
            {
                this.ValidateSignatureFor(jsonWebTokenRequest, fileName);
                if (jsonWebTokenRequest.SignatureValid)
                    return;
            }
        }

        public override string GetToken(HttpRequestMessage request)
        {
            return base.GetToken(request);  
        }
    }
}