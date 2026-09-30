using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Indiana.AI.Application.Abstractions;
using Indiana.AI.Domain.Models;

namespace Indiana.AI.Infrastructure.Adapters;

/// <summary>
/// Mock SQL adapter (e.g. for a state such as Nevada that stores documents in a
/// SQL back-end). Demonstrates that a different tenant resolves to a different
/// adapter with no change to AI logic. Backed by the seeded
/// <see cref="MockEdmsRepository"/> for the POC.
/// </summary>
public class SqlAdapter : IDocumentAdapter
{
    private readonly MockEdmsRepository _repository;

    /// <summary>Creates a new <see cref="SqlAdapter"/>.</summary>
    public SqlAdapter(MockEdmsRepository repository) => _repository = repository;

    /// <inheritdoc />
    public string Name => "SqlAdapter";

    /// <inheritdoc />
    public Task<EdmsQueryResult> GetArtifactAsync(string fileId, string? tenantId, CancellationToken cancellationToken = default)
    {
        var record = _repository.GetFile(fileId)
            ?? throw new KeyNotFoundException(
                $"SQL document '{fileId}' was not found for tenant '{tenantId}'.");

        return Task.FromResult(record);
    }
}
