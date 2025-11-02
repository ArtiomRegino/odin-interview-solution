using BatchService.Contracts;
using BatchService.Models;

namespace BatchService.Services;

public class BatchStore : IBatchStore
{
    public BatchStatusDto? GetBatchStatus()
    {
        throw new NotImplementedException();
    }
}