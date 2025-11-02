using System.Net;
using Common.Models;
using Common.Validation;
using IpLookupService.Contracts;
using IpLookupService.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace IpLookupService.Services;

public class IPCacheService : IIPCacheService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<IPCacheService> _logger;

    public IPCacheService(HttpClient httpClient, ILogger<IPCacheService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<IPDetailsDto?> GetCachedIpDetails(string ipAddress, CancellationToken ct)
    {
        IpValidator.ValidateIPAddressOrThrow(ipAddress);

        var response = await GetCachedIpDetailsRequest(ipAddress, ct);

        if (response.IsSuccessStatusCode)
        {
            return await ParseSuccessResponse(response, ipAddress, ct);
        }
        
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogDebug("Cache MISS for {Ip}", ipAddress);
            return null;
        }
        
        await HandleNonSuccessResponse(response, ipAddress, ct);
        return null;
    }

    public async Task SaveIpDetails(string ipAddress, IPDetailsDto ipDetails, CancellationToken ct)
    {
        IpValidator.ValidateIPAddressOrThrow(ipAddress);

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.PutAsJsonAsync($"/cache/{ipAddress}", ipDetails, ct);
        }
        catch (HttpRequestException e)
        {
            throw new IPCacheNotAvailableException("Network error in IP cache provider.", e);
        }
        catch (Exception e)
        {
            throw new IPCacheException("Error in IP cache provider.", e);
        }
        
        await HandleNonSuccessResponse(response, ipAddress, ct);
    }
    
    private async Task<HttpResponseMessage> GetCachedIpDetailsRequest(string ipAddress, CancellationToken ct)
    {
        HttpResponseMessage response;
        try
        {
            response = await _httpClient.GetAsync($"/cache/{ipAddress}", ct);
        }
        catch (HttpRequestException e)
        {
            throw new IPCacheNotAvailableException("Network error in IP cache provider.", e);
        }
        catch (Exception e)
        {
            throw new IPCacheException("Error in IP cache provider.", e);
        }

        return response;
    }

    private async Task HandleNonSuccessResponse(HttpResponseMessage response, string ipAddress, CancellationToken ct)
    {
        ProblemDetails? problemDetails;
        try
        {
            problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse IP Cache provider error body for IP {Ip}.", ipAddress);
            throw new IPCacheException($"Failed to parse IP Cache provider error body for IP {ipAddress}.");
        }
        
        _logger.LogWarning("IP Cache provider error {Code} for {Ip}: {Title}", problemDetails?.Status, ipAddress, problemDetails?.Title);
        throw new IPCacheException(problemDetails?.Status, problemDetails?.Title);
    }

    private async Task<IPDetailsDto> ParseSuccessResponse(HttpResponseMessage response, string ipAddress,
        CancellationToken ct)
    {
        IPDetailsDto? ipDetails;
        try
        {
            ipDetails = await response.Content.ReadFromJsonAsync<IPDetailsDto>(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse IP Cache provider response body for IP {Ip}.", ipAddress);
            throw new IPCacheException($"Failed to parse IP Cache provider response body for IP {ipAddress}.", ex);
        }
        
        if (ipDetails == null)
        {
            _logger.LogError("IP Cache provider response body was null for IP {Ip}", ipAddress);
            throw new IPCacheException("Empty response from IP Cache provider.");
        }

        return ipDetails;
    }
}