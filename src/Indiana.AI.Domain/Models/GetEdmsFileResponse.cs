using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Indiana.AI.Domain.Models;

/// <summary>
/// The file record returned by the File Management Server <c>GetEDMSFileQuery</c>
/// (API 2), including the Base64 encoded file content.
/// </summary>
public class EdmsQueryResult
{
    /// <summary>The EDMS file identifier (GUID).</summary>
    [JsonPropertyName("edmsFileId")]
    public string EdmsFileId { get; set; } = string.Empty;

    /// <summary>File name.</summary>
    [JsonPropertyName("fileName")]
    public string FileName { get; set; } = string.Empty;

    /// <summary>File extension without the leading dot, for example "pdf".</summary>
    [JsonPropertyName("fileExtension")]
    public string FileExtension { get; set; } = "pdf";

    /// <summary>Base64 encoded file content.</summary>
    [JsonPropertyName("fileData")]
    public string FileData { get; set; } = string.Empty;

    /// <summary>EDMS key identifier metadata.</summary>
    [JsonPropertyName("keyIdentifiers")]
    public Dictionary<string, string> KeyIdentifiers { get; set; } = new();

    /// <summary>Owning module.</summary>
    [JsonPropertyName("moduleName")]
    public string ModuleName { get; set; } = "Provider Enrollment";

    /// <summary>User id associated with the record.</summary>
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    /// <summary>Last modified timestamp.</summary>
    [JsonPropertyName("lastModifiedDate")]
    public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;

    /// <summary>Whether the record is active.</summary>
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Response envelope for the File Management Server <c>GetEDMSFileQuery</c>
/// (API 2), mirroring the query-result envelope with paging metadata.
/// </summary>
public class GetEdmsFileResponse
{
    /// <summary>Echoed query filter.</summary>
    [JsonPropertyName("where")]
    public EdmsWhere Where { get; set; } = new();

    /// <summary>The retrieved file record.</summary>
    [JsonPropertyName("queryResult")]
    public EdmsQueryResult QueryResult { get; set; } = new();

    /// <summary>Page size.</summary>
    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    /// <summary>Current page.</summary>
    [JsonPropertyName("currentPage")]
    public int CurrentPage { get; set; }

    /// <summary>Row count.</summary>
    [JsonPropertyName("rowCount")]
    public int RowCount { get; set; } = 1;
}
