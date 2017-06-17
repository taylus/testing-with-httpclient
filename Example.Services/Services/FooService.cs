using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Example.Services
{
    /// <summary>
    /// Implements a service for <see cref="Foo"/> objects w/ basic CRUD operations.
    /// </summary>
    public class FooService : BaseService, IFooService
    {
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
    }
}
