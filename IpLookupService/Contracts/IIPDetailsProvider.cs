using Common.Models;

namespace IpLookupService.Contracts;

public interface IIPDetailsProvider
{
    Task<IPDetailsDto> GetIpDetails(string ipAddress, CancellationToken ct = default);
}