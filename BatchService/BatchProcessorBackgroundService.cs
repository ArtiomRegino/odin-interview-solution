using BatchService.Contracts;

namespace BatchService;

public class BatchProcessorBackgroundService: BackgroundService
{
    private readonly IIPLookupService _ipLookupService;
    private readonly IBatchStore _batchStore;
    private readonly ILogger<BatchProcessorBackgroundService> _logger;

    public BatchProcessorBackgroundService(IIPLookupService ipLookupService, IBatchStore batchStore,
        ILogger<BatchProcessorBackgroundService> logger)
    {
        _ipLookupService = ipLookupService;
        _batchStore = batchStore;
        _logger = logger;
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {

            var ipDetails = _ipLookupService.GetIPDetails("");
        }
    }
}