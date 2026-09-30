using System.Text.Json.Serialization;

namespace Indiana.AI.Domain.Models;

/// <summary>DME surety bond detail.</summary>
public class DmeSuretyBond
{
    /// <summary>Bond number (optional).</summary>
    [JsonPropertyName("bondNumber")]
    public string? BondNumber { get; set; }

    /// <summary>Effective date (optional).</summary>
    [JsonPropertyName("effectiveDate")]
    public string? EffectiveDate { get; set; }
}
