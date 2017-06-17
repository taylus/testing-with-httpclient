using System;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Example.Services.Test
{
    [TestClass]
    public class FooService_Should
    {
        [TestMethod, ExpectedException(typeof(ExampleServiceException))]
        public async Task Throw_Exception_On_Unsuccessful_Response()
        {
            //arrange - set up an error response
            var stubHandler = new StubHttpClientHandler();
            stubHandler.EnqueueResponse(new HttpResponseMessage(HttpStatusCode.InternalServerError));
            var httpClient = new HttpClient(stubHandler);
            var fooService = new FooService(httpClient);

            //act
            await fooService.GetById(1);
        }

        [TestMethod]
        public async Task Return_Foos_By_Id()
        {
            //arrange - set up a successful response w/ serialized Foo data in the body
            var stubHandler = new StubHttpClientHandler();
            stubHandler.EnqueueResponse(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(BuildFooJson(1, "foo", new Dictionary<string, string> { { "key", "value" } }).ToString())
            });
            var httpClient = new HttpClient(stubHandler);
            var fooService = new FooService(httpClient);

            //act
            var foo = await fooService.GetById(1);

            //assert
            Assert.IsNotNull(foo, "Expected a Foo to be returned.");
            Assert.AreEqual(1, foo.Id, "Foo ID was not the expected value.");
            Assert.AreEqual("foo", foo.Name, "Foo name was not the expected value.");
            CollectionAssert.AreEquivalent(new Dictionary<string, string> { { "key", "value" } }, foo.Data, "Foo data doesn't look as expected.");
        }

        [TestMethod]
        public async Task Return_All_Foos()
        {
            //arrange - set up a successful response w/ serialized list of Foo data in the body
            var stubHandler = new StubHttpClientHandler();
            stubHandler.EnqueueResponse(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(BuildFooListJson(5).ToString())
            });
            var httpClient = new HttpClient(stubHandler);
            var fooService = new FooService(httpClient);

            //act
            var foos = await fooService.GetAll();

            //assert
            Assert.AreEqual(5, foos.Count, "Did not get expected number of Foo objects.");
        }

        [TestMethod]
        public async Task Save_Foos()
        {
            //arrange - set up a successful "ok, I saved your thing" response
            var stubHandler = new StubHttpClientHandler();
            var stubResponse = new HttpResponseMessage(HttpStatusCode.Created);
            stubResponse.Headers.Location = new Uri("http://localhost.fiddler/foo/1");
            stubHandler.EnqueueResponse(stubResponse);
            var httpClient = new HttpClient(stubHandler);
            var fooService = new FooService(httpClient);

            //act
            await fooService.Save(new Foo(1, "apple", new Dictionary<string, string> { { "type", "fruit" }, { "color", "red" } }));

            //assert - in this case we care more about how the request looks than the response
            Assert.AreEqual(1, stubHandler.Requests.Count);
            var expectedRequestUri = new Uri(httpClient.BaseAddress, "foo");
            Assert.AreEqual(expectedRequestUri, stubHandler.Requests[0].RequestUri);
            Assert.AreEqual(HttpMethod.Post, stubHandler.Requests[0].Method);
            Assert.AreEqual(1, stubHandler.RequestContents.Count);
            Assert.AreEqual("{\"Id\":1,\"Name\":\"apple\",\"Data\":{\"type\":\"fruit\",\"color\":\"red\"}}", stubHandler.RequestContents[0]);
        }

        [TestMethod]
        public async Task Delete_Foos()
        {
            //arrange - set up a successful "ok, I deleted your thing" response
            var stubHandler = new StubHttpClientHandler();
            var stubResponse = new HttpResponseMessage(HttpStatusCode.NoContent);
            stubHandler.EnqueueResponse(stubResponse);
            var httpClient = new HttpClient(stubHandler);
            var fooService = new FooService(httpClient);

            //act
            await fooService.Delete(1);

            //assert - in this case we care more about how the request looks than the response
            Assert.AreEqual(1, stubHandler.Requests.Count);
            var expectedRequestUri = new Uri(httpClient.BaseAddress, "foo/1");
            Assert.AreEqual(expectedRequestUri, stubHandler.Requests[0].RequestUri);
            Assert.AreEqual(HttpMethod.Delete, stubHandler.Requests[0].Method);
            Assert.IsNull(stubHandler.Requests[0].Content);
        }

        /// <summary>
        /// Returns a JObject representing a sample <see cref="Foo"/>.
        /// </summary>
        private static JObject BuildFooJson(int id, string name, Dictionary<string, string> data)
        {
            return JObject.FromObject(new
            {
                Id = id,
                Name = name,
                Data = data
            });
        }

        /// <summary>
        /// Returns a JArray representing multiple sample <see cref="Foo"/> objects.
        /// </summary>
        private static JArray BuildFooListJson(int howMany)
        {
            return JArray.FromObject(Enumerable.Range(0, howMany).Select(f => BuildFooJson(f, "foo #" + f, null)));
        }
    }
}
