using System.Text.Json.Serialization;

namespace Indiana.AI.Domain.Models;

/// <summary>
/// A single field-level validation error (schema <c>ValidationError</c>)
/// produced by the AI Verification Service.
/// </summary>
public class ValidationError
{
    /// <summary>Validation error code, for example "3001" or "3116".</summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>The value the applicant entered on the enrollment.</summary>
    [JsonPropertyName("entered")]
    public string? Entered { get; set; }

    /// <summary>The value the AI extracted from the uploaded document.</summary>
    [JsonPropertyName("extracted")]
    public string? Extracted { get; set; }
}
