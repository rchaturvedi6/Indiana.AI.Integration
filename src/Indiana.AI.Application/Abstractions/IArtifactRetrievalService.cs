using System.Threading;
using System.Threading.Tasks;
using Indiana.AI.Domain.Models;

namespace Indiana.AI.Application.Abstractions;

/// <summary>
/// Retrieves document artifacts from a tenant's document store (API 2). The
/// implementation routes the request through the tenant-aware router to the
/// appropriate document adapter (simulating <c>GetEDMSFileQuery</c>).
/// </summary>
public interface IArtifactRetrievalService
{
    /// <summary>Retrieves a document artifact for the supplied query request.</summary>
    Task<GetEdmsFileResponse> RetrieveAsync(GetEdmsFileRequest request, CancellationToken cancellationToken = default);

    /// <summary>Retrieves a document artifact by file id, scoped to a tenant.</summary>
    Task<EdmsQueryResult> RetrieveByFileIdAsync(string fileId, string? tenantId, CancellationToken cancellationToken = default);
}
