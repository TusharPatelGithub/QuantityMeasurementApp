using System.Net;
using System.Text.Json;
using BusinessLayer.Exceptions;

namespace QuantityMeasurementApp.Middleware;

/// <summary>
/// Global exception handler middleware that catches all unhandled exceptions
/// and returns appropriate HTTP responses (400 for business logic errors, 500 otherwise).
/// </summary>
public class GlobalExceptionHandler : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (QuantityMeasurementException ex)
        {
            // Known business rules exceptions -> 400 Bad Request
            _logger.LogWarning(ex, "Business rule violation: {Message}", ex.Message);
            await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            // Unknown exceptions -> 500 Internal Server Error
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred processing your request.");
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            error = message,
            timestamp = DateTime.UtcNow
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
