namespace BatchService.Exceptions;

public class IPLookupException : Exception
{
    public int? Code { get; set; }
    public string? Info { get; set; }
    
    
    public IPLookupException() : base()
    {
    }
    
    public IPLookupException(string message) : base(message)
    {
    }
    
    public IPLookupException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public IPLookupException(int? code, string info) : base($"IP provider error {code}: {info}")
    {
        Code = code;
        Info = info;
    }
}