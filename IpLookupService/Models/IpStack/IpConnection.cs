using System.Text.Json.Serialization;

namespace IpLookupService.Models.IpStack;

public class IpConnection
{
    [JsonPropertyName("asn")]
    public string? Asn { get; set; }

    [JsonPropertyName("isp")]
    public string? Isp { get; set; }

    [JsonPropertyName("sld")]
    public string? Sld { get; set; }

    [JsonPropertyName("tld")]
    public string? Tld { get; set; }

    [JsonPropertyName("carrier")]
    public string? Carrier { get; set; }

    [JsonPropertyName("home")]
    public bool? Home { get; set; }

    [JsonPropertyName("organization_type")]
    public string? OrganizationType { get; set; }

    [JsonPropertyName("isic_code")]
    public string? IsicCode { get; set; }

    [JsonPropertyName("naics_code")]
    public string? NaicsCode { get; set; }
}