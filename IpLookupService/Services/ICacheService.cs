using Common.Models;

namespace IpLookupService.Services;

public interface ICacheService
{
    Task<IPDetailsDto?> GetCachedIpDetails(string ipAddress);
}