using System.Net;

namespace IpLookupService;

public class IpValidator
{
    public static bool IsValidIp(string ip)
    {
        return IPAddress.TryParse(ip, out _);
    }
}