using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Indiana.AI.Domain.Models;

namespace Indiana.AI.Application.Abstractions;

/// <summary>
/// Persists and retrieves <see cref="VerificationResult"/> records.
/// </summary>
public interface IVerificationResultRepository
{
    /// <summary>Persists (inserts or replaces) a verification result.</summary>
    Task SaveAsync(VerificationResult result, CancellationToken cancellationToken = default);

    /// <summary>Retrieves the stored result for a file, or <c>null</c> when absent.</summary>
    Task<VerificationResult?> GetByFileIdAsync(string fileId, CancellationToken cancellationToken = default);

    /// <summary>Retrieves all stored verification results.</summary>
    Task<IReadOnlyList<VerificationResult>> GetAllAsync(CancellationToken cancellationToken = default);
}
