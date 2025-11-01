using System.Net;

namespace Common.Extensions;

public static class IpValidator
{
    public static bool IsValidIp(string ip)
    {
        return IPAddress.TryParse(ip, out _);
    }
    
    public static bool IsNotValidIp(string ip)
    {
        return !IsValidIp(ip);
    }
}