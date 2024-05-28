using CrossCuttingConcern.ExceptionHandling.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrossCuttingConcern.ExceptionHandling.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception, nameof(exception));

        if (exception.GetType().GetInterface(nameof(IException)) == typeof(IException))
        {
            await ((IException)exception).ContextResponseWriteAsync(context);
            return;
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsync(new ProblemDetails
        {
            Status = context.Response.StatusCode,
            Type = "Internal",
            Title = "Internal exception",
            Detail = "Something went wrong!",
        }.ToString());
    }
}
