using System.Text.Json.Serialization;

namespace IpLookupService.Models.IpStack;

public class IpCurrency
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("plural")]
    public string? Plural { get; set; }

    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }

    [JsonPropertyName("symbol_native")]
    public string? SymbolNative { get; set; }
}