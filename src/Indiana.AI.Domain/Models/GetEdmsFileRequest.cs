using System.Text.Json.Serialization;

namespace Indiana.AI.Domain.Models;

/// <summary>
/// Query filter for the File Management Server <c>GetEDMSFileQuery</c> (API 2).
/// </summary>
public class EdmsWhere
{
    /// <summary>The EDMS file identifier (GUID) to retrieve.</summary>
    [JsonPropertyName("edmsFileId")]
    public string EdmsFileId { get; set; } = string.Empty;

    /// <summary>Tenant identifier (GUID).</summary>
    [JsonPropertyName("tenantId")]
    public string? TenantId { get; set; }

    /// <summary>Owning module, for example "Provider Enrollment".</summary>
    [JsonPropertyName("moduleName")]
    public string? ModuleName { get; set; } = "Provider Enrollment";

    /// <summary>Optional additional file details filter.</summary>
    [JsonPropertyName("fileDetails")]
    public object? FileDetails { get; set; }
}

/// <summary>
/// Requestor context block carried by the File Management Server query.
/// </summary>
public class EdmsRequestor
{
    /// <summary>Correlation id.</summary>
    [JsonPropertyName("correlationId")]
    public string? CorrelationId { get; set; }

    /// <summary>Tenant identifier (GUID).</summary>
    [JsonPropertyName("tenantId")]
    public string? TenantId { get; set; }

    /// <summary>Calling application name.</summary>
    [JsonPropertyName("applicationName")]
    public string? ApplicationName { get; set; }
}

/// <summary>
/// Request envelope for the File Management Server <c>GetEDMSFileQuery</c>
/// (API 2). Retrieves the binary content of an uploaded EDMS file.
/// </summary>
public class GetEdmsFileRequest
{
    /// <summary>Query filter identifying the file.</summary>
    [JsonPropertyName("where")]
    public EdmsWhere Where { get; set; } = new();

    /// <summary>Requestor context.</summary>
    [JsonPropertyName("requestor")]
    public EdmsRequestor? Requestor { get; set; }
}
