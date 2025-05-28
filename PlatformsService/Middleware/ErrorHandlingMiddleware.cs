using Microsoft.AspNetCore.Mvc;
using PlatformsService.Exceptions;
using Microsoft.EntityFrameworkCore;

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
        catch (DuplicateResourceException ex)
        {
            await HandleSpecificProblemAsync(context, ex,
                StatusCodes.Status409Conflict,
                "Resource Conflict.",
                ex.Message,
                "https://tools.ietf.org/html/rfc7231#section-6.5.8"
            );
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await HandleSpecificProblemAsync(context, ex,
                StatusCodes.Status409Conflict,
                "Concurrency Conflict.",
                "The resource has been modified by another operation since it was last retrieved. Please refresh and try again.",
                "https://tools.ietf.org/html/rfc7231#section-6.5.8"
            );
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException?.Message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) == true)
            {
                await HandleSpecificProblemAsync(context, ex,
                   StatusCodes.Status409Conflict,
                   "Resource Conflict.",
                   "A resource with the same unique identifier already exists.",
                   "https://tools.ietf.org/html/rfc7231#section-6.5.8"
               );
            }
            else
            {
                await HandleSpecificProblemAsync(context, ex,
                    StatusCodes.Status500InternalServerError,
                    "Database Update Error.",
                    "An error occurred while updating the database.",
                    "https://tools.ietf.org/html/rfc7231#section-6.6.1" // Still a server error
                );
            }
        }
        catch (KeyNotFoundException ex)
        {
            await HandleSpecificProblemAsync(context, ex,
                StatusCodes.Status404NotFound,
                "Resource Not Found.",
                ex.Message,
                "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            );
        }
        catch (ResourceNotFoundException ex)
        {
            await HandleSpecificProblemAsync(context, ex,
                StatusCodes.Status404NotFound,
                "Resource Not Found.",
                ex.Message,
                "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            );
        }
        catch (Exception ex)
        {
            await HandleGenericExceptionAsync(context, ex);
        }
    }

    private async Task HandleSpecificProblemAsync(HttpContext context, Exception ex, int statusCode, string title, string detail, string type)
    {
        _logger.LogError(ex, "Caught specific exception: {ExceptionType} - {Message}", ex.GetType().Name, ex.Message);

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = type
        };

        var errorsDictionary = new Dictionary<string, string[]>();

        if (_env.IsDevelopment())
        {
            problemDetails.Extensions["exceptionDetails"] = ex.ToString();
        }
        else
        {
            errorsDictionary.Add("general", [detail]);
        }

        problemDetails.Extensions["errors"] = errorsDictionary;

        context.Response.StatusCode = problemDetails.Status.GetValueOrDefault(StatusCodes.Status500InternalServerError);
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