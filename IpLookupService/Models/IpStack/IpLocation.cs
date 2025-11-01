using System.Text.Json.Serialization;

namespace IpLookupService.Models.IpStack;

public class IpLocation
{
    [JsonPropertyName("geoname_id")]
    public long? GeoNameId { get; set; }

    [JsonPropertyName("capital")]
    public string? Capital { get; set; }
    
    [JsonPropertyName("languages")]
    public List<IpLanguage>? Languages { get; set; }

    [JsonPropertyName("country_flag")]
    public string? CountryFlag { get; set; }

    [JsonPropertyName("country_flag_emoji")]
    public string? CountryFlagEmoji { get; set; }

    [JsonPropertyName("country_flag_emoji_unicode")]
    public string? CountryFlagEmojiUnicode { get; set; }

    [JsonPropertyName("calling_code")]
    public string? CallingCode { get; set; }

    [JsonPropertyName("is_eu")]
    public bool? IsEu { get; set; }
}