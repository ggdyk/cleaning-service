using System.Text.Json;
using Api.Models;
using Domain.Exceptions;
using FluentValidation;

namespace Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
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

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, error, message, validationErrors) = exception switch
        {
            NotFoundException ex =>
                (StatusCodes.Status404NotFound, "NotFound", ex.Message, (IDictionary<string, string[]>?)null),

            BusinessRuleException ex =>
                (StatusCodes.Status409Conflict, "BusinessRuleViolation", ex.Message, null),

            ForbiddenException ex =>
                (StatusCodes.Status403Forbidden, "Forbidden", ex.Message, null),

            ValidationException ex =>
                (StatusCodes.Status400BadRequest, "ValidationError", "Ошибка валидации.",
                    ex.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray())),

            UnauthorizedAccessException ex =>
                (StatusCodes.Status401Unauthorized, "Unauthorized", ex.Message, null),

            _ => (StatusCodes.Status500InternalServerError, "InternalServerError",
                  GetInternalErrorMessage(exception), null)
        };

        if (statusCode >= 500)
            _logger.LogError(exception, "Необработанное исключение: {Message}", exception.Message);
        else
            _logger.LogWarning("Клиентская ошибка {StatusCode}: {Message}", statusCode, exception.Message);
        
        var response = new ApiErrorResponse
        {
            StatusCode = statusCode,
            Error = error,
            Message = message,
            ValidationErrors = validationErrors,
            TraceId = context.TraceIdentifier
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }

    private string GetInternalErrorMessage(Exception exception)
    {
        return _environment.IsDevelopment()
            ? exception.ToString()
            : "Произошла внутренняя ошибка сервера. Обратитесь в поддержку, указав TraceId.";
    }
}