namespace BatchService.Exceptions;

public class IPLookupNotAvailableException: IPLookupException
{
    public IPLookupNotAvailableException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public IPLookupNotAvailableException(int code) : base($"IP provider returned status code {code}.")
    {
        Code = code;
    }
    
    public IPLookupNotAvailableException(int code, string info) : base(code, info)
    {
    }
}