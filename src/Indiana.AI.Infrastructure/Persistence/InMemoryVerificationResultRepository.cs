using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Indiana.AI.Application.Abstractions;
using Indiana.AI.Domain.Models;

namespace Indiana.AI.Infrastructure.Persistence;

/// <summary>
/// Thread-safe in-memory implementation of
/// <see cref="IVerificationResultRepository"/>. Suitable for the POC; a real
/// deployment would persist to a database.
/// </summary>
public class InMemoryVerificationResultRepository : IVerificationResultRepository
{
    private readonly ConcurrentDictionary<string, VerificationResult> _store = new();

    /// <inheritdoc />
    public Task SaveAsync(VerificationResult result, CancellationToken cancellationToken = default)
    {
        _store[result.FileId] = result;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<VerificationResult?> GetByFileIdAsync(string fileId, CancellationToken cancellationToken = default)
    {
        _store.TryGetValue(fileId, out var result);
        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<VerificationResult>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<VerificationResult> all = _store.Values.ToList();
        return Task.FromResult(all);
    }
}
