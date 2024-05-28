using CrossCuttingConcern.ExceptionHandling.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Serialization;
using System.Text.Json;

namespace CrossCuttingConcern.ExceptionHandling;

public class LockedException : Exception, IException
{
    public LockedException()
    {
    }

    protected LockedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public LockedException(string? message) : base(message)
    {
    }

    public LockedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public async Task ContextResponseWriteAsync(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status423Locked;

        var errorResponse = new ProblemDetails()
        {
            Status = context.Response.StatusCode,
            Type = nameof(LockedException),
            Title = nameof(LockedException),
            Detail = this.Message,
            Instance = context.Request.Path
        };

        JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var jsonError = JsonSerializer.Serialize(errorResponse, jsonSerializerOptions);

        await context.Response.WriteAsync(jsonError);
    }
}