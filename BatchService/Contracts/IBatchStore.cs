using BatchService.Models;
using Common.Models;

namespace BatchService.Contracts;

public interface IBatchStore
{
    void CreateBatch(BatchStatusDto batchStatus);
    void RemoveBatch(Guid batchId);
    BatchStatusDto? GetBatch(Guid batchId);
    IEnumerable<BatchStatusDto> GetAllBatches();
    void MarkBatchRunning(Guid batchId);
    void MarkBatchCompleted(Guid batchId);
    void MarkIpSuccess(Guid batchId, string ip, IPDetailsDto details);
    void MarkIpError(Guid batchId, string ip, string errorMessage);
}