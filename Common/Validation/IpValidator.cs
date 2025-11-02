using System.Net;

namespace Common.Validation;

public static class IpValidator
{
    public static void ValidateIPAddressOrThrow(string ipAddress)
    {
        var isNotValidIp = IsNotValidIp(ipAddress);
        if (isNotValidIp)
        {
            throw new ArgumentException("Invalid IP address format", nameof(ipAddress));
        }
    }
 
    private static bool IsNotValidIp(string ip)
    {
        return !IsValidIp(ip);
    }
    
    private static bool IsValidIp(string ip)
    {
        return IPAddress.TryParse(ip, out _);
    }
}