using System.Text.Json.Serialization;

namespace Indiana.AI.Domain.Models;

/// <summary>Malpractice / professional liability insurance detail.</summary>
public class MalpracticeInsurance
{
    /// <summary>Carrier type name.</summary>
    [JsonPropertyName("carrierTypeName")]
    public string? CarrierTypeName { get; set; }

    /// <summary>Policy number.</summary>
    [JsonPropertyName("policyNumber")]
    public string? PolicyNumber { get; set; }

    /// <summary>Carrier type code.</summary>
    [JsonPropertyName("carrierTypeCode")]
    public string? CarrierTypeCode { get; set; }

    /// <summary>Effective date.</summary>
    [JsonPropertyName("effectiveDate")]
    public string? EffectiveDate { get; set; }

    /// <summary>Carrier name.</summary>
    [JsonPropertyName("carrierName")]
    public string? CarrierName { get; set; }

    /// <summary>Per-occurrence coverage amount.</summary>
    [JsonPropertyName("coverageAmountPerOccurrence")]
    public double? CoverageAmountPerOccurrence { get; set; }

    /// <summary>Aggregate coverage amount.</summary>
    [JsonPropertyName("coverageAmountAggregate")]
    public double? CoverageAmountAggregate { get; set; }

    /// <summary>End date.</summary>
    [JsonPropertyName("endDate")]
    public string? EndDate { get; set; }
}
