using System.Text.Json.Serialization;

namespace Indiana.AI.Domain.Models;

/// <summary>Provider education / degree detail.</summary>
public class EducationDetail
{
    /// <summary>Degree name.</summary>
    [JsonPropertyName("degreeName")]
    public string? DegreeName { get; set; }

    /// <summary>Provider degree id.</summary>
    [JsonPropertyName("providerDegreeId")]
    public string? ProviderDegreeId { get; set; }

    /// <summary>Whether this is a revalidation.</summary>
    [JsonPropertyName("isRevalidation")]
    public bool IsRevalidation { get; set; }

    /// <summary>Year of graduation.</summary>
    [JsonPropertyName("yearOfGraduation")]
    public string? YearOfGraduation { get; set; }

    /// <summary>School attended.</summary>
    [JsonPropertyName("school")]
    public string? School { get; set; }

    /// <summary>Provider date of birth (optional).</summary>
    [JsonPropertyName("providerDOB")]
    public string? ProviderDOB { get; set; }
}
