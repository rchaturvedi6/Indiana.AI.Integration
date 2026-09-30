using System;
using System.Collections.Generic;
using Indiana.AI.Domain.Enums;

namespace Indiana.AI.Domain.Models;

/// <summary>
/// The persisted verification outcome for a document. Stored by the
/// Verification Result Repository and returned by
/// <c>GET /api/verification-results/{fileId}</c>.
/// </summary>
public class VerificationResult
{
    /// <summary>The file identifier (GUID) this result applies to.</summary>
    public string FileId { get; set; } = string.Empty;

    /// <summary>The enrollment tracking id the document belonged to.</summary>
    public string TrackingId { get; set; } = string.Empty;

    /// <summary>Request correlation GUID.</summary>
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>Overall verification status for the document.</summary>
    public VerificationStatus Status { get; set; } = VerificationStatus.Pending;

    /// <summary>Wire response code associated with the outcome.</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Field-level validation errors captured during verification.</summary>
    public List<ValidationError> ValidationErrors { get; set; } = new();

    /// <summary>Timestamp (UTC) the result was persisted.</summary>
    public DateTime VerifiedUtc { get; set; } = DateTime.UtcNow;
}
