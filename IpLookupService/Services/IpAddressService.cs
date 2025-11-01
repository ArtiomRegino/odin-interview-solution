using IpLookupService.Models;

namespace IpLookupService.Services;

public class IpAddressService: IIpAddressService
{
    public IpAddressService()
    {
        
    }

    public Task<IpAddressDetails> GetIpDetailsAsync(string ipAddress)
    {
        return Task.FromResult<IpAddressDetails>(default(IpAddressDetails));
    }
}