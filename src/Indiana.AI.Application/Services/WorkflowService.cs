using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Indiana.AI.Application.Abstractions;
using Indiana.AI.Domain.Enums;
using Indiana.AI.Domain.Models;

namespace Indiana.AI.Application.Services;

/// <summary>
/// Default <see cref="IWorkflowService"/> implementation. Delegates upload and
/// validation to the (swappable) <see cref="IAiPlatformClient"/>, persists the
/// verification results locally and exposes artifact retrieval and result
/// lookups to the API layer.
/// </summary>
public class WorkflowService : IWorkflowService
{
    private readonly IAiPlatformClient _aiPlatform;
    private readonly IAIPayloadMapper _mapper;
    private readonly IArtifactRetrievalService _artifactRetrieval;
    private readonly IVerificationResultRepository _repository;

    /// <summary>Creates a new <see cref="WorkflowService"/>.</summary>
    public WorkflowService(
        IAiPlatformClient aiPlatform,
        IAIPayloadMapper mapper,
        IArtifactRetrievalService artifactRetrieval,
        IVerificationResultRepository repository)
    {
        _aiPlatform = aiPlatform;
        _mapper = mapper;
        _artifactRetrieval = artifactRetrieval;
        _repository = repository;
    }

    /// <inheritdoc />
    public Task<UploadResponse> ProcessUploadAsync(UploadRequest request, CancellationToken cancellationToken = default) =>
        _aiPlatform.UploadAsync(request, cancellationToken);

    /// <inheritdoc />
    public async Task<DocumentValidationResponse> ValidateAsync(UploadRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var response = await _aiPlatform.ValidateAsync(request, cancellationToken).ConfigureAwait(false);

        var tracking = _mapper.CreateTrackingContext(request);
        var failedByFileId = response.FailedDocs
            .GroupBy(d => d.FileId)
            .ToDictionary(g => g.Key, g => g.First());
        var provider = request.Enrollment?.ProviderInformation ?? new ProviderInformation();

        foreach (var attachment in provider.Attachments.GroupBy(a => a.FileId).Select(g => g.First()))
        {
            var failed = failedByFileId.TryGetValue(attachment.FileId, out var failedDoc);

            var result = new VerificationResult
            {
                FileId = attachment.FileId,
                TrackingId = tracking.TrackingId,
                CorrelationId = tracking.CorrelationId,
                Status = failed ? VerificationStatus.Fail : VerificationStatus.Pass,
                Code = (failed ? ResponseCode.ValidationFailed : ResponseCode.ValidationPassed).ToCode(),
                ValidationErrors = failed ? failedDoc!.ValidationErrors : new List<ValidationError>()
            };

            await _repository.SaveAsync(result, cancellationToken).ConfigureAwait(false);
        }

        return response;
    }

    /// <inheritdoc />
    public Task<GetEdmsFileResponse> RetrieveArtifactAsync(GetEdmsFileRequest request, CancellationToken cancellationToken = default) =>
        _artifactRetrieval.RetrieveAsync(request, cancellationToken);

    /// <inheritdoc />
    public Task<VerificationResult?> GetVerificationResultAsync(string fileId, CancellationToken cancellationToken = default) =>
        _repository.GetByFileIdAsync(fileId, cancellationToken);

    /// <inheritdoc />
    public Task<IReadOnlyList<VerificationResult>> GetAllVerificationResultsAsync(CancellationToken cancellationToken = default) =>
        _repository.GetAllAsync(cancellationToken);
}
