using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Example.Services.Test
{
    [TestClass]
    public class WeatherService_Should
    {
        [TestMethod ExpectedException(typeof(ExampleServiceException))]
        public async Task Throw_Exception_On_Unsuccessful_Response()
        {
            //arrange - set up an error response
            var stubHandler = new StubHttpClientHandler();
            stubHandler.EnqueueResponse(new HttpResponseMessage(HttpStatusCode.InternalServerError));
            var weatherService = new WeatherService(new HttpClient(stubHandler));

            //act
            await weatherService.GetWeather("Eau Claire");
        }

        [TestMethod]
        public async Task Return_The_Weather_By_City()
        {
            //arrange - set up a successful response
            var stubHandler = new StubHttpClientHandler();
            stubHandler.EnqueueResponse(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(new Weather()
                {
                    data = new WeatherData()
                    {
                        location = "Eau Claire",
                        temperature = "25",
                        skytext = "Scattered clouds",
                        humidity = "50",
                        wind = "15.52 km/h",
                        date = "06-17-2017"
                    }
                }))
            });
            var weatherService = new WeatherService(new HttpClient(stubHandler));

            //act
            var weather = await weatherService.GetWeather("Eau Claire");

            //assert
            Assert.AreEqual("Eau Claire", weather?.data?.location);
            Assert.AreEqual("25", weather?.data?.temperature);
            Assert.AreEqual("Scattered clouds", weather?.data?.skytext);
            Assert.AreEqual("50", weather?.data?.humidity);
            Assert.AreEqual("15.52 km/h", weather?.data?.wind);
            Assert.AreEqual("06-17-2017", weather?.data?.date);
        }

        [TestMethod, TestCategory("Integration")]
        public async Task Return_The_Weather_By_City_From_Live_Service()
        {
            //arrange - target the real service
            var weatherService = new WeatherService(new HttpClient());

            //act
            var weather = await weatherService.GetWeather("Eau Claire");

            //assert
            Assert.AreEqual("Eau Claire", weather?.data?.location);
            Assert.IsNotNull(weather?.data?.temperature);
            Assert.IsNotNull(weather?.data?.skytext);
            Assert.IsNotNull(weather?.data?.humidity);
            Assert.IsNotNull(weather?.data?.wind);
            Assert.IsNotNull(weather?.data?.date);
        }
    }
}
