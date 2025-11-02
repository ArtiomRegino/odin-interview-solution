using BatchService.Models;

namespace BatchService.Contracts;

public interface IBatchStore
{
    BatchStatusDto? GetBatchStatus();
}