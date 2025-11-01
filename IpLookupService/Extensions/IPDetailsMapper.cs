using IpLookupService.Models;
using IpLookupService.Models.IpStack;

namespace IpLookupService.Extensions;

public static class IPDetailsMapper
{
    public static IPDetailsDto MapToDto(this IpStackResponse src)
    {
        return new IPDetailsDto
        {
            Ip = src.Ip,
            Type = src.Type,
            ContinentCode = src.ContinentCode,
            ContinentName = src.ContinentName,
            CountryCode = src.CountryCode,
            CountryName = src.CountryName,
            RegionName = src.RegionName,
            City = src.City,
            Zip = src.Zip,
            Latitude = src.Latitude,
            Longitude = src.Longitude,

            TimeZoneId = src.TimeZone?.Id,
            TimeZoneCurrentTime = src.TimeZone?.CurrentTime,
            TimeZoneGmtOffset = src.TimeZone?.GmtOffset,
            TimeZoneIsDaylightSaving = src.TimeZone?.IsDaylightSaving,

            Isp = src.Connection?.Isp,
            Asn = src.Connection?.Asn,
            IsProxy = src.Security?.IsProxy,
            IsTor = src.Security?.IsTor,
            ThreatLevel = src.Security?.ThreatLevel
        };
    }
}