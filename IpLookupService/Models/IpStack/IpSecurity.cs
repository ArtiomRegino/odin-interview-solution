using System.Text.Json;
using System.Text.Json.Serialization;

namespace IpLookupService.Models.IpStack;

public class IpSecurity
{
    [JsonPropertyName("is_proxy")]
    public bool? IsProxy { get; set; }

    [JsonPropertyName("proxy_type")]
    public string? ProxyType { get; set; }

    [JsonPropertyName("is_crawler")]
    public bool? IsCrawler { get; set; }

    [JsonPropertyName("crawler_name")]
    public string? CrawlerName { get; set; }

    [JsonPropertyName("crawler_type")]
    public string? CrawlerType { get; set; }

    [JsonPropertyName("is_tor")]
    public bool? IsTor { get; set; }

    [JsonPropertyName("threat_level")]
    public string? ThreatLevel { get; set; }
    
    [JsonPropertyName("threat_types")]
    public JsonElement? ThreatTypes { get; set; }

    [JsonPropertyName("anonymizer_status")]
    public string? AnonymizerStatus { get; set; }

    [JsonPropertyName("proxy_last_detected")]
    public string? ProxyLastDetected { get; set; }

    [JsonPropertyName("proxy_level")]
    public string? ProxyLevel { get; set; }

    [JsonPropertyName("vpn_service")]
    public string? VpnService { get; set; }

    [JsonPropertyName("hosting_facility")]
    public bool? HostingFacility { get; set; }
}