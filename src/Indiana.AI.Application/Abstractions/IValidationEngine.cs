using System.Threading;
using System.Threading.Tasks;
using Indiana.AI.Domain.Models;

namespace Indiana.AI.Application.Abstractions;

/// <summary>
/// Orchestrates the validation of an upload: maps the payload, retrieves each
/// artifact, runs the mock verification and produces the validation response.
/// </summary>
public interface IValidationEngine
{
    /// <summary>
    /// Validates all attachments in the supplied upload and returns the
    /// aggregated validation response.
    /// </summary>
    Task<DocumentValidationResponse> ValidateAsync(
        UploadRequest request,
        TrackingContext tracking,
        CancellationToken cancellationToken = default);
}
