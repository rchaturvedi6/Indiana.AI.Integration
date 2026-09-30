using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Indiana.AI.Application.Abstractions;
using Indiana.AI.Domain.Models;

namespace Indiana.AI.Infrastructure.Adapters;

/// <summary>
/// Mock file adapter for future states that don't yet have a real back-end.
/// Demonstrates a fallback / placeholder tenant channel. Backed by the seeded
/// <see cref="MockEdmsRepository"/>.
/// </summary>
public class MockFileAdapter : IDocumentAdapter
{
    private readonly MockEdmsRepository _repository;

    /// <summary>Creates a new <see cref="MockFileAdapter"/>.</summary>
    public MockFileAdapter(MockEdmsRepository repository) => _repository = repository;

    /// <inheritdoc />
    public string Name => "MockFileAdapter";

    /// <inheritdoc />
    public Task<EdmsQueryResult> GetArtifactAsync(string fileId, string? tenantId, CancellationToken cancellationToken = default)
    {
        var record = _repository.GetFile(fileId)
            ?? throw new KeyNotFoundException(
                $"File '{fileId}' was not found for tenant '{tenantId}'.");

        return Task.FromResult(record);
    }
}
