using CrossCuttingConcern.ExceptionHandling.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Serialization;
using System.Text.Json;

namespace CrossCuttingConcern.ExceptionHandling;

public class BusinessException : Exception, IException
{
    public BusinessException()
    {
    }

    protected BusinessException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public BusinessException(string? message) : base(message)
    {
    }

    public BusinessException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public async Task ContextResponseWriteAsync(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;

        var errorResponse = new ProblemDetails()
        {
            Status = context.Response.StatusCode,
            Type = nameof(BusinessException),
            Title = nameof(BusinessException),
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
