namespace IpLookupService.Exceptions;

public class IPServiceException : Exception
{
    public int Code { get; set; }
    public string? Type { get; set; }
    public string? Info { get; set; }
    
    
    public IPServiceException() : base()
    {
    }
    
    public IPServiceException(string message) : base(message)
    {
    }
    
    public IPServiceException(int code) : base($"IP provider returned status code {code}.")
    {
        Code = code;
    }
    
    public IPServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public IPServiceException(int code, string type, string info) : base($"IP provider error {code} ({type}): {info}")
    {
        Code = code;
        Type = type;
        Info = info;
    }
}