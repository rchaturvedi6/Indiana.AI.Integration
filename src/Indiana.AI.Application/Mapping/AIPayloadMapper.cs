using System;
using System.Linq;
using Indiana.AI.Application.Abstractions;
using Indiana.AI.Domain.Models;

namespace Indiana.AI.Application.Mapping;

/// <summary>
/// Default implementation of <see cref="IAIPayloadMapper"/>. Derives the
/// correlation context from the inbound route/enrollment and projects the
/// Indiana enrollment payload onto the canonical AI verification payload.
/// </summary>
public class AIPayloadMapper : IAIPayloadMapper
{
    /// <inheritdoc />
    public TrackingContext CreateTrackingContext(UploadRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var route = request.Route ?? new Route();
        var enrollment = request.Enrollment ?? new Enrollment();

        return new TrackingContext
        {
            TrackingId = string.IsNullOrWhiteSpace(enrollment.TrackingID)
                ? $"TRK-{Guid.NewGuid():N}"
                : enrollment.TrackingID,
            CorrelationId = string.IsNullOrWhiteSpace(route.RequestGUID)
                ? $"COR-{Guid.NewGuid():N}"
                : route.RequestGUID,
            TenantId = route.TenantID,
            AppClient = route.AppClient,
            ReceivedUtc = DateTime.UtcNow
        };
    }

    /// <inheritdoc />
    public AIVerificationPayload Map(UploadRequest request, TrackingContext tracking)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(tracking);

        var provider = request.Enrollment?.ProviderInformation ?? new ProviderInformation();

        return new AIVerificationPayload
        {
            TrackingId = tracking.TrackingId,
            CorrelationId = tracking.CorrelationId,
            TenantId = tracking.TenantId,
            ProviderNpi = provider.Npi,
            ProviderLegalName = provider.BusinessInformation?.LegalName ?? string.Empty,
            Documents = provider.Attachments
                .Select(a => new AIDocument
                {
                    FileId = a.FileId,
                    FileName = a.FileName,
                    AttachmentTypeCode = a.AttachmentTypeCode,
                    AttachmentTypeName = a.AttachmentTypeName
                })
                .ToList(),
            CreatedUtc = DateTime.UtcNow
        };
    }
}
