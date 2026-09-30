using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Indiana.AI.Application.Abstractions;
using Indiana.AI.Domain.Enums;
using Indiana.AI.Domain.Models;

namespace Indiana.AI.Application.Services;

/// <summary>
/// In-process (mock) implementation of <see cref="IAiPlatformClient"/>. Simulates
/// the AI Verification Service platform locally: acknowledges uploads and runs
/// the local <see cref="IValidationEngine"/> (which uses the mock FMS and mock
/// AI verification). This is the default implementation.
/// </summary>
public class LocalAiPlatformClient : IAiPlatformClient
{
    private readonly IAIPayloadMapper _mapper;
    private readonly IRouter _router;
    private readonly IValidationEngine _validationEngine;

    /// <summary>Creates a new <see cref="LocalAiPlatformClient"/>.</summary>
    public LocalAiPlatformClient(IAIPayloadMapper mapper, IRouter router, IValidationEngine validationEngine)
    {
        _mapper = mapper;
        _router = router;
        _validationEngine = validationEngine;
    }

    /// <inheritdoc />
    public Task<UploadResponse> UploadAsync(UploadRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var stopwatch = Stopwatch.StartNew();
        var tracking = _mapper.CreateTrackingContext(request);

        // Map to the canonical AI payload and confirm routing resolves for the tenant.
        _ = _mapper.Map(request, tracking);
        _ = _router.ResolveAdapterName(request.Route?.TenantID);

        stopwatch.Stop();

        var response = new UploadResponse
        {
            Route = request.Route ?? new Route(),
            AckStatus = new AckStatus
            {
                Code = ResponseCode.UploadAccepted.ToCode(),
                Status = "PASS",
                ProcessingTimeMs = stopwatch.Elapsed.TotalMilliseconds,
                Timestamp = DateTime.UtcNow
            }
        };

        return Task.FromResult(response);
    }

    /// <inheritdoc />
    public Task<DocumentValidationResponse> ValidateAsync(UploadRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tracking = _mapper.CreateTrackingContext(request);
        return _validationEngine.ValidateAsync(request, tracking, cancellationToken);
    }
}
