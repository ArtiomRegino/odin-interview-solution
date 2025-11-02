namespace BatchService.Models;

public class BatchStatusDto
{
    public Guid BatchId { get; init; }
    public BatchStatus Status { get; set; }
    public Dictionary<string, IpWorkItemDto> Items { get; init; } = [];
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;
    
    public DateTime? CompletedAtUtc { get; set; }
}

public enum BatchStatus
{
    Pending,
    Running,
    Completed,
}