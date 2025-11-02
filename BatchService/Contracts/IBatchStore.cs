using BatchService.Models;
using Common.Models;

namespace BatchService.Contracts;

public interface IBatchStore
{
    void CreateBatch(BatchStatusDto batchStatus);
    BatchStatusDto? GetBatch(Guid batchId);
    void MarkBatchRunning(Guid batchId);
    void MarkBatchCompleted(Guid batchId);
    void MarkIpSuccess(Guid batchId, string ip, IPDetailsDto details);
    void MarkIpError(Guid batchId, string ip, string errorMessage);
}