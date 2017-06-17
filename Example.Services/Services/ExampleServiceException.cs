using System;
using System.Net;

namespace Example.Services
{
    /// <summary>
    /// Represents errors that occur while communicating with a service in this project.
    /// </summary>
    public class ExampleServiceException : Exception
    {
        public HttpStatusCode StatusCode { get; protected set; }

        public ExampleServiceException(string message, HttpStatusCode statusCode) : this(message, statusCode, null) { }
        public ExampleServiceException(string message, HttpStatusCode statusCode, Exception innerException) : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
