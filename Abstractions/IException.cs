using Microsoft.AspNetCore.Http;

namespace CrossCuttingConcern.ExceptionHandling.Abstractions;

public interface IException
{
    Task ContextResponseWriteAsync(HttpContext context);
}
