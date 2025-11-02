using BatchService.Configuration;
using BatchService.Contracts;
using BatchService.Models;

namespace BatchService.Jobs;

public class BatchCleanupWorker : BackgroundService
{
    private readonly IBatchStore _store;
    private readonly ILogger<BatchCleanupWorker> _logger;
    private readonly BatchCleanupOptions _options;

    public BatchCleanupWorker(IBatchStore store, ILogger<BatchCleanupWorker> logger)
    {
        _store = store;
        _logger = logger;
        _options = new BatchCleanupOptions();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("BatchCleanupWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                SweepOnce();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during batch cleanup sweep");
            }

            try
            {
                await Task.Delay(_options.SweepInterval, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation("BatchCleanupWorker stopped");
    }

    private void SweepOnce()
    {
        var now = DateTime.UtcNow;

        foreach (var batch in _store.GetAllBatches())
        {
            var ageSinceCreate = now - batch.CreatedAtUtc;
            
            if (batch.Status == BatchStatus.Completed && batch.CompletedAtUtc is DateTime completedAt &&
                now - completedAt > _options.CompletedTtl)
            {
                _store.RemoveBatch(batch.BatchId);
                continue;
            }
            
            if (ageSinceCreate > _options.HardTtl)
            {
                _store.RemoveBatch(batch.BatchId);
            }
        }
    }
}