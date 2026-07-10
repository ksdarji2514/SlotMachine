using System.Net;
using System.Text.Json;
using SlotMachine.Common.Exceptions;
using SlotMachine.DTO.Common;

namespace SlotMachine.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    public ExceptionHandlingMiddleware(
        RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // run the rest of the pipeline
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception ex)
    {
        // Map exception type -> status code + message.
        var (status, message) = ex switch
        {
            PlayerNotFoundException => (HttpStatusCode.NotFound, ex.Message),   // 404
            InsufficientBalanceException => (HttpStatusCode.Conflict, ex.Message),  // 409
            InvalidOperationException => (HttpStatusCode.InternalServerError, ex.Message),
            _ => (HttpStatusCode.InternalServerError,
                                            "An unexpected error occurred.")
        };

        // Log the full detail server-side; return only a safe message to the client.
        if (status == HttpStatusCode.InternalServerError)
            _logger.LogError(ex, "Unhandled exception");

        context.Response.StatusCode = (int)status;
        context.Response.ContentType = "application/json";

        var body = ApiResponse<string>.Fail(message);
        await context.Response.WriteAsync(JsonSerializer.Serialize(body, JsonOptions));
    }
}