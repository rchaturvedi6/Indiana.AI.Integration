using System.Threading;
using System.Threading.Tasks;
using Indiana.AI.Domain.Models;

namespace Indiana.AI.Application.Abstractions;

/// <summary>
/// A document retrieval adapter for a tenant's document store (for example the
/// Indiana Oracle BLOB store). Each adapter is identified by a unique
/// <see cref="Name"/> and retrieves an artifact by file id and tenant id.
/// </summary>
public interface IDocumentAdapter
{
    /// <summary>
    /// The adapter's unique name, matched against the tenant→adapter map in
    /// configuration (for example "IndianaOracleAdapter").
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Retrieves a document artifact by its file id, scoped to the supplied
    /// tenant id.
    /// </summary>
    Task<EdmsQueryResult> GetArtifactAsync(string fileId, string? tenantId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Resolves the concrete <see cref="IDocumentAdapter"/> for a given adapter
/// name. This is the entry point of the adapter framework.
/// </summary>
public interface IAdapterResolver
{
    /// <summary>Resolves an adapter by name; throws when none is registered.</summary>
    IDocumentAdapter Resolve(string adapterName);
}
