using Common.Models;
using Common.Validation;
using IpLookupService.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace IpLookupService;

internal static class Endpoints
{
    internal static void UseIpAddressEndpoints(this WebApplication app)
    {
        app.MapGet("/ip/{ipAddress}", GetIpAddressDetails())
            .WithName("GetIpDetails")
            .WithSummary("Retrieve IP address details")
            .WithDescription("""
                             Retrieves geographical, network, and connection details for a given IP address
                             using the external IPStack API.
                             """)
            .Produces<IPDetailsDto>()
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status502BadGateway)
            .Produces<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
            .WithOpenApi();
    }

    private static Func<string, IIPDetailsProvider, Task<IResult>> GetIpAddressDetails()
    {
        return async (ipAddress, ipDetailsProvider) =>
        {
            IpValidator.ValidateIPAddressOrThrow(ipAddress);
            var result = await ipDetailsProvider.GetIpDetails(ipAddress);
            
            return Results.Ok(result);
        };
    }
}