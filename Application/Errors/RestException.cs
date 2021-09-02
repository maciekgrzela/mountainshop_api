using System;
using System.Net;
using Application.Core;

namespace Application.Errors
{
    public class RestException : Exception
    {
        public HandlerResponse Response { get; }
        public object Errors { get; }
        
        public RestException(HandlerResponse response, object errors = null)
        {
            Response = response;
            Errors = errors;
        }
    }
}