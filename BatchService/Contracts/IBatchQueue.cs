using BatchService.Models;

namespace BatchService.Contracts;

public interface IBatchQueue
{
    void Enqueue(BatchJob job);
    IAsyncEnumerable<BatchJob> DequeueAsync(CancellationToken ct);
}