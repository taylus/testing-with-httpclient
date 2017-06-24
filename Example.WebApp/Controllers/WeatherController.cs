using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Example.Services;

namespace Example.WebApp.Controllers
{
    public class WeatherController : Controller
    {
        private IWeatherService weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            this.weatherService = weatherService;
        }

        public async Task<IActionResult> GetWeather(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                ViewBag.Weather = "Please provide a city via query string: ?city=xyz.";
            }
            else
            {
                ViewBag.Weather = await weatherService.GetWeather(city);
            }
            return View();
        }
    }
}