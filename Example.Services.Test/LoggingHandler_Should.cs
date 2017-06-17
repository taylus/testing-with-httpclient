using System.Net;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using log4net.Config;
using log4net.Appender;

namespace Example.Services.Test
{
    [TestClass]
    public class LoggingHandler_Should
    {
        [TestMethod]
        public async Task WriteLogs()
        {
            //arrange - set up an in-memory log4net appender
            var memoryAppender = new MemoryAppender();
            BasicConfigurator.Configure(memoryAppender);

            //set up an HTTP client using the logging handler and give it a response with no content
            var stubHandler = new StubHttpClientHandler();
            stubHandler.EnqueueResponse(new HttpResponseMessage(HttpStatusCode.NoContent));
            var client = new HttpClient(new LoggingHandler(stubHandler));

            //act
            await client.GetAsync("http://localhost.fiddler");

            //assert
            var logMessages = memoryAppender.GetEvents();
            Assert.AreEqual(2, logMessages.Length);
            Assert.IsTrue(logMessages.Any(l => l.RenderedMessage.StartsWith("Request:")), "An expected message was not logged.");
            Assert.IsTrue(logMessages.Any(l => l.RenderedMessage.StartsWith("Response:")), "An expected message was not logged.");
        }

        [TestMethod]
        public async Task WriteLogsWithContent()
        {
            //arrange - set up an in-memory log4net appender
            var memoryAppender = new MemoryAppender();
            BasicConfigurator.Configure(memoryAppender);

            //set up an HTTP client using the logging handler and give it a response with content
            var stubHandler = new StubHttpClientHandler();
            stubHandler.EnqueueResponse(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent("something went wrong") });
            var client = new HttpClient(new LoggingHandler(stubHandler));

            //act
            await client.PostAsync("http://localhost.fiddler", new StringContent("request data"));

            //assert
            var logMessages = memoryAppender.GetEvents();
            Assert.AreEqual(4, logMessages.Length);
            Assert.IsTrue(logMessages.Any(l => l.RenderedMessage.StartsWith("Request:")), "An expected message was not logged.");
            Assert.IsTrue(logMessages.Any(l => l.RenderedMessage.Contains("request data")), "An expected message was not logged.");
            Assert.IsTrue(logMessages.Any(l => l.RenderedMessage.StartsWith("Response:")), "An expected message was not logged.");
            Assert.IsTrue(logMessages.Any(l => l.RenderedMessage.Contains("something went wrong")), "An expected message was not logged.");
        }
    }
}
