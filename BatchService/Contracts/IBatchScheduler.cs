namespace BatchService.Contracts;

public interface IBatchScheduler
{
    Guid Schedule(string[] ipAddresses);
}