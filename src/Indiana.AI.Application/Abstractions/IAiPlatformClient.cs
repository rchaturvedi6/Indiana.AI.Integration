using Indiana.AI.Domain.Models;

namespace Indiana.AI.Application.Abstractions;

/// <summary>
/// Represents the external AI Verification Service platform boundary (Enso).
/// The local mock simulates the platform in-process; the live implementation
/// forwards to the real Enso <c>/pe/document_upload</c> and
/// <c>/pe/document_validate</c> endpoints. Swapping the registration for this
/// single interface flips the whole solution between mock and live.
/// </summary>
public interface IAiPlatformClient
{
    /// <summary>Submits an upload (API 1) and returns the acknowledgement.</summary>
    Task<UploadResponse> UploadAsync(UploadRequest request, CancellationToken cancellationToken = default);

    /// <summary>Submits a validate request (API 3) and returns the validation results.</summary>
    Task<DocumentValidationResponse> ValidateAsync(UploadRequest request, CancellationToken cancellationToken = default);
}
