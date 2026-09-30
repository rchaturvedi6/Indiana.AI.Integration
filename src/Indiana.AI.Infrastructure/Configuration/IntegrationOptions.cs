namespace Indiana.AI.Infrastructure.Configuration;

/// <summary>
/// Binds the <c>Integration</c> configuration section. Controls whether the
/// solution runs against the in-process mocks or the live Enso AI Verification
/// Service and File Management Server.
/// </summary>
public class IntegrationOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "Integration";

    /// <summary>
    /// When true (default) the mocks are used. When false the live Enso and FMS
    /// clients are used and require valid endpoints + credentials below.
    /// </summary>
    public bool UseMocks { get; set; } = true;

    /// <summary>Live Enso AI Verification Service settings (APIs 1 and 3).</summary>
    public EnsoOptions Enso { get; set; } = new();

    /// <summary>Live File Management Server settings (API 2).</summary>
    public FmsOptions Fms { get; set; } = new();
}

/// <summary>Live Enso AI Verification Service settings.</summary>
public class EnsoOptions
{
    /// <summary>Base URL, for example https://enso-apis.enso.dev.pgwcloud.net.</summary>
    public string BaseUrl { get; set; } = "https://enso-apis.enso.dev.pgwcloud.net";

    /// <summary>Relative path for the document upload endpoint (API 1).</summary>
    public string UploadPath { get; set; } = "/pe/document_upload";

    /// <summary>Relative path for the document validate endpoint (API 3).</summary>
    public string ValidatePath { get; set; } = "/pe/document_validate";

    /// <summary>Issued API key sent as the <c>X-API-Key</c> header.</summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>Source identifier sent as the <c>X-Source</c> header.</summary>
    public string Source { get; set; } = "provider_portal";
}

/// <summary>Live File Management Server settings.</summary>
public class FmsOptions
{
    /// <summary>Base URL, for example https://bas-fltest-perf.mapshc.com/HPP.Core.FileMgmtApi.</summary>
    public string BaseUrl { get; set; } = "https://bas-fltest-perf.mapshc.com/HPP.Core.FileMgmtApi";

    /// <summary>Relative path for the GetEDMSFileQuery endpoint (API 2).</summary>
    public string GetEdmsFilePath { get; set; } = "/R6.0/FileMgmtQuery/GetEDMSFileQuery";
}
