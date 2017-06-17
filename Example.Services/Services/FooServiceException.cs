using System;
using System.Net;

namespace Example.Services
{
    /// <summary>
    /// Represents errors that occur while communicating with the Foo service.
    /// </summary>
    public class FooServiceException : Exception
    {
        public HttpStatusCode StatusCode { get; protected set; }

        public FooServiceException(string message, HttpStatusCode statusCode) : this(message, statusCode, null) { }
        public FooServiceException(string message, HttpStatusCode statusCode, Exception innerException) : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
