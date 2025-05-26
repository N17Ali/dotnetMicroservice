using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace PlatformsService.Middleware;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IHostEnvironment env)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger = logger;
    private readonly IHostEnvironment _env = env;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BadHttpRequestException ex)
        {
            await HandleBadHttpRequestExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            await HandleGenericExceptionAsync(context, ex);
        }
    }

    private async Task HandleBadHttpRequestExceptionAsync(HttpContext context, BadHttpRequestException ex)
    {
        _logger.LogWarning(ex, "Bad HTTP Request (model binding/JSON parsing failure): {Message}", ex.Message);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "One or more request body parameters are invalid.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        };

        var errorsDictionary = new Dictionary<string, string[]>();

        if (ex.InnerException is JsonException jsonException)
        {
            var propertyName = jsonException.Path?.TrimStart('$')?.TrimStart('.')?.Split('[')[0];

            if (!string.IsNullOrEmpty(propertyName))
            {
                errorsDictionary.Add(
                    propertyName,
                    [
                        $"The value for '{propertyName}' is in an unexpected format."
                    ]);
            }
            else
            {
                errorsDictionary.Add("requestBody", [$"Invalid JSON format: {jsonException.Message.Split('\n')[0]}."]);
            }
        }
        else
        {
            errorsDictionary.Add("request", ["The request was malformed or could not be understood."]);
        }

        problemDetails.Extensions["errors"] = errorsDictionary;

        context.Response.StatusCode = problemDetails.Status.GetValueOrDefault(StatusCodes.Status400BadRequest);
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private async Task HandleGenericExceptionAsync(HttpContext context, Exception ex)
    {
        _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected server error occurred.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };

        var errorsDictionary = new Dictionary<string, string[]>();

        if (_env.IsDevelopment())
        {
            errorsDictionary.Add("exception", [ex.ToString()]);
        }
        else
        {
            errorsDictionary.Add("general", ["An unexpected server error occurred. Please try again later."]);
        }

        problemDetails.Extensions["errors"] = errorsDictionary;

        context.Response.StatusCode = problemDetails.Status.GetValueOrDefault(StatusCodes.Status500InternalServerError);
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}