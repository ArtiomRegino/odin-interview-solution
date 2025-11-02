using BatchService.Contracts;
using BatchService.Models;
using Common.Validation;
using Microsoft.AspNetCore.Mvc;

namespace BatchService.Controllers;

[ApiController]
[Route("[controller]")]
public class BatchController : ControllerBase
{
    private readonly ILogger<BatchController> _logger;
    private readonly IBatchStore _batchStore;
    private readonly IBatchScheduler _batchScheduler;
    
    public BatchController(IBatchStore batchStore, ILogger<BatchController> logger)
    {
        _batchStore = batchStore;
        _logger = logger;
    }

    [HttpPost]
    public ActionResult<Guid> PostIpAddresses([FromBody] string[] ipAddresses)
    {
        if (ipAddresses.Length == 0 || ipAddresses == null)
        {
            throw new ArgumentException("At least one IP address must be provided.", nameof(ipAddresses));
        }

        var invalidIPAddresses = ipAddresses.Where(ipAddress => ipAddress.IsValidIp()).ToArray();
        if (invalidIPAddresses.Length == ipAddresses.Length)
        {
            throw new ArgumentException("No valid IP addresses were provided.", nameof(ipAddresses));
        }
        
        var batchId = _batchScheduler.Schedule(ipAddresses);
        
        return batchId;
    }

    [HttpGet("/{batchId}")]
    public ActionResult<BatchStatusDto> GetBatchStatus([FromRoute] Guid batchId)
    {
        var batchStatus = _batchStore.GetBatch(batchId);
        if (batchStatus is null)
        {
            _logger.LogInformation($"Batch with id {batchId} not found.");
            return NotFound(CreateProblem(StatusCodes.Status404NotFound, "Batch not found",
                "No batch with the specified ID exists or it has already expired."));
        }
        
        return batchStatus;
    }
    
    private static ProblemDetails CreateProblem(int statusCode, string title, string detail)
    {
        return new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail
        };
    }
}