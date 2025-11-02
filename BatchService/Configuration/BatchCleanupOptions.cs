namespace BatchService.Configuration;

public class BatchCleanupOptions
{
    public TimeSpan CompletedTtl { get; set; } = TimeSpan.FromMinutes(5);
    public TimeSpan HardTtl { get; set; } = TimeSpan.FromMinutes(30);
    public TimeSpan SweepInterval { get; set; } = TimeSpan.FromMinutes(1);
}