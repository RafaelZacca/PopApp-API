using System;
using System.Net;

namespace Core.Supports.Exceptions
{
    public class HttpFailedStatusException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public HttpFailedStatusException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
