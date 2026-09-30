using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Indiana.AI.Domain.Models;

/// <summary>A provider specialty and its taxonomy.</summary>
public class Specialty
{
    /// <summary>Specialty code.</summary>
    [JsonPropertyName("specialtyCode")]
    public string SpecialtyCode { get; set; } = string.Empty;

    /// <summary>Specialty name.</summary>
    [JsonPropertyName("specialtyName")]
    public string SpecialtyName { get; set; } = string.Empty;

    /// <summary>Whether this is the primary specialty.</summary>
    [JsonPropertyName("isPrimary")]
    public bool IsPrimary { get; set; }

    /// <summary>Associated programs.</summary>
    [JsonPropertyName("programs")]
    public List<object> Programs { get; set; } = new();

    /// <summary>Taxonomy code.</summary>
    [JsonPropertyName("taxonomyCode")]
    public string? TaxonomyCode { get; set; }

    /// <summary>Taxonomy name.</summary>
    [JsonPropertyName("taxonomyName")]
    public string? TaxonomyName { get; set; }

    /// <summary>Associated waivers.</summary>
    [JsonPropertyName("waivers")]
    public List<object> Waivers { get; set; } = new();
}
