﻿using System.Net.Http;
using System.Threading.Tasks;

namespace Example.Services
{
    public abstract class BaseService
    {
        /// <summary>
        /// The HttpClient this service uses for HTTP communication.
        /// </summary>
        public HttpClient HttpClient { get; protected set; }

        /// <summary>
        /// Sends the given request to the underlying HttpClient and returns its response.
        /// Throws an <see cref="ExampleServiceException"/> if we get an unsuccessful response.
        /// </summary>
        protected virtual async Task<HttpResponseMessage> GetResponseAsync(HttpRequestMessage request)
        {
            var response = await HttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return response;
            }
            {
                throw new ExampleServiceException(string.Format("Received a failure response from service: {0} {1}. Body: {2}",
                    (int)response.StatusCode, response.StatusCode, await SafeReadContentFrom(response) ?? "(null)"), response.StatusCode);
            }
        }

        /// <summary>
        /// Sends the given request to the underlying HttpClient and returns its response content.
        /// </summary>
        protected virtual async Task<string> GetResponseContentAsync(HttpRequestMessage request)
        {
            var response = await GetResponseAsync(request);
            return await SafeReadContentFrom(response);
        }

        /// <summary>
        /// Reads the content from the given response and returns it as a string.
        /// Returns null if the response has no content (e.g. HTTP 204)
        /// </summary>
        protected virtual async Task<string> SafeReadContentFrom(HttpResponseMessage response)
        {
            return await (response.Content?.ReadAsStringAsync() ?? Task.FromResult<string>(null));
        }
    }
}
