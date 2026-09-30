using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Indiana.AI.Application.Abstractions;
using Indiana.AI.Domain.Models;

namespace Indiana.AI.Infrastructure.Adapters;

/// <summary>
/// Mock Indiana Oracle adapter. Represents Indiana's Oracle BLOB document store:
/// retrieves an artifact by file id (scoped to the tenant) and returns metadata
/// plus Base64 content in the structure the AI service expects. Backed by the
/// seeded <see cref="MockEdmsRepository"/> (stand-in for the Oracle BLOB store).
/// </summary>
public class IndianaOracleAdapter : IDocumentAdapter
{
    private readonly MockEdmsRepository _repository;

    /// <summary>Creates a new <see cref="IndianaOracleAdapter"/>.</summary>
    public IndianaOracleAdapter(MockEdmsRepository repository) => _repository = repository;

    /// <inheritdoc />
    public string Name => "IndianaOracleAdapter";

    /// <inheritdoc />
    public Task<EdmsQueryResult> GetArtifactAsync(string fileId, string? tenantId, CancellationToken cancellationToken = default)
    {
        var record = _repository.GetFile(fileId)
            ?? throw new KeyNotFoundException(
                $"Oracle BLOB '{fileId}' was not found for tenant '{tenantId}'.");

        return Task.FromResult(record);
    }
}
