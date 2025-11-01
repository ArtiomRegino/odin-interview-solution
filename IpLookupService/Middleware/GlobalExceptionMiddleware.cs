using IpLookupService.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace IpLookupService.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Bad request: {Message}", ex.Message);
            await WriteProblem(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (IPServiceNotAvailableException ex)
        {
            _logger.LogWarning(ex, "Upstream provider unavailable: {Message}", ex.Message);
            await WriteProblem(context, StatusCodes.Status503ServiceUnavailable, "IP provider unavailable");
        }
        catch (IPServiceException ex)
        {
            _logger.LogWarning(ex, "Upstream service returned invalid data: {Message}", ex.Message);
            await WriteProblem(context, StatusCodes.Status502BadGateway, "Invalid response from IP provider");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error");
            await WriteProblem(context, StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    private static async Task WriteProblem(HttpContext context, int statusCode, string title, string? detail = null)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path,
        };

        await context.Response.WriteAsJsonAsync(problem);
    }
}
