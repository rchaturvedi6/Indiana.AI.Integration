using System;
using System.Threading;
using System.Threading.Tasks;
using Indiana.AI.Application.Abstractions;
using Indiana.AI.Domain.Models;

namespace Indiana.AI.Infrastructure.Artifacts;

/// <summary>
/// Artifact Retrieval Service. Routes a retrieval request through the
/// tenant-aware router to the resolved document adapter, mirroring the flow:
/// Artifact Retrieval → Router (tenant) → Adapter Framework → tenant adapter →
/// document store.
/// </summary>
public class ArtifactRetrievalService : IArtifactRetrievalService
{
    private readonly IRouter _router;

    /// <summary>Creates a new <see cref="ArtifactRetrievalService"/>.</summary>
    public ArtifactRetrievalService(IRouter router) => _router = router;

    /// <inheritdoc />
    public async Task<GetEdmsFileResponse> RetrieveAsync(GetEdmsFileRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var where = request.Where ?? new EdmsWhere();
        var result = await RetrieveByFileIdAsync(where.EdmsFileId, where.TenantId, cancellationToken).ConfigureAwait(false);

        return new GetEdmsFileResponse
        {
            Where = where,
            QueryResult = result,
            PageSize = 0,
            CurrentPage = 0,
            RowCount = 1
        };
    }

    /// <inheritdoc />
    public Task<EdmsQueryResult> RetrieveByFileIdAsync(string fileId, string? tenantId, CancellationToken cancellationToken = default)
    {
        var adapter = _router.ResolveAdapter(tenantId);
        return adapter.GetArtifactAsync(fileId, tenantId, cancellationToken);
    }
}
