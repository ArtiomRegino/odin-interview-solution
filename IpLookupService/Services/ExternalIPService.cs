using Common.Models;
using Common.Validation;
using IpLookupService.Configuration;
using IpLookupService.Contracts;
using IpLookupService.Exceptions;
using IpLookupService.Extensions;
using IpLookupService.Models.IpStack;
using Microsoft.Extensions.Options;

namespace IpLookupService.Services;

public class ExternalIPService: IExternalIPService
{
    private readonly HttpClient _httpClient;
    private readonly IPStackSettings _settings;
    private readonly ILogger<ExternalIPService> _logger;

    public ExternalIPService(HttpClient httpClient, IOptions<IPStackSettings> settings, ILogger<ExternalIPService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<IPDetailsDto> GetIpDetailsAsync(string ipAddress, CancellationToken ct = default)
    {
        IpValidator.ValidateIPAddressOrThrow(ipAddress);
        var response = await SendIPDetailsRequest(ipAddress, ct);

        await HandleNonSuccessResponse(response, ipAddress, ct);
        var ipStackResponse = await ParseSuccessResponse(response, ipAddress, ct);
        
        return ipStackResponse.MapToDto();
    }

    private async Task<HttpResponseMessage> SendIPDetailsRequest(string ipAddress, CancellationToken ct)
    {
        HttpResponseMessage response;
        try
        {
            var url = $"{ipAddress}?access_key={_settings.ApiKey}";
            response = await _httpClient.GetAsync($"/{url}", ct);
        }
        catch (HttpRequestException e)
        {
            throw new IPServiceNotAvailableException("Network error in IP provider.", e);
        }

        return response;
    }

    private async Task HandleNonSuccessResponse(HttpResponseMessage response, string ipAddress, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }
        
        IPStackError? error = null;
        try
        {
            error = (await response.Content.ReadFromJsonAsync<IPStackErrorResponse>(ct))?.Error;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse IP provider error body for IP {Ip}.", ipAddress);
        }
        
        if (error is not null)
        {
            _logger.LogWarning("IP provider error {Code} ({Type}) for {Ip}: {Info}", error.Code, error.Type, ipAddress, error.Info);
            throw new IPServiceNotAvailableException(error.Code, error.Type, error.Info);
        }
        
        _logger.LogWarning("IP provider returned non-success status code {Status} for IP {Ip}", response.StatusCode, ipAddress);
        throw new IPServiceNotAvailableException((int)response.StatusCode);
    }

    private async Task<IpStackResponse> ParseSuccessResponse(HttpResponseMessage response, string ipAddress,
        CancellationToken ct)
    {
        IpStackResponse? ipStackResponse;
        try
        {
            ipStackResponse = await response.Content.ReadFromJsonAsync<IpStackResponse>(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse IP provider response body for IP {Ip}.", ipAddress);
            throw new IPServiceException($"Failed to parse IP provider response body for IP {ipAddress}.", ex);
        }
        
        if (ipStackResponse == null)
        {
            _logger.LogError("IP provider response body was null for IP {Ip}", ipAddress);
            throw new IPServiceException("Empty response from IP provider.");
        }

        return ipStackResponse;
    }
}