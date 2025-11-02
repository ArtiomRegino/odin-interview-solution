using BatchService.Contracts;
using Common.Models;

namespace BatchService.Services;

public class IPLookupService : IIPLookupService
{
    public Task<IPDetailsDto> GetIPDetails(string ipAddress)
    {
        return Task.FromResult<IPDetailsDto>(null);
    }
}