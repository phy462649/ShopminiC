using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Exceptions;

namespace LandingPageApp.Api.Middlewares
{
    /// <summary>
    /// Global exception handling middleware that catches all exceptions and returns appropriate HTTP responses.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
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

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ErrorResponse
            {
                Success = false,
                Message = exception.Message
            };

            switch (exception)
            {
                case ValidationException validationEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.ErrorCode = "VALIDATION_ERROR";
                    response.Errors = validationEx.Errors;
                    break;

                case AuthenticationException authEx:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.ErrorCode = authEx.ErrorCode;
                    // Use generic message for authentication failures
                    response.Message = "Invalid credentials";
                    break;

                case AuthorizationException authzEx:
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    response.ErrorCode = authzEx.ErrorCode;
                    break;

                case ConflictException conflictEx:
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    response.ErrorCode = "CONFLICT";
                    break;

                case RateLimitException rateLimitEx:
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    response.ErrorCode = "RATE_LIMIT_EXCEEDED";
                    context.Response.Headers.Add("Retry-After", rateLimitEx.RetryAfterSeconds.ToString());
                    break;

                case InvalidOperationException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.ErrorCode = "NOT_FOUND";
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.ErrorCode = "INTERNAL_SERVER_ERROR";
                    response.Message = "An unexpected error occurred";
                    break;
            }

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            return context.Response.WriteAsJsonAsync(response, options);
        }
    }
}
