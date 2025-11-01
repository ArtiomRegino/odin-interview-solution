using IpLookupService.Models;

namespace IpLookupService.Services;

public interface IIpAddressService
{
    Task<IpAddressDetails> GetIpDetailsAsync(string ipAddress);
}