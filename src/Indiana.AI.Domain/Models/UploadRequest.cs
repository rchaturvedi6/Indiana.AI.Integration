using System.Text.Json.Serialization;

namespace Indiana.AI.Domain.Models;

/// <summary>
/// The upload request contract (schema <c>DocUploadRequest</c>) accepted by
/// <c>POST /api/document-upload</c> (API 1). The validate request
/// (<c>POST /api/document-validate</c>, API 3) uses the same shape with
/// <c>route.requestType = "document_validate"</c>.
/// </summary>
public class UploadRequest
{
    /// <summary>Caller / request context envelope.</summary>
    [JsonPropertyName("route")]
    public Route Route { get; set; } = new();

    /// <summary>Enrollment and provider information.</summary>
    [JsonPropertyName("enrollment")]
    public Enrollment Enrollment { get; set; } = new();
}
