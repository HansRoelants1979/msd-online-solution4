using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Tc.Crm.Service.MessageHandlers
{
    public class BookingMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var header = request.Headers.ToString();
            var t = request.Content.ReadAsStringAsync();
            Trace.TraceInformation(header);
            Trace.TraceInformation(t.Result);
            return base.SendAsync(request, cancellationToken);
        }
    }
}