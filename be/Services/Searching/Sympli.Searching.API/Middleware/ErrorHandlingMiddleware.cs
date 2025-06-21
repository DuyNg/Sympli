using Sympli.Searching.Core.Exceptions;
using System.Text.Json;

namespace Sympli.Searching.API.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Generate a correlation ID for tracing
            var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault() ?? Guid.NewGuid().ToString();
            context.Items["CorrelationId"] = correlationId;

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, correlationId);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception, string correlationId)
        {
            // Log the exception with context
            _logger.LogError(exception, "Error in request {CorrelationId} at {RequestPath}", correlationId, context.Request.Path);

            // Determine the status code and message based on the exception type
            var (statusCode, errorMessage) = exception switch
            {
                ConfigurationMissingException configEx =>
                    (StatusCodes.Status500InternalServerError, $"Configuration error: {configEx.Message}"),
                SearchProviderException providerEx =>
                    (StatusCodes.Status503ServiceUnavailable, providerEx.Message),
                ArgumentOutOfRangeException argEx when argEx.Message.Contains("Unhandled engine") =>
                    (StatusCodes.Status400BadRequest, "Invalid search engine specified."),
                _ =>
                    (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
            };

            // Set the response
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var errorResponse = new
            {
                StatusCode = statusCode,
                Message = errorMessage,
                Details = exception.Message,
                CorrelationId = correlationId,
                Timestamp = DateTime.UtcNow
            };

            var jsonResponse = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(jsonResponse);
        }
    }

    // Extension method for registration
    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}