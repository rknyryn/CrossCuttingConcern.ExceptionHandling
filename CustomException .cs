using CrossCuttingConcern.ExceptionHandling.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Serialization;
using System.Text.Json;

namespace CrossCuttingConcern.ExceptionHandling;

public class CustomException : Exception, IException
{
    public int Code { get; }

    public CustomException(int code)
    {
        Code = code;
    }

    protected CustomException(int code, SerializationInfo info, StreamingContext context) : base(info, context)
    {
        Code = code;
    }

    public CustomException(int code, string? message) : base(message)
    {
        Code = code;
    }

    public CustomException(int code, string? message, Exception? innerException) : base(message, innerException)
    {
        Code = code;
    }

    public async Task ContextResponseWriteAsync(HttpContext context)
    {
        context.Response.StatusCode = this.Code;

        var errorResponse = new ProblemDetails()
        {
            Status = context.Response.StatusCode,
            Type = nameof(Exception),
            Title = nameof(Exception),
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
