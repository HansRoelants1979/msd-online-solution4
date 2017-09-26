using System.Net;

namespace Tc.Crm.Service.Models
{
    public class ConfirmationResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
    }
}