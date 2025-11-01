using Common.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CacheService.CacheService;

public class MemoryCacheStore : ICacheStore
{
    private readonly IMemoryCache _memoryCache;
    private readonly TimeSpan _ttl;
    
    public MemoryCacheStore(IMemoryCache memoryCache, IConfiguration config)
    {
        _memoryCache = memoryCache;
        var ttlMinutes = config.GetValue("Cache:TtlMinutes", 1);
        _ttl = TimeSpan.FromMinutes(ttlMinutes);
    }
    
    public bool TryGet(string ip, out IPDetailsDto? details)
    {
        return _memoryCache.TryGetValue(ip, out details);
    }

    public void Set(string ip, IPDetailsDto details)
    {
        _memoryCache.Set(ip, details, _ttl);
    }
}