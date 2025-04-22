using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging; // Add this for logging
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace PhysicalPersonsDirectory.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IStringLocalizer<ExceptionHandlingMiddleware> _localizer;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger; // Add logger

    public ExceptionHandlingMiddleware(
        RequestDelegate next, 
        IStringLocalizer<ExceptionHandlingMiddleware> localizer, 
        ILogger<ExceptionHandlingMiddleware> logger) // Inject logger
    {
        _next = next;
        _localizer = localizer;
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
            _logger.LogError(ex, "Unhandled exception caught in middleware");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            _logger.LogWarning("Response has already started, skipping middleware handling");
            return;
        }

        context.Response.ContentType = "application/json";

        object response;

        switch (exception)
        {
            case ArgumentException argEx:
                _logger.LogWarning("Handling ArgumentException in middleware: {Message}", argEx.Message);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new { error = _localizer["InvalidRequest"].Value, details = argEx.Message };
                break;
            default:
                _logger.LogError(exception, "Handling unexpected exception in middleware");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new { message = _localizer["GenericError"].Value };
                break;
        }

        var result = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(result);
    }
}

// Marker class for resource localization
public class SharedResources
{
}