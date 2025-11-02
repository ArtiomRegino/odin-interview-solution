using BatchService.Contracts;
using BatchService.Models;

namespace BatchService.Services;

public class BatchScheduler : IBatchScheduler
{
    private readonly IBatchStore _batchStore;
    private readonly IBatchQueue _batchQueue;
    private readonly ILogger<BatchScheduler> _logger;

    public BatchScheduler(IBatchStore batchStore, IBatchQueue batchQueue, ILogger<BatchScheduler> logger)
    {
        _batchStore = batchStore;
        _batchQueue = batchQueue;
        _logger = logger;
    }
    
    public Guid Schedule(string[] ipAddresses)
    {
        if (ipAddresses == null || ipAddresses.Length == 0)
        {
            throw new ArgumentException("No IPs provided", nameof(ipAddresses));
        }
        
        var uniqueIpAddresses = ipAddresses.Distinct();
        var batchId = Guid.NewGuid();
        var batch = new BatchStatusDto
        {
            Status = BatchStatus.Pending,
            BatchId = batchId,
            Items = uniqueIpAddresses.Select(ip => new IpWorkItemDto
            {
                Status = IpWorkItemStatus.Pending,
                Ip = ip
            }).ToDictionary(x => x.Ip)
        };

        _batchStore.CreateBatch(batch);

        var batchJob = new BatchJob(batchId);
        _batchQueue.Enqueue(batchJob);
        _logger.LogInformation("Scheduled batch {BatchId} with {Count} IPs", batchId, ipAddresses.Length);
        
        return batchId;
    }
}