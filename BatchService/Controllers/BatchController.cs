using BatchService.Contracts;
using BatchService.Models;
using Common.Validation;
using Microsoft.AspNetCore.Mvc;

namespace BatchService.Controllers;

[ApiController]
[Route("batch")]
public class BatchController : ControllerBase
{
    private readonly ILogger<BatchController> _logger;
    private readonly IBatchStore _batchStore;
    private readonly IBatchScheduler _batchScheduler;
    
    public BatchController(IBatchStore batchStore, IBatchScheduler batchScheduler, ILogger<BatchController> logger)
    {
        _batchScheduler = batchScheduler;
        _batchStore = batchStore;
        _logger = logger;
    }

    /// <summary>
    /// Submit a list of IP addresses for asynchronous batch processing.
    /// </summary>
    /// <remarks>
    /// This endpoint accepts an array of IP addresses and schedules them for background processing.
    /// The service:
    /// 1. Creates a new batch with status "Pending".
    /// 2. Assigns a unique batchId (GUID).
    /// 3. Returns the batchId immediately with HTTP 202 Accepted.
    ///
    /// Processing itself is done asynchronously by a background worker:
    /// - IPs are processed in chunks of up to 10 at a time.
    /// - For each IP, the Batch Service calls the IP Lookup Service.
    /// - The Lookup Service is responsible for cache-first lookup and cache warming.
    ///
    /// You can query the current status of the batch using GET /batch/{batchId}.
    ///
    /// Notes:
    /// - Invalid IPs are still included in the batch, but they will end up with state = "Error".
    /// - If *all* IPs are invalid, the request is rejected with HTTP 400.
    /// </remarks>
    /// <param name="ipAddresses">Array of IP addresses to resolve (IPv4 or IPv6).</param>
    /// <response code="202">Batch accepted. Returns the batchId to poll.</response>
    /// <response code="400">No valid IP addresses were provided.</response>
    /// <response code="500">Unhandled server error.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public ActionResult<Guid> PostIpAddresses([FromBody] string[] ipAddresses)
    {
        if (ipAddresses.Length == 0 || ipAddresses == null)
        {
            throw new ArgumentException("At least one IP address must be provided.", nameof(ipAddresses));
        }

        var invalidIPAddresses = ipAddresses.Where(ipAddress => ipAddress.IsNotValidIp()).ToArray();
        if (invalidIPAddresses.Length == ipAddresses.Length)
        {
            throw new ArgumentException("No valid IP addresses were provided.", nameof(ipAddresses));
        }
        
        var batchId = _batchScheduler.Schedule(ipAddresses);
        
        return batchId;
    }

    /// <summary>
    /// Get the processing status of a previously submitted batch.
    /// </summary>
    /// <remarks>
    /// This endpoint returns:
    /// - overall batch status:
    ///   - Pending  = scheduled, not yet processed
    ///   - Running  = currently being processed by the background worker
    ///   - Completed = fully processed
    ///
    /// - per-IP results:
    ///   Each IP has:
    ///     - state: "Pending" | "Success" | "Error"
    ///     - details: populated if successfully resolved (geolocation etc.)
    ///     - error: populated if resolution failed
    ///
    /// The response model includes a dictionary of items keyed by IP address.
    /// </remarks>
    /// <param name="batchId">The batch identifier returned by POST /batch.</param>
    /// <response code="200">Returns current batch status and per-IP results.</response>
    /// <response code="404">Batch not found (invalid ID or already expired).</response>
    /// <response code="500">Unhandled server error.</response>
    [HttpGet("{batchId:guid}")]
    [ProducesResponseType(typeof(BatchStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
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