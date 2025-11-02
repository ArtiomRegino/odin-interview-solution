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
 
    public static bool IsNotValidIp(this string ip)
    {
        return !IsValidIp(ip);
    }
    
    public static bool IsValidIp(this string ip)
    {
        return IPAddress.TryParse(ip, out _);
    }
}