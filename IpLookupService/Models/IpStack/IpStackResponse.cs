using System.Text.Json.Serialization;

namespace IpLookupService.Models.IpStack;

public class IpStackResponse
{
    [JsonPropertyName("ip")]
    public string Ip { get; set; }

    [JsonPropertyName("hostname")]
    public string? Hostname { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("continent_code")]
    public string? ContinentCode { get; set; }

    [JsonPropertyName("continent_name")]
    public string? ContinentName { get; set; }

    [JsonPropertyName("country_code")]
    public string? CountryCode { get; set; }

    [JsonPropertyName("country_name")]
    public string? CountryName { get; set; }

    [JsonPropertyName("region_code")]
    public string? RegionCode { get; set; }

    [JsonPropertyName("region_name")]
    public string? RegionName { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("zip")]
    public string? Zip { get; set; }

    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }
    
    [JsonPropertyName("msa")]
    public string? Msa { get; set; }

    [JsonPropertyName("dma")]
    public string? Dma { get; set; }

    [JsonPropertyName("radius")]
    public double? Radius { get; set; }

    [JsonPropertyName("ip_routing_type")]
    public string? IpRoutingType { get; set; }

    [JsonPropertyName("connection_type")]
    public string? ConnectionType { get; set; }

    [JsonPropertyName("location")]
    public IpLocation? Location { get; set; }

    [JsonPropertyName("time_zone")]
    public IpTimeZone? TimeZone { get; set; }

    [JsonPropertyName("currency")]
    public IpCurrency? Currency { get; set; }

    [JsonPropertyName("connection")]
    public IpConnection? Connection { get; set; }

    [JsonPropertyName("security")]
    public IpSecurity? Security { get; set; }
}