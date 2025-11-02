using Common.Models;

namespace IpLookupService.Contracts;

public interface IIPCacheService
{
    Task<IPDetailsDto?> GetCachedIpDetails(string ipAddress, CancellationToken ct);
    Task SaveIpDetails(string ipAddress, IPDetailsDto ipDetails, CancellationToken ct);
}