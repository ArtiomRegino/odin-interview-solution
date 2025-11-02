using Common.Models;

namespace IpLookupService.Contracts;

public interface IExternalIPService
{
    Task<IPDetailsDto> GetIpDetailsAsync(string ipAddress, CancellationToken ct = default);
}