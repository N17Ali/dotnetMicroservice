namespace PlatformsService.Middleware;

public static class ErrorHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomErrorHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
}