using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.Services
{
    public class SurveyJsonWebTokenService : JsonWebTokenServiceBase
    {

        public SurveyJsonWebTokenService(IConfigurationService configurationService)
        {
            this.ConfigurationService = configurationService;
        }
       
        public override void ValidateSignature(JsonWebTokenRequest jsonWebTokenRequest)
        {
            var fileNames = ConfigurationService.GetPublicKeyFileNames(Api.Survey);
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

        
    }
}