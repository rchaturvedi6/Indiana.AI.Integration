using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Indiana.AI.Application.Abstractions;
using Indiana.AI.Domain.Enums;
using Indiana.AI.Domain.Models;

namespace Indiana.AI.Application.Services;

/// <summary>
/// Orchestrates document validation (API 3). For each attachment it retrieves
/// the artifact from the File Management Server, submits it to the mock AI
/// verification service and aggregates the results into a
/// <see cref="DocumentValidationResponse"/>.
/// </summary>
public class ValidationEngine : IValidationEngine
{
    private readonly IArtifactRetrievalService _artifactRetrieval;
    private readonly IDocumentVerificationService _verification;

    /// <summary>Creates a new <see cref="ValidationEngine"/>.</summary>
    public ValidationEngine(
        IArtifactRetrievalService artifactRetrieval,
        IDocumentVerificationService verification)
    {
        _artifactRetrieval = artifactRetrieval;
        _verification = verification;
    }

    /// <inheritdoc />
    public async Task<DocumentValidationResponse> ValidateAsync(
        UploadRequest request,
        TrackingContext tracking,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var stopwatch = Stopwatch.StartNew();
        var provider = request.Enrollment?.ProviderInformation ?? new ProviderInformation();
        var tenantId = request.Route?.TenantID;

        // Merge results by fileId so an attachment referenced more than once
        // collapses into a single failed document (mirroring the AI sample).
        var errorsByFileId = new Dictionary<string, List<ValidationError>>();
        var erroredFileIds = new HashSet<string>();

        foreach (var attachment in provider.Attachments)
        {
            try
            {
                var artifact = await _artifactRetrieval
                    .RetrieveByFileIdAsync(attachment.FileId, tenantId, cancellationToken)
                    .ConfigureAwait(false);

                var errors = _verification.Verify(attachment, provider, artifact);
                if (errors.Count == 0)
                {
                    continue;
                }

                if (!errorsByFileId.TryGetValue(attachment.FileId, out var existing))
                {
                    existing = new List<ValidationError>();
                    errorsByFileId[attachment.FileId] = existing;
                }

                foreach (var error in errors)
                {
                    var duplicate = existing.Exists(e =>
                        e.Code == error.Code && e.Entered == error.Entered && e.Extracted == error.Extracted);
                    if (!duplicate)
                    {
                        existing.Add(error);
                    }
                }
            }
            catch (KeyNotFoundException)
            {
                // Artifact could not be retrieved: count as an errored document.
                erroredFileIds.Add(attachment.FileId);
            }
        }

        stopwatch.Stop();

        var failedDocs = errorsByFileId
            .Select(kvp => new FailedDoc { FileId = kvp.Key, ValidationErrors = kvp.Value })
            .ToList();

        var distinctIds = provider.Attachments.Select(a => a.FileId).Distinct().ToList();
        var failedCount = errorsByFileId.Count;
        var errorCount = erroredFileIds.Count(id => !errorsByFileId.ContainsKey(id));
        var passedCount = distinctIds.Count(id => !errorsByFileId.ContainsKey(id) && !erroredFileIds.Contains(id));
        var overallPass = failedCount == 0 && errorCount == 0;

        return new DocumentValidationResponse
        {
            Route = request.Route ?? new Route(),
            FailedDocs = failedDocs,
            ResultsSummary = new ResultsSummary
            {
                Code = (overallPass ? ResponseCode.ValidationPassed : ResponseCode.ValidationFailed).ToCode(),
                Status = overallPass ? "PASS" : "FAIL",
                Timestamp = DateTime.UtcNow,
                ProcessingTimeMs = stopwatch.Elapsed.TotalMilliseconds,
                TotalDocs = provider.Attachments.Count,
                PassedDocs = passedCount,
                FailedDocs = failedCount,
                ErrorDocs = errorCount
            }
        };
    }
}
