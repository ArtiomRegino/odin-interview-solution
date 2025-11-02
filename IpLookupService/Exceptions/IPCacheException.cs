namespace IpLookupService.Exceptions;

public class IPCacheException : Exception
{
    public int? Code { get; set; }
    public string? Info { get; set; }
    
    
    public IPCacheException() : base()
    {
    }
    
    public IPCacheException(string message) : base(message)
    {
    }
    
    public IPCacheException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public IPCacheException(int? code, string info) : base($"IP provider error {code}: {info}")
    {
        Code = code;
        Info = info;
    }
}