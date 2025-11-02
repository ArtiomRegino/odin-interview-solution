using System.Collections.Concurrent;
using BatchService.Contracts;
using BatchService.Models;
using Common.Models;

namespace BatchService.Services;

public class BatchStore : IBatchStore
{
    private readonly ConcurrentDictionary<Guid, BatchStatusDto> _batchStatuses = new();

    public void CreateBatch(BatchStatusDto batchStatus)
    {
        if (!_batchStatuses.TryAdd(batchStatus.BatchId, batchStatus))
        {
            throw new InvalidOperationException($"Batch with id {batchStatus.BatchId} already exists.");
        }
    }
    
    public BatchStatusDto? GetBatch(Guid batchId)
    {
        _batchStatuses.TryGetValue(batchId, out var batchStatus);
        return batchStatus;
    }

    public void MarkBatchRunning(Guid batchId)
    {
        if (_batchStatuses.TryGetValue(batchId, out var batch))
        {
            batch.Status = BatchStatus.Running;
        }
    }

    public void MarkBatchCompleted(Guid batchId)
    {
        if (_batchStatuses.TryGetValue(batchId, out var batch))
        {
            batch.Status = BatchStatus.Completed;
        }
    }

    public void MarkIpSuccess(Guid batchId, string ip, IPDetailsDto details)
    {
        if (!_batchStatuses.TryGetValue(batchId, out var batch))
            return;

        if (!batch.Items.TryGetValue(ip, out var item))
            return;

        item.Status = IpWorkItemStatus.Success;
        item.Details = details;
        item.Error = null;
    }

    public void MarkIpError(Guid batchId, string ip, string errorMessage)
    {
        if (!_batchStatuses.TryGetValue(batchId, out var batch))
            return;

        if (!batch.Items.TryGetValue(ip, out var item))
            return;

        item.Status = IpWorkItemStatus.Error;
        item.Error = errorMessage;
        item.Details = null;
    }
}