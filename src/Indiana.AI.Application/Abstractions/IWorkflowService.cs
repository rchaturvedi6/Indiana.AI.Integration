using System.Collections.Generic;
using Indiana.AI.Domain.Models;

namespace Indiana.AI.Application.Abstractions;

/// <summary>
/// The top-level workflow facade used by the API controllers. Coordinates
/// upload acknowledgement, validation orchestration, artifact retrieval and
/// verification-result persistence.
/// </summary>
public interface IWorkflowService
{
    /// <summary>Handles an inbound upload (API 1) and returns the acknowledgement.</summary>
    Task<UploadResponse> ProcessUploadAsync(UploadRequest request, CancellationToken cancellationToken = default);

    /// <summary>Runs validation (API 3), persists the results and returns the response.</summary>
    Task<DocumentValidationResponse> ValidateAsync(UploadRequest request, CancellationToken cancellationToken = default);

    /// <summary>Retrieves a document artifact from the File Management Server (API 2).</summary>
    Task<GetEdmsFileResponse> RetrieveArtifactAsync(GetEdmsFileRequest request, CancellationToken cancellationToken = default);

    /// <summary>Returns the stored verification result for a file, or <c>null</c>.</summary>
    Task<VerificationResult?> GetVerificationResultAsync(string fileId, CancellationToken cancellationToken = default);

    /// <summary>Returns all stored verification results.</summary>
    Task<IReadOnlyList<VerificationResult>> GetAllVerificationResultsAsync(CancellationToken cancellationToken = default);
}
