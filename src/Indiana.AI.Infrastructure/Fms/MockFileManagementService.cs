using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Indiana.AI.Application.Abstractions;
using Indiana.AI.Infrastructure.Adapters;
using Indiana.AI.Domain.Models;

namespace Indiana.AI.Infrastructure.Fms;

/// <summary>
/// Mock File Management Server. Serves the <c>GetEDMSFileQuery</c> contract from
/// the seeded <see cref="MockEdmsRepository"/> so the AI Verification Service can
/// download a file during validation without a real Oracle / EDMS. Always
/// returns mock data, independent of the mock/live integration toggle, because
/// this endpoint *is* the stand-in FMS.
/// </summary>
public class MockFileManagementService : IFileManagementService
{
    private readonly MockEdmsRepository _repository;

    /// <summary>Creates a new <see cref="MockFileManagementService"/>.</summary>
    public MockFileManagementService(MockEdmsRepository repository) => _repository = repository;

    /// <inheritdoc />
    public Task<GetEdmsFileResponse> GetEdmsFileAsync(GetEdmsFileRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var where = request.Where ?? new EdmsWhere();
        var record = _repository.GetFile(where.EdmsFileId)
            ?? throw new KeyNotFoundException(
                $"EDMS file '{where.EdmsFileId}' was not found (tenant '{where.TenantId}').");

        var response = new GetEdmsFileResponse
        {
            Where = where,
            QueryResult = record,
            PageSize = 0,
            CurrentPage = 0,
            RowCount = 1
        };

        return Task.FromResult(response);
    }
}
