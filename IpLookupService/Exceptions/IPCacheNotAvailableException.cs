namespace IpLookupService.Exceptions;

public class IPCacheNotAvailableException: IPCacheException
{
    public IPCacheNotAvailableException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public IPCacheNotAvailableException(int code) : base($"IP provider returned status code {code}.")
    {
        Code = code;
    }
    
    public IPCacheNotAvailableException(int code, string info) : base(code, info)
    {
    }
}