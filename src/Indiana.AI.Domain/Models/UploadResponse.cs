using System;
using System.Text.Json.Serialization;

namespace Indiana.AI.Domain.Models;

/// <summary>Acknowledgement status block returned by the Upload API (API 1).</summary>
public class AckStatus
{
    /// <summary>Wire response code, for example "1002".</summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>Status, "PASS" or "FAIL".</summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    /// <summary>Server-side processing time in milliseconds.</summary>
    [JsonPropertyName("processingTimeMs")]
    public double ProcessingTimeMs { get; set; }

    /// <summary>Timestamp the acknowledgement was produced.</summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// The upload response contract (schema <c>AckResponse</c>) returned by
/// <c>POST /api/document-upload</c>. Echoes the request <see cref="Route"/> and
/// carries the <see cref="AckStatus"/>.
/// </summary>
public class UploadResponse
{
    /// <summary>Echoed caller / request context.</summary>
    [JsonPropertyName("route")]
    public Route Route { get; set; } = new();

    /// <summary>Acknowledgement status.</summary>
    [JsonPropertyName("ack_status")]
    public AckStatus AckStatus { get; set; } = new();
}
