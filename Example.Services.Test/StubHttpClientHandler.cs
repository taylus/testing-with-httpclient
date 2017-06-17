using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Example.Services.Test
{
    /// <summary>
    /// A test stand-in for <see cref="HttpClientHandler"/>, the default message
    /// handler which normally sends requests out to the network. <para/>
    /// 
    /// This handler maintains a queue of responses and sends them back in order.
    /// It also keeps a copy of all requests sent to it for later inspection.
    /// It is intended to be used in unit tests, where we don't want to hit actual
    /// services over HTTP.
    /// </summary>
    public class StubHttpClientHandler : DelegatingHandler
    {
        /// <summary>
        /// Internal queue of responses returned by this handler.
        /// </summary>
        private readonly Queue<HttpResponseMessage> responses = new Queue<HttpResponseMessage>();

        /// <summary>
        /// List of requests sent to this handler.
        /// </summary>
        /// <remarks>
        /// Note that request content is disposed by HttpClient once sent.
        /// If you need to make any assertions about request content in tests, use <see cref="RequestContents"/>
        /// <see>https://stackoverflow.com/questions/29369945/objectdisposedexception-on-httpclient</see>
        /// </remarks>
        public List<HttpRequestMessage> Requests { get; private set; } = new List<HttpRequestMessage>();

        /// <summary>
        /// List of request contents sent to this handler.
        /// </summary>
        public List<string> RequestContents { get; private set; } = new List<string>();

        /// <summary>
        /// Queues up the given response to be returned from this handler's SendAsync.
        /// </summary>
        public void EnqueueResponse(HttpResponseMessage response)
        {
            responses.Enqueue(response);
        }

        /// <summary>
        /// Processes the given request through the message handler pipeline.
        /// </summary>
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (responses.Count == 0) throw new InvalidOperationException("No responses are queued for this request.");
            Requests.Add(request);
            if(request.Content != null) RequestContents.Add(await request.Content.ReadAsStringAsync());
            return responses.Dequeue();
        }
    }
}
