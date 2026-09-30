using System.Text.Json.Serialization;

namespace Indiana.AI.Domain.Models;

/// <summary>Professional license detail.</summary>
public class LicenseDetail
{
    /// <summary>License number.</summary>
    [JsonPropertyName("licenseNumber")]
    public string LicenseNumber { get; set; } = string.Empty;

    /// <summary>Issuing state code.</summary>
    [JsonPropertyName("stateCode")]
    public string? StateCode { get; set; }

    /// <summary>Assigning board code.</summary>
    [JsonPropertyName("assigningBoard")]
    public string? AssigningBoard { get; set; }

    /// <summary>Assigning board name.</summary>
    [JsonPropertyName("assigningBoardName")]
    public string? AssigningBoardName { get; set; }

    /// <summary>Effective date.</summary>
    [JsonPropertyName("effectiveDate")]
    public string? EffectiveDate { get; set; }

    /// <summary>Expiration date.</summary>
    [JsonPropertyName("expirationDate")]
    public string? ExpirationDate { get; set; }
}
