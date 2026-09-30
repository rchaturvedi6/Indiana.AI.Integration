using System.Text.Json.Serialization;

namespace Indiana.AI.Domain.Models;

/// <summary>DEA registration detail.</summary>
public class DeaDetail
{
    /// <summary>DEA number.</summary>
    [JsonPropertyName("deaNumber")]
    public string DeaNumber { get; set; } = string.Empty;

    /// <summary>Effective date.</summary>
    [JsonPropertyName("effectiveDate")]
    public string? EffectiveDate { get; set; }

    /// <summary>End date.</summary>
    [JsonPropertyName("endDate")]
    public string? EndDate { get; set; }
}
