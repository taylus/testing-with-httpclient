using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace Example.Services
{
    /// <summary>
    /// An HTTP message handler which logs the requests and responses passing through it.
    /// </summary>
    public class LoggingHandler : DelegatingHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LoggingHandler));

        /// <summary>
        /// Creates a new isntance of this handler with the given "inner"
        /// handler which requests are sent to for further processing.
        /// </summary>
        public LoggingHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

        /// <summary>
        /// Processes the given request through the message handler pipeline,
        /// logging the request and response along the way.
        /// </summary>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            log.Debug("Request: " + request.ToString());
            if(request.Content != null)
            {
                log.Debug("Request content: " + await request.Content.ReadAsStringAsync());
            }

            var response = await base.SendAsync(request, cancellationToken);
            log.Debug("Response: " + response.ToString());
            if(response.Content != null)
            {
                log.Debug("Response content: " + await response.Content.ReadAsStringAsync());
            }

            return response;
        }
    }
}
