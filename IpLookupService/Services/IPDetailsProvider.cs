using Common.Models;
using Common.Validation;
using IpLookupService.Contracts;
using IpLookupService.Exceptions;

namespace IpLookupService.Services;

public class IPDetailsProvider : IIPDetailsProvider
{
    private readonly IExternalIPService _externalIPService;
    private readonly ILogger<IPDetailsProvider> _logger;
    private readonly IIPCacheService _ipCacheService;

    public IPDetailsProvider(IIPCacheService ipCacheService, IExternalIPService externalIPService, ILogger<IPDetailsProvider> logger)
    {
        _ipCacheService = ipCacheService;
        _externalIPService = externalIPService;
        _logger = logger;
    }

    public async Task<IPDetailsDto> GetIpDetails(string ipAddress, CancellationToken cancellationToken = default)
    {
        IpValidator.ValidateIPAddressOrThrow(ipAddress);
        
        try
        {
            var cachedIpDetails = await _ipCacheService.GetCachedIpDetails(ipAddress, cancellationToken);
            if (cachedIpDetails != null)
            {
                return cachedIpDetails;
            }
        }
        catch (IPCacheException ex)
        {
            _logger.LogWarning(ex, "Error getting details for {ipAddress} from cache. Continuing without cache.", ipAddress);
        }
        
        var freshIpDetails = await _externalIPService.GetIpDetailsAsync(ipAddress, cancellationToken);
        
        try
        {
            await _ipCacheService.SaveIpDetails(ipAddress, freshIpDetails, cancellationToken);
        }
        catch (IPCacheException ex)
        {
            _logger.LogWarning(ex, "Cache service error while writing IP {Ip}. Skipping cache update.", ipAddress);
        }
        
        return freshIpDetails;
    }
}