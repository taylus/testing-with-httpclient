using System.Threading.Tasks;

namespace Example.Services
{
    public interface IWeatherService
    {
        /// <summary>
        /// Gets the weather for the given city.
        /// </summary>
        Task<Weather> GetWeather(string city);
    }
}
