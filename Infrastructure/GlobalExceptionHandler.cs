using BlogFlow.API.DTOs.Common;
using BlogFlow.API.Exceptions;
using BlogFlow.API.Exceptions.Base;
using Microsoft.AspNetCore.Diagnostics;
using System.Net.Mime;

namespace BlogFlow.API.Infrastructure
{
    // Centralized handler for all unhandled API exceptions
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
            => _logger = logger;

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            // Safe fallback for unknown/unhandled exceptions
            int statusCode = StatusCodes.Status500InternalServerError;
            string message = "An unexpected error occurred on the server.";
            string errorCode = "INTERNAL_SERVER_ERROR";
            IEnumerable<string>? errors = null;

            // Use custom exception details when available
            if (exception is AppException appEx)
            {
                statusCode = appEx.StatusCode;
                message = appEx.Message;
                errorCode = appEx.ErrorCode;
                errors = appEx.Errors;
            }

            // Standardized API error response
            var response = new ErrorResponseDTO
            {
                StatusCode = statusCode,
                Message = message,
                ErrorCode = errorCode,
                Timestamp = DateTimeOffset.UtcNow, // Exact failure time (UTC)
                TraceId = httpContext.TraceIdentifier, // Helps trace request in logs
                Errors = errors
            };

            var logLevel = exception switch
            {
                NotFoundException => LogLevel.Information,
                UnauthorizedException => LogLevel.Warning,
                ForbiddenException => LogLevel.Warning,
                BadRequestException => LogLevel.Warning,
                ConflictException => LogLevel.Warning,
                _ => LogLevel.Error
            };

            if (_logger.IsEnabled(logLevel))
            {
                _logger.Log(
                    logLevel,
                    exception,
                    "Exception occurred during {Method} {Path}. StatusCode: {StatusCode}, ErrorCode: {ErrorCode}, TraceId: {TraceId}",
                    httpContext.Request.Method,
                    httpContext.Request.Path,
                    statusCode,
                    errorCode,
                    httpContext.TraceIdentifier);
            }

            // Configure HTTP response
            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = MediaTypeNames.Application.Json;

            // Serialize DTO into JSON response body
            await httpContext.Response.WriteAsJsonAsync(
                response,
                cancellationToken);

            // Tell ASP.NET the exception has been handled
            return true;
        }
    }
}