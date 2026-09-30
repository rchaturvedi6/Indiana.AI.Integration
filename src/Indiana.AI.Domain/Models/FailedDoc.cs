using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Indiana.AI.Domain.Models;

/// <summary>
/// A document that failed validation (schema <c>FailedDoc</c>), emitted in the
/// <c>failedDocs</c> array with its associated <c>validation_errors</c>.
/// </summary>
public class FailedDoc
{
    /// <summary>The file identifier (GUID) this result applies to.</summary>
    [JsonPropertyName("fileId")]
    public string FileId { get; set; } = string.Empty;

    /// <summary>Field-level validation errors for the document.</summary>
    [JsonPropertyName("validation_errors")]
    public List<ValidationError> ValidationErrors { get; set; } = new();
}
