using System.Text.Json.Serialization;

namespace Indiana.AI.Domain.Models;

/// <summary>
/// Common envelope identifying the caller and request context, shared by the
/// upload and validate contracts (APIs 1 and 3).
/// </summary>
public class Route
{
    /// <summary>Logical service domain, for example "prov-enroll".</summary>
    [JsonPropertyName("domain")]
    public string Domain { get; set; } = "prov-enroll";

    /// <summary>API version, for example 1.5.</summary>
    [JsonPropertyName("apiVer")]
    public double? ApiVer { get; set; } = 1.5;

    /// <summary>Tenant identifier (GUID).</summary>
    [JsonPropertyName("tenantID")]
    public string TenantID { get; set; } = string.Empty;

    /// <summary>Calling application client, for example "prov_portal_ftest".</summary>
    [JsonPropertyName("appClient")]
    public string AppClient { get; set; } = string.Empty;

    /// <summary>Optional client identifier.</summary>
    [JsonPropertyName("clientID")]
    public string? ClientID { get; set; }

    /// <summary>Correlation GUID issued by the caller.</summary>
    [JsonPropertyName("requestGUID")]
    public string RequestGUID { get; set; } = string.Empty;

    /// <summary>
    /// Request discriminator: <c>document_upload</c> (API 1) or
    /// <c>document_validate</c> (API 3).
    /// </summary>
    [JsonPropertyName("requestType")]
    public string RequestType { get; set; } = "document_upload";

    /// <summary>Client-generated timestamp identifier.</summary>
    [JsonPropertyName("timeStampId")]
    public string? TimeStampId { get; set; }
}
