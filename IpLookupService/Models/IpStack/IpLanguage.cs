using System.Text.Json.Serialization;

namespace IpLookupService.Models.IpStack;

public class IpLanguage
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("native")]
    public string? Native { get; set; }
}