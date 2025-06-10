using System.Net;
using System.Text.Json;
using BlogSystem.Shared.Exceptions;

namespace BlogSystem.Common.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
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
            catch (ApplicationCustomException ex)
            {
                await HandleCustomExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleCustomExceptionAsync(HttpContext context, ApplicationCustomException exception)
        {
            _logger.LogError(exception, "An application custom exception occurred: {Message}", exception.Message);

            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = exception.StatusCode;

            var errorResponse = new
            {
                StatusCode = exception.StatusCode,
                ErrorCode = exception.ErrorCode,
                Message = exception.Message,
                Timestamp = DateTime.UtcNow
            };

            await response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var errorResponse = new
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "An unexpected error occurred.",
                Timestamp = DateTime.UtcNow
            };

            await response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }
}