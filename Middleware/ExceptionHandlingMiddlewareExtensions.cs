using Microsoft.AspNetCore.Builder;

namespace CrossCuttingConcern.ExceptionHandling.Middleware;

public static class ExceptionHandlingMiddlewareExtensions
{
    public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
