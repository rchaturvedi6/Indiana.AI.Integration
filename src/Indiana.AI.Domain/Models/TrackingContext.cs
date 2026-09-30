using System;

namespace Indiana.AI.Domain.Models;

/// <summary>
/// Cross-cutting correlation metadata threaded through mapping, routing,
/// validation and persistence so a single request can be traced end-to-end.
/// Derived from the inbound <see cref="Route.RequestGUID"/> and
/// <see cref="Enrollment.TrackingID"/>.
/// </summary>
public class TrackingContext
{
    /// <summary>Enrollment tracking identifier.</summary>
    public string TrackingId { get; set; } = string.Empty;

    /// <summary>Request correlation GUID.</summary>
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>Tenant identifier.</summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>Calling application client.</summary>
    public string AppClient { get; set; } = string.Empty;

    /// <summary>Timestamp (UTC) the gateway received the request.</summary>
    public DateTime ReceivedUtc { get; set; } = DateTime.UtcNow;
}
