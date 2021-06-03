using System;
using System.Net;

namespace Application.Errors
{
    public class RestException : Exception
    {
        private HttpStatusCode StatusCode { get; }
        private object Errors { get; }
        
        public RestException(HttpStatusCode statusCode, object errors = null)
        {
            StatusCode = statusCode;
            Errors = errors;
        }
    }
}