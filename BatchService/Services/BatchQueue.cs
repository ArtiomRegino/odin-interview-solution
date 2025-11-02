using System.Threading.Channels;
using BatchService.Contracts;
using BatchService.Models;

namespace BatchService.Services;

public class BatchQueue : IBatchQueue
{
    private readonly Channel<BatchJob> _jobs;

    public BatchQueue(Channel<BatchJob> jobs)
    {
        _jobs = jobs;
    }
    
    public void Enqueue(BatchJob job)
    {
        if (!_jobs.Writer.TryWrite(job))
        {
            throw new InvalidOperationException("Failed to enqueue batch job.");
        }
    }

    public async IAsyncEnumerable<BatchJob> DequeueAsync(CancellationToken ct)
    {
        while (await _jobs.Reader.WaitToReadAsync(ct))
        {
            while (_jobs.Reader.TryRead(out var job))
            {
                yield return job;
            }
        }
    }
}