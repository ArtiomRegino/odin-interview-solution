using CacheService.CacheService;
using Common.Models;
using Common.Validation;
using Microsoft.AspNetCore.Mvc;

namespace CacheService;

internal static class Endpoints
{
    internal static void UseCacheEndpoints(this WebApplication app)
    {
        app.MapGet("/cache/{ipAddress}", GetCachedIpDetails())
            .WithName("GetCachedIpDetails")
            .WithSummary("Get cached IP details")
            .WithDescription("""
                             Returns cached details for the given IP address if present and not expired.
                             TTL is 1 minute per IP entry.
                             """)
            .Produces<IPDetailsDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
            .WithOpenApi();

        app.MapPut("/cache/{ipAddress}", PutCachedIpDetails())
            .WithName("PutCachedIpDetails")
            .WithSummary("Insert or update cached IP details")
            .WithDescription("""
                             Stores or refreshes cached IP details for the given IP address.
                             Cache entries expire automatically 1 minute after insertion.
                             """)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
            .WithOpenApi();

    }

    private static Func<string, ICacheStore, IResult> GetCachedIpDetails()
    {
        return ([FromRoute] ipAddress, cache) =>
        {
            IpValidator.ValidateIPAddressOrThrow(ipAddress);
            if (!cache.TryGet(ipAddress, out var ipAddressDetails) || ipAddressDetails is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(ipAddressDetails);
        };
    }

    private static Func<string, IPDetailsDto, ICacheStore, IResult> PutCachedIpDetails()
    {
        return ([FromRoute] ipAddress, [FromBody] ipDetails, cache) =>
        {
            IpValidator.ValidateIPAddressOrThrow(ipAddress);
            if (ipDetails is null)
            {
                throw new ArgumentException("Request body is required.");
            }

            if (!string.Equals(ipAddress, ipDetails.Ip, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("IP in route does not match IP in body.");
            }

            cache.Set(ipAddress, ipDetails);
            return Results.NoContent();
        };
    }
}