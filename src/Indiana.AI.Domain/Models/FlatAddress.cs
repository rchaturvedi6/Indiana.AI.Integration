using System.Text.Json.Serialization;

namespace Indiana.AI.Domain.Models;

/// <summary>Flat address used in the Doc Upload / Doc Validate payloads.</summary>
public class FlatAddress
{
    /// <summary>Street line 1.</summary>
    [JsonPropertyName("line1")]
    public string Line1 { get; set; } = string.Empty;

    /// <summary>Street line 2 (optional).</summary>
    [JsonPropertyName("line2")]
    public string? Line2 { get; set; }

    /// <summary>City.</summary>
    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    /// <summary>State id / code.</summary>
    [JsonPropertyName("stateId")]
    public string? StateId { get; set; }

    /// <summary>State display value.</summary>
    [JsonPropertyName("stateValue")]
    public string? StateValue { get; set; }

    /// <summary>Postal code.</summary>
    [JsonPropertyName("postalCode")]
    public string? PostalCode { get; set; }

    /// <summary>County id.</summary>
    [JsonPropertyName("countyId")]
    public string? CountyId { get; set; }

    /// <summary>County display value.</summary>
    [JsonPropertyName("countyValue")]
    public string? CountyValue { get; set; }

    /// <summary>Country id.</summary>
    [JsonPropertyName("countryId")]
    public string? CountryId { get; set; }

    /// <summary>Country display value.</summary>
    [JsonPropertyName("countryValue")]
    public string? CountryValue { get; set; }

    /// <summary>Whether the address has been validated ("True"/"False").</summary>
    [JsonPropertyName("isValid")]
    public string? IsValid { get; set; }

    /// <summary>Latitude.</summary>
    [JsonPropertyName("latitude")]
    public string? Latitude { get; set; }

    /// <summary>Longitude.</summary>
    [JsonPropertyName("longitude")]
    public string? Longitude { get; set; }
}
