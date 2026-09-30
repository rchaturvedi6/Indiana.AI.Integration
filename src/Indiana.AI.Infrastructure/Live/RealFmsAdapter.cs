using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Indiana.AI.Application.Abstractions;
using Indiana.AI.Domain.Models;

namespace Indiana.AI.Infrastructure.Live;

/// <summary>
/// Live File Management Server (FMS / EDMS) adapter. Registered as the
/// "IndianaOracleAdapter" in live mode: calls the real <c>GetEDMSFileQuery</c>
/// endpoint (API 2) for the Indiana tenant and maps the query-result envelope
/// onto <see cref="EdmsQueryResult"/>. Only used when
/// <c>Integration:UseMocks</c> is false.
/// </summary>
public class RealFmsAdapter : IDocumentAdapter
{
    private readonly HttpClient _http;
    private readonly string _getEdmsFilePath;

    /// <summary>Creates a new <see cref="RealFmsAdapter"/>.</summary>
    public RealFmsAdapter(HttpClient http, string getEdmsFilePath)
    {
        _http = http;
        _getEdmsFilePath = getEdmsFilePath;
    }

    /// <inheritdoc />
    public string Name => "IndianaOracleAdapter";

    /// <inheritdoc />
    public async Task<EdmsQueryResult> GetArtifactAsync(string edmsFileId, string? tenantId, CancellationToken cancellationToken = default)
    {
        // The real FMS request envelope filters by EDMS file id + tenant.
        var payload = new GetEdmsFileRequest
        {
            Where = new EdmsWhere { EdmsFileId = edmsFileId, TenantId = tenantId, ModuleName = "Provider Enrollment" }
        };

        using var httpResponse = await _http
            .PostAsJsonAsync(_getEdmsFilePath, payload, cancellationToken)
            .ConfigureAwait(false);
        httpResponse.EnsureSuccessStatusCode();

        var response = await httpResponse.Content
            .ReadFromJsonAsync<GetEdmsFileResponse>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var result = response?.QueryResult;
        if (result is null || string.IsNullOrEmpty(result.EdmsFileId))
        {
            throw new KeyNotFoundException($"EDMS file '{edmsFileId}' was not found by the File Management Server.");
        }

        return result;
    }
}
