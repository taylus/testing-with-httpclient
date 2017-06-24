using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Example.Services
{
    /// <summary>
    /// Implements a weather-retrieving service using https://weathers.co/api.
    /// </summary>
    public class WeatherService : BaseService, IWeatherService
    {
        /// <summary>
        /// Creates an instance of this client which will use the given HttpClient.
        /// </summary>
        public WeatherService(HttpClient httpClient)
        {
            HttpClient = httpClient;
            if (HttpClient.BaseAddress == null) HttpClient.BaseAddress = new Uri("https://weathers.co/api.php");
        }

        /// <summary>
        /// Gets the weather for the given city.
        /// </summary>
        public async Task<Weather> GetWeather(string city)
        {
            if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("City cannot be empty.");
            var request = new HttpRequestMessage(HttpMethod.Get, $"?city={Uri.EscapeDataString(city)}");
            string responseContent = await GetResponseContentAsync(request);
            return JsonConvert.DeserializeObject<Weather>(responseContent);
        }
    }
}
