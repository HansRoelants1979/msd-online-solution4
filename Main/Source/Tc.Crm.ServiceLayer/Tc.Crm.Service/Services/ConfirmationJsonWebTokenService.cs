using System.Diagnostics;
using Tc.Crm.Service.Models;

namespace Tc.Crm.Service.Services
{
    public class ConfirmationJsonWebTokenService : JsonWebTokenServiceBase
    {

        public ConfirmationJsonWebTokenService(IConfigurationService configurationService)
        {
            this.ConfigurationService = configurationService;
        }

        public override void ValidateSignature(JsonWebTokenRequest jsonWebTokenRequest)
        {
            var fileNames = ConfigurationService.GetPublicKeyFileNames(Api.Confirmation);
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