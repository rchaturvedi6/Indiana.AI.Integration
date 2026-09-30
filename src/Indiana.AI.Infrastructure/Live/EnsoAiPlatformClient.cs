using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Indiana.AI.Application.Abstractions;
using Indiana.AI.Domain.Models;

namespace Indiana.AI.Infrastructure.Live;

/// <summary>
/// Live implementation of <see cref="IAiPlatformClient"/> that forwards upload
/// (API 1) and validate (API 3) requests to the real Enso AI Verification
/// Service. The typed <see cref="HttpClient"/> is configured with the base
/// address and the <c>X-API-Key</c> / <c>X-Source</c> headers in
/// <c>AddInfrastructure</c>. Only used when <c>Integration:UseMocks</c> is false.
/// </summary>
public class EnsoAiPlatformClient : IAiPlatformClient
{
    private readonly HttpClient _http;
    private readonly string _uploadPath;
    private readonly string _validatePath;

    /// <summary>Creates a new <see cref="EnsoAiPlatformClient"/>.</summary>
    public EnsoAiPlatformClient(HttpClient http, string uploadPath, string validatePath)
    {
        _http = http;
        _uploadPath = uploadPath;
        _validatePath = validatePath;
    }

    /// <inheritdoc />
    public async Task<UploadResponse> UploadAsync(UploadRequest request, CancellationToken cancellationToken = default)
    {
        using var httpResponse = await _http
            .PostAsJsonAsync(_uploadPath, request, cancellationToken)
            .ConfigureAwait(false);
        httpResponse.EnsureSuccessStatusCode();

        return await httpResponse.Content
            .ReadFromJsonAsync<UploadResponse>(cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new InvalidOperationException("Enso upload returned an empty response.");
    }

    /// <inheritdoc />
    public async Task<DocumentValidationResponse> ValidateAsync(UploadRequest request, CancellationToken cancellationToken = default)
    {
        using var httpResponse = await _http
            .PostAsJsonAsync(_validatePath, request, cancellationToken)
            .ConfigureAwait(false);
        httpResponse.EnsureSuccessStatusCode();

        return await httpResponse.Content
            .ReadFromJsonAsync<DocumentValidationResponse>(cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new InvalidOperationException("Enso validate returned an empty response.");
    }
}
