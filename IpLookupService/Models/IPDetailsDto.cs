namespace IpLookupService.Models;

public class IPDetailsDto
{
    public string Ip { get; set; } = null!;
    public string? Type { get; set; }
    public string? ContinentCode { get; set; }
    public string? ContinentName { get; set; }
    public string? CountryCode { get; set; }
    public string? CountryName { get; set; }
    public string? RegionName { get; set; }
    public string? City { get; set; }
    public string? Zip { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public string? TimeZoneId { get; set; }
    public string? TimeZoneCurrentTime { get; set; }
    public int? TimeZoneGmtOffset { get; set; }
    public bool? TimeZoneIsDaylightSaving { get; set; }

    public string? Isp { get; set; }
    public string? Asn { get; set; }
    public bool? IsProxy { get; set; }
    public bool? IsTor { get; set; }
    public string? ThreatLevel { get; set; }
}