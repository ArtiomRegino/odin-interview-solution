using IpLookupService.Models;

namespace IpLookupService.Services;

public interface IExternalIPService
{
    Task<IPDetailsDto> GetIpDetailsAsync(string ipAddress, CancellationToken ct = default);
}