using Common.Models;

namespace CacheService.CacheService;

public interface ICacheStore
{
    bool TryGet(string ip, out IPDetailsDto? details);
    void Set(string ip, IPDetailsDto details);
}