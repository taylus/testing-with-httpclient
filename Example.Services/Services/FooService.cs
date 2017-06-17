using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Example.Services
{
    /// <summary>
    /// Implements a service for <see cref="Foo"/> objects w/ basic CRUD operaitons.
    /// </summary>
    public class FooService : IFooService
    {
        /// <summary>
        /// The HttpClient this service uses for HTTP communication.
        /// </summary>
        public HttpClient HttpClient { get; private set; }

        /// <summary>
        /// Creates an instance of this client which will use the given HttpClient.
        /// </summary>
        public FooService(HttpClient httpClient)
        {
            HttpClient = httpClient;
            if (HttpClient.BaseAddress == null) HttpClient.BaseAddress = new Uri("http://localhost.fiddler/");
        }

        /// <summary>
        /// Retrieves the <see cref="Foo"/> with the given ID.
        /// </summary>
        public async Task<Foo> GetById(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "foo");
            string responseContent = await GetResponseContentAsync(request);
            return JsonConvert.DeserializeObject<Foo>(responseContent);
        }

        /// <summary>
        /// Retrieves all <see cref="Foo"/>s that exist in the service.
        /// </summary>
        public async Task<IList<Foo>> GetAll()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "foo");
            string responseContent = await GetResponseContentAsync(request);
            return JsonConvert.DeserializeObject<IList<Foo>>(responseContent);
        }

        /// <summary>
        /// Saves the given <see cref="Foo"/>.
        /// </summary>
        public async Task Save(Foo foo)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "foo");
            request.Content = new StringContent(JsonConvert.SerializeObject(foo));
            await GetResponseAsync(request);
        }

        /// <summary>
        /// Deletes the <see cref="Foo"/> with the given ID.
        /// </summary>
        public async Task Delete(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"foo/{id}");
            await GetResponseAsync(request);
        }

        /// <summary>
        /// Sends the given request to the underlying HttpClient and returns its response.
        /// Throws a <see cref="FooServiceException"/> if we get an unsuccessful response.
        /// </summary>
        private async Task<HttpResponseMessage> GetResponseAsync(HttpRequestMessage request)
        {
            var response = await HttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return response;
            }
            {
                throw new FooServiceException(string.Format("Received a failure response from service: {0} {1}. Body: {2}",
                    (int)response.StatusCode, response.StatusCode, await SafeReadContentFrom(response) ?? "(null)"), response.StatusCode);
            }
        }

        /// <summary>
        /// Sends the given request to the underlying HttpClient and returns its response content.
        /// Throws a <see cref="FooServiceException"/> if we get an unsuccessful response.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<string> GetResponseContentAsync(HttpRequestMessage request)
        {
            var response = await GetResponseAsync(request);
            return await SafeReadContentFrom(response);
        }

        /// <summary>
        /// Reads the content from the given response and returns it as a string.
        /// Returns null if the response has no content (e.g. HTTP 204)
        /// </summary>
        private async Task<string> SafeReadContentFrom(HttpResponseMessage response)
        {
            return await (response.Content?.ReadAsStringAsync() ?? Task.FromResult<string>(null));
        }
    }
}
