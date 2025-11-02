using BatchService.Contracts;
using BatchService.Exceptions;
using Common.Models;
using Common.Validation;
using Microsoft.AspNetCore.Mvc;

namespace BatchService.Services;

public class IPLookupService : IIPLookupService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<IPLookupService> _logger;

    public IPLookupService(HttpClient httpClient, ILogger<IPLookupService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<IPDetailsDto> GetIPDetails(string ipAddress, CancellationToken ct = default)
    {
        IpValidator.ValidateIPAddressOrThrow(ipAddress);

        var response = await SendIPDetailsRequest(ipAddress, ct);

        await HandleNonSuccessResponse(response, ipAddress, ct);
        var ipDetails = await ParseSuccessResponse(response, ipAddress, ct);

        return ipDetails;
    }

    private async Task<HttpResponseMessage> SendIPDetailsRequest(string ipAddress, CancellationToken ct)
    {
        HttpResponseMessage response;
        try
        {
            response = await _httpClient.GetAsync($"/ip/{ipAddress}", ct);
        }
        catch (HttpRequestException e)
        {
            throw new IPLookupNotAvailableException("Network error in IP Lookup service.", e);
        }
        catch (Exception e)
        {
            throw new IPLookupException("Error in IP Lookup service.", e);
        }

        return response;
    }
    
    private async Task HandleNonSuccessResponse(HttpResponseMessage response, string ipAddress, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }
        
        ProblemDetails? problemDetails;
        try
        {
            problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse IP Lookup error body for IP {Ip}.", ipAddress);
            throw new IPLookupException($"Failed to parse IP Lookup error body for IP {ipAddress}.");
        }
        
        _logger.LogWarning("IP Lookup error {Code} for {Ip}: {Title}", problemDetails?.Status, ipAddress, problemDetails?.Title);
        throw new IPLookupException(problemDetails?.Status, problemDetails?.Title);
    }

    private async Task<IPDetailsDto> ParseSuccessResponse(HttpResponseMessage response, string ipAddress, CancellationToken ct)
    {
        IPDetailsDto? ipDetails;
        try
        {
            ipDetails = await response.Content.ReadFromJsonAsync<IPDetailsDto>(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse IP Lookup response body for IP {Ip}.", ipAddress);
            throw new IPLookupException($"Failed to parse IP Lookup response body for IP {ipAddress}.", ex);
        }
        
        if (ipDetails == null)
        {
            _logger.LogError("IP Lookup response body was null for IP {Ip}", ipAddress);
            throw new IPLookupException("Empty response from IP Lookup.");
        }

        return ipDetails;
    }
}