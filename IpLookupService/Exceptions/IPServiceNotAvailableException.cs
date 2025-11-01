namespace IpLookupService.Exceptions;

public class IPServiceNotAvailableException: IPServiceException
{
    public IPServiceNotAvailableException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public IPServiceNotAvailableException(int code) : base($"IP provider returned status code {code}.")
    {
        Code = code;
    }
    
    public IPServiceNotAvailableException(int code, string type, string info) : base(code, type, info)
    {
    }
}