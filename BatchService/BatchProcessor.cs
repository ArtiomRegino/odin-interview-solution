using BatchService.Contracts;
using BatchService.Models;

namespace BatchService;

public class BatchProcessor: BackgroundService
{
    private const int ChunkSize = 10;
    
    private readonly IIPLookupService _ipLookupService;
    private readonly IBatchStore _batchStore;
    private readonly IBatchQueue _batchQueue;
    private readonly ILogger<BatchProcessor> _logger;

    public BatchProcessor(IIPLookupService ipLookupService, IBatchStore batchStore, IBatchQueue batchQueue,
        ILogger<BatchProcessor> logger)
    {
        _ipLookupService = ipLookupService;
        _batchStore = batchStore;
        _batchQueue = batchQueue;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _logger.LogInformation("BatchProcessor started");
        
        await foreach (var job in _batchQueue.DequeueAsync(ct))
        {
            await ProcessJob(job, ct);
        }
        
        _logger.LogInformation("BatchProcessor finished");
    }

    private async Task ProcessJob(BatchJob job, CancellationToken ct)
    {
        var batch = _batchStore.GetBatch(job.BatchId);
        if (batch == null)
        {
            _logger.LogWarning("Batch {BatchId} not found", job.BatchId);
            return;
        }
        
        _batchStore.MarkBatchRunning(job.BatchId);
        
        var ipChunks = batch.Items.Chunk(ChunkSize);
        foreach (var chunk in ipChunks)
        {
            await ProcessChunk(job.BatchId, chunk, ct);
        }

        _batchStore.MarkBatchCompleted(job.BatchId);
    }

    private async Task ProcessChunk(Guid batchId, IpWorkItemDto[] chunk, CancellationToken ct)
    {
        foreach (var ipItem in chunk)
        {
            try
            {
                var ipDetails = await _ipLookupService.GetIPDetails(ipItem.Ip, ct);
            
                _batchStore.MarkIpSuccess(batchId, ipItem.Ip, ipDetails);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Failed to resolve IP {Ip} for batch {BatchId}", ipItem.Ip, batchId);
                _batchStore.MarkIpError(batchId,  ipItem.Ip, e.Message);
            }
        }
    }
}