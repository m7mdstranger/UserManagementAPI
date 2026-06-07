using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Exceptions;

namespace UserManagementAPI.Middleware
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
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
                _logger.LogError($"An unexpected error occurred: {ex.Message}");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ErrorResponse();

            switch (exception)
            {
                case ValidationException validationEx:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response = new ErrorResponse
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Validation failed",
                        Errors = validationEx.Errors
                    };
                    break;

                case NotFoundException notFoundEx:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    response = new ErrorResponse
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = notFoundEx.Message,
                        Errors = new List<string> { notFoundEx.Message }
                    };
                    break;

                case DbUpdateException dbEx:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response = new ErrorResponse
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Database constraint violation",
                        Errors = new List<string> { dbEx.InnerException?.Message ?? dbEx.Message }
                    };
                    break;

                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response = new ErrorResponse
                    {
                        StatusCode = StatusCodes.Status500InternalServerError,
                        Message = "An unexpected error occurred",
                        Errors = new List<string> { exception.Message }
                    };
                    break;
            }

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);
            return context.Response.WriteAsync(json);
        }
    }

    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}