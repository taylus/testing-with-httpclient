# Writing extensible, testable code with HttpClient
## Target audience
.NET developers who want to talk to a web service (or anything HTTP, really) using a standard, well-defined, and easy to test class that comes with async support out of the box.

## Introduction
I was recently tasked with writing some wrapper code around a vendor's REST API and chose to go with .NET 4.5's HttpClient. After reading about and working with it for a while, I was very impressed with how much effort and care Microsoft put into making this class easy to use, extend, and unit test (so much so that I made this writeup).


## Extending HttpClient for unit testing
The coolest thing about HttpClient is that it operates by sending messages through a pipeline of "message handlers" which you can create and control. The default handler sends your request to a server and returns its response, but what if you want different behavior, say, during a unit test? Stick a message handler in the pipeline that returns canned responses and the code under test will be none the wiser.

This test exercises error handling in a weather service to see how it will react to an HTTP 500 Internal Server Error without ever touching the network:

```csharp
public class StubHttpClientHandler : DelegatingHandler
{
    private readonly Queue<HttpResponseMessage> responses = new Queue<HttpResponseMessage>();

    public void EnqueueResponse(HttpResponseMessage response)
    {
        responses.Enqueue(response);
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (responses.Count == 0) throw new InvalidOperationException("No responses are queued for this request.");
        return Task.FromResult(responses.Dequeue());
    }
}

[TestClass]
public class WeatherService_Should
{
    [TestMethod ExpectedException(typeof(WeatherServiceException))]
    public async Task Throw_Exception_On_Unsuccessful_Response()
    {
        var stubHandler = new StubHttpClientHandler();
        stubHandler.EnqueueResponse(new HttpResponseMessage(HttpStatusCode.InternalServerError));
        var weatherService = new WeatherService(new HttpClient(stubHandler));

        await weatherService.GetWeather("Eau Claire");
    }
}
```

# Extending HttpClient for logging
Message handlers are meant to be chained together, and this allows for a handler which passively logs all the requests and responses passing through it:

``` csharp
public class LoggingHandler : DelegatingHandler
{
    private static readonly ILog log = LogManager.GetLogger(typeof(LoggingHandler));

    public LoggingHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        log.Debug("Request: " + request.ToString());
        if(request.Content != null)
        {
            log.Debug("Request content: " + await request.Content.ReadAsStringAsync());
        }

        var response = await base.SendAsync(request, cancellationToken);
        log.Debug("Response: " + response.ToString());
        if(response.Content != null)
        {
            log.Debug("Response content: " + await response.Content.ReadAsStringAsync());
        }

        return response;
    }
}
```

I found myself easily testing this handler using log4net's MemoryAppender, which stores log messages in an array, but you could also just pass in the whole logging interface as a dependency.

More examples and explanations of extending and working with HttpClient are available in a great post here that got me started:
* https://www.thomaslevesque.com/2016/12/08/fun-with-the-httpclient-pipeline/

# A word of caution about HttpClient lifecycles
Despite implementing IDisposable, HttpClient is intended to be long-lived and reused rather than disposed in a "using" block every time you need it. Don't worry, it's [thread-safe](https://stackoverflow.com/questions/11178220/is-httpclient-safe-to-use-concurrently/11178252#11178252).

If you're using an IoC container then set up a singleton scope, at least for each unique kind of service you're talking to (there's a couple properties, like BaseAddress, that you can't change once you've started sending requests).

Further reading on this:
* https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
* http://www.nimaara.com/2016/11/01/beware-of-the-net-httpclient/


# Show me the codes
The code examples above came from a GitHub repository I'm putting together as reference for myself in the future if nothing else, but if it helps anyone else out, great!

* https://github.com/taylus/testing-with-httpclient

Aside from the IDisposable confusion, I think Microsoft did a great job with HttpClient, and I look forward to using it in future projects.
