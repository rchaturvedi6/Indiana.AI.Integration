using System.Text.Json.Serialization;

namespace Indiana.AI.Domain.Models;

/// <summary>
/// A document attachment included in an upload / validate payload. The binary
/// content is retrieved later through the File Management Server (API 2) using
/// <see cref="FileId"/>.
/// </summary>
public class Attachment
{
    /// <summary>Unique file identifier (GUID).</summary>
    [JsonPropertyName("fileId")]
    public string FileId { get; set; } = string.Empty;

    /// <summary>Original file name, for example "2.Federal_W9_Tax_Form.pdf".</summary>
    [JsonPropertyName("fileName")]
    public string FileName { get; set; } = string.Empty;

    /// <summary>Attachment type code the provider entered, for example "Federal", "HospLic".</summary>
    [JsonPropertyName("attachmentTypeCode")]
    public string AttachmentTypeCode { get; set; } = string.Empty;

    /// <summary>Attachment type display name, for example "Federal W-9 Form".</summary>
    [JsonPropertyName("attachmentTypeName")]
    public string AttachmentTypeName { get; set; } = string.Empty;
}
