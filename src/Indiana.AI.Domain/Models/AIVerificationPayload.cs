using System;
using System.Collections.Generic;

namespace Indiana.AI.Domain.Models;

/// <summary>A single document entry inside the canonical AI verification payload.</summary>
public class AIDocument
{
    /// <summary>File identifier (GUID).</summary>
    public string FileId { get; set; } = string.Empty;

    /// <summary>File name.</summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>Attachment type code the provider entered.</summary>
    public string AttachmentTypeCode { get; set; } = string.Empty;

    /// <summary>Attachment type display name the provider entered.</summary>
    public string AttachmentTypeName { get; set; } = string.Empty;
}

/// <summary>
/// The canonical payload the AI Integration Gateway produces from an Indiana
/// enrollment <see cref="UploadRequest"/> and forwards to the (mocked) AI
/// Document Verification Service.
/// </summary>
public class AIVerificationPayload
{
    /// <summary>Enrollment tracking identifier.</summary>
    public string TrackingId { get; set; } = string.Empty;

    /// <summary>Request correlation GUID.</summary>
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>Tenant identifier.</summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>Provider NPI.</summary>
    public string ProviderNpi { get; set; } = string.Empty;

    /// <summary>Provider legal name.</summary>
    public string ProviderLegalName { get; set; } = string.Empty;

    /// <summary>Documents to be verified.</summary>
    public List<AIDocument> Documents { get; set; } = new();

    /// <summary>Timestamp (UTC) the payload was created.</summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
