using IpLookupService.Services;

namespace IpLookupService;

internal static class Endpoints
{
    internal static void UseIpAddressEndpoints(this WebApplication app)
    {
        app.MapGet("/ip/{ipAddress}", GetIpAddressDetails());
    }

    private static Func<string, ILogger<WebApplication>, IIpAddressService, Task<IResult>> GetIpAddressDetails()
    {
        return async (ipAddress, logger, ipAddressService) =>
        {
            var isIpValid = IpValidator.IsValidIp(ipAddress);
            if (isIpValid)
            {
                var result = await ipAddressService.GetIpDetailsAsync(ipAddress);
                return Results.Ok(result);
            }

            return Results.BadRequest(new
            {
                error = "Invalid IP address",
                code = 400
            });
        };
    }
}