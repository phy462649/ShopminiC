using System.Diagnostics;

namespace LandingPageApp.Api.Middlewares;

public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimingMiddleware> _logger;

    public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        // Thêm header để tracking
        context.Response.OnStarting(() =>
        {
            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;
            
            // Thêm response header với thời gian xử lý
            context.Response.Headers["X-Response-Time-Ms"] = elapsedMs.ToString();
            
            // Log thông tin
            var endpoint = context.GetEndpoint()?.DisplayName ?? context.Request.Path;
            var method = context.Request.Method;
            var statusCode = context.Response.StatusCode;
            
            // Log với màu theo thời gian
            var logLevel = elapsedMs switch
            {
                < 100 => LogLevel.Information,
                < 500 => LogLevel.Warning,
                _ => LogLevel.Error
            };
            
            _logger.Log(logLevel, 
                "[{Method}] {Endpoint} - {StatusCode} - {ElapsedMs}ms",
                method, endpoint, statusCode, elapsedMs);
            
            return Task.CompletedTask;
        });

        await _next(context);
    }
}

public static class RequestTimingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestTiming(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestTimingMiddleware>();
    }
}
