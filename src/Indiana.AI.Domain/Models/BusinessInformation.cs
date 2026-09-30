using System.Text.Json.Serialization;

namespace Indiana.AI.Domain.Models;

/// <summary>Business / legal entity information for the provider.</summary>
public class BusinessInformation
{
    /// <summary>Doing-business-as name (optional).</summary>
    [JsonPropertyName("doingBusinessAsName")]
    public string? DoingBusinessAsName { get; set; }

    /// <summary>Legal name.</summary>
    [JsonPropertyName("legalName")]
    public string LegalName { get; set; } = string.Empty;

    /// <summary>Tax name (optional).</summary>
    [JsonPropertyName("taxName")]
    public string? TaxName { get; set; }
}
