using Common.Models;

namespace IpLookupService.Services;

public class CacheService : ICacheService
{
    public CacheService()
    {
        
    }
    
    public async Task<IPDetailsDto?> GetCachedIpDetails(string ipAddress)
    {

        return await Task.FromResult<IPDetailsDto?>(default);
    }
}