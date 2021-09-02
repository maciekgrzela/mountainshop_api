using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Core;
using Application.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace API.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            object errors = null;
            switch (exception)
            {
                case RestException re:
                    errors = re.Errors;
                    context.Response.StatusCode = (int) GetStatusCodeByHandlerResponse(re.Response);
                    break;
                case { } ex:
                    errors = string.IsNullOrWhiteSpace(ex.Message) ? "Error" : ex.Message;
                    context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                    break;
            }

            context.Response.ContentType = "application/json";
            var results = JsonSerializer.Serialize(new {
                errors
            });

            await context.Response.WriteAsync(results);
        }

        private HttpStatusCode GetStatusCodeByHandlerResponse(HandlerResponse response)
        {
            return response switch
            {
                HandlerResponse.ResourceNotFound => HttpStatusCode.NotFound,
                HandlerResponse.InvalidRequest => HttpStatusCode.BadRequest,
                HandlerResponse.ClientHasNoAccess => HttpStatusCode.Forbidden,
                HandlerResponse.ClientIsNotAuthorized => HttpStatusCode.Unauthorized,
                _ => HttpStatusCode.InternalServerError
            };
        }
    }
}