using System.Collections.Generic;

namespace Indiana.AI.Api.Security;

/// <summary>
/// Binds the <c>Security</c> configuration section. Controls inbound handling of
/// the AI Verification Service security contract (<c>X-API-Key</c> /
/// <c>X-Source</c>). Enforcement is opt-in so the POC/demo runs without
/// credentials by default.
/// </summary>
public class SecurityOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "Security";

    /// <summary>
    /// When true, protected routes require a valid <see cref="ApiKeyHeaderName"/>
    /// (and <see cref="SourceHeaderName"/>). Default false (demo-friendly).
    /// </summary>
    public bool RequireApiKey { get; set; } = false;

    /// <summary>API key header name. Defaults to "X-API-Key".</summary>
    public string ApiKeyHeaderName { get; set; } = "X-API-Key";

    /// <summary>Source header name. Defaults to "X-Source".</summary>
    public string SourceHeaderName { get; set; } = "X-Source";

    /// <summary>
    /// When true, the <see cref="SourceHeaderName"/> header must also be present.
    /// </summary>
    public bool RequireSource { get; set; } = true;

    /// <summary>
    /// Optional allow-list of accepted API keys. When empty, any non-empty key is
    /// accepted (presence check only) — suitable for a POC.
    /// </summary>
    public List<string> AllowedApiKeys { get; set; } = new();

    /// <summary>
    /// Path prefixes that require authentication when enforcement is on.
    /// These are the gateway's inbound (InterChange-facing) routes.
    /// </summary>
    public List<string> ProtectedPathPrefixes { get; set; } = new() { "/api/document", "/api/artifact" };
}
