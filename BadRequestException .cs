using CrossCuttingConcern.ExceptionHandling.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Serialization;
using System.Text.Json;

namespace CrossCuttingConcern.ExceptionHandling;

public class BadRequestException : Exception, IException
{
    public BadRequestException()
    {
    }

    protected BadRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public BadRequestException(string? message) : base(message)
    {
    }

    public BadRequestException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public async Task ContextResponseWriteAsync(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        var errorResponse = new ProblemDetails()
        {
            Status = context.Response.StatusCode,
            Type = nameof(BadRequestException),
            Title = nameof(BadRequestException),
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
