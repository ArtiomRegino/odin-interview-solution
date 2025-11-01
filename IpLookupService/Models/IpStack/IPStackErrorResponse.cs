namespace IpLookupService.Models.IpStack;

public class IPStackErrorResponse
{
    public bool? Success { get; set; }
    public IPStackError? Error { get; set; }
}

public class IPStackError
{
    public int Code { get; set; }
    public string? Type { get; set; }
    public string? Info { get; set; }
}