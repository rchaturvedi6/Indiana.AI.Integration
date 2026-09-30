using Indiana.AI.Domain.Models;

namespace Indiana.AI.Application.Abstractions;

/// <summary>
/// Converts an Indiana enrollment <see cref="UploadRequest"/> into the
/// canonical <see cref="AIVerificationPayload"/> consumed by the AI platform,
/// and mints the correlation identifiers for the request.
/// </summary>
public interface IAIPayloadMapper
{
    /// <summary>
    /// Creates a fresh <see cref="TrackingContext"/> (tracking id + correlation
    /// id) for an inbound upload.
    /// </summary>
    TrackingContext CreateTrackingContext(UploadRequest request);

    /// <summary>
    /// Maps an Indiana enrollment payload to the canonical AI payload using the
    /// supplied tracking context.
    /// </summary>
    AIVerificationPayload Map(UploadRequest request, TrackingContext tracking);
}
