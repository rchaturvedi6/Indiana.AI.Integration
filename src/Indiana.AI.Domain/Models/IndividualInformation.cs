using System.Text.Json.Serialization;

namespace Indiana.AI.Domain.Models;

/// <summary>Individual (person) information for the provider.</summary>
public class IndividualInformation
{
    /// <summary>Provider name.</summary>
    [JsonPropertyName("name")]
    public Name Name { get; set; } = new();

    /// <summary>Gender (M / F / U).</summary>
    [JsonPropertyName("gender")]
    public string? Gender { get; set; }

    /// <summary>Date of birth.</summary>
    [JsonPropertyName("dateOfBirth")]
    public string? DateOfBirth { get; set; }
}
