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
            try
            {
                await ProcessJob(job.BatchId, ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                _logger.LogInformation("BatchProcessor stopped due to cancellation.");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error while processing batch {BatchId}", job.BatchId);
            }
        }
        
        _logger.LogInformation("BatchProcessor finished");
    }

    private async Task ProcessJob(Guid batchId, CancellationToken ct)
    {
        var batch = _batchStore.GetBatch(batchId);
        if (batch == null)
        {
            _logger.LogWarning("Batch {BatchId} not found", batchId);
            return;
        }

        if (batch.Status is BatchStatus.Completed or BatchStatus.Running )
        {
            _logger.LogWarning("Batch {BatchId} is already taken by another BatchProcessor instance.", batchId);
            return;
        }
        
        _batchStore.MarkBatchRunning(batchId);
        
        var ipChunks = batch.Items.Values.Chunk(ChunkSize);
        foreach (var chunk in ipChunks)
        {
            await ProcessChunk(batchId, chunk, ct);
        }

        _batchStore.MarkBatchCompleted(batchId);
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