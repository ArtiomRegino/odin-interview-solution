using Common.Models;

namespace BatchService.Models;

public class IpWorkItemDto
{
    public string Ip { get; init; } = null!;
    public IpWorkItemStatus Status { get; set; }
    public IPDetailsDto? Details { get; set; }
    public string? Error { get; set; }
}

public enum IpWorkItemStatus
{
    Pending,
    Success,
    Error,
}