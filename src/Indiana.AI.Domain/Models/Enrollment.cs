using System.Text.Json.Serialization;

namespace Indiana.AI.Domain.Models;

/// <summary>
/// Enrollment block (schema <c>DocEnrollment</c>) carried by the upload and
/// validate payloads.
/// </summary>
public class Enrollment
{
    /// <summary>Tracking identifier for the enrollment application.</summary>
    [JsonPropertyName("trackingID")]
    public string TrackingID { get; set; } = string.Empty;

    /// <summary>Application type, for example "NewEnrollment".</summary>
    [JsonPropertyName("applicationType")]
    public string? ApplicationType { get; set; }

    /// <summary>Enrollment type code, for example "Facility".</summary>
    [JsonPropertyName("enrollmentType")]
    public string? EnrollmentType { get; set; }

    /// <summary>Enrollment type display name.</summary>
    [JsonPropertyName("enrollmentTypeName")]
    public string? EnrollmentTypeName { get; set; }

    /// <summary>Provider type code, for example "Hospital".</summary>
    [JsonPropertyName("providerType")]
    public string? ProviderType { get; set; }

    /// <summary>Provider type display name.</summary>
    [JsonPropertyName("providerTypeName")]
    public string? ProviderTypeName { get; set; }

    /// <summary>Provider information.</summary>
    [JsonPropertyName("providerInformation")]
    public ProviderInformation ProviderInformation { get; set; } = new();

    /// <summary>Desired effective date.</summary>
    [JsonPropertyName("desiredEffectiveDate")]
    public string? DesiredEffectiveDate { get; set; }
}
