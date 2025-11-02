using Common.Models;

namespace BatchService.Contracts;

public interface IIPLookupService
{
    Task<IPDetailsDto> GetIPDetails(string ipAddress, CancellationToken ct = default);
}