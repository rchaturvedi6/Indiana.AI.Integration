using System.Text.Json.Serialization;

namespace Indiana.AI.Domain.Models;

/// <summary>CLIA (Clinical Laboratory Improvement Amendments) certificate detail.</summary>
public class CliaDetail
{
    /// <summary>CLIA number.</summary>
    [JsonPropertyName("cliaNumber")]
    public string CliaNumber { get; set; } = string.Empty;

    /// <summary>Effective date.</summary>
    [JsonPropertyName("effectiveDate")]
    public string? EffectiveDate { get; set; }

    /// <summary>End date.</summary>
    [JsonPropertyName("endDate")]
    public string? EndDate { get; set; }

    /// <summary>CLIA type name.</summary>
    [JsonPropertyName("cliaTypeName")]
    public string? CliaTypeName { get; set; }

    /// <summary>CLIA type code.</summary>
    [JsonPropertyName("cliaType")]
    public string? CliaType { get; set; }
}
