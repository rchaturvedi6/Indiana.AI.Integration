using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Indiana.AI.Domain.Models;

/// <summary>Overall result summary block for a validation run (API 3).</summary>
public class ResultsSummary
{
    /// <summary>Wire response code, for example "2001".</summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>Status, "PASS" or "FAIL".</summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    /// <summary>Timestamp the summary was produced.</summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>Server-side processing time in milliseconds.</summary>
    [JsonPropertyName("processingTimeMs")]
    public double ProcessingTimeMs { get; set; }

    /// <summary>Total number of documents evaluated.</summary>
    [JsonPropertyName("totalDocs")]
    public int TotalDocs { get; set; }

    /// <summary>Number of documents that passed.</summary>
    [JsonPropertyName("passedDocs")]
    public int PassedDocs { get; set; }

    /// <summary>Number of documents that failed.</summary>
    [JsonPropertyName("failedDocs")]
    public int FailedDocs { get; set; }

    /// <summary>Number of documents that errored.</summary>
    [JsonPropertyName("errorDocs")]
    public int ErrorDocs { get; set; }
}

/// <summary>
/// The validation response contract (schema <c>DocValidateResponse</c>)
/// returned by <c>POST /api/document-validate</c> (API 3).
/// </summary>
public class DocumentValidationResponse
{
    /// <summary>Echoed caller / request context.</summary>
    [JsonPropertyName("route")]
    public Route Route { get; set; } = new();

    /// <summary>Documents that failed validation.</summary>
    [JsonPropertyName("failedDocs")]
    public List<FailedDoc> FailedDocs { get; set; } = new();

    /// <summary>Overall result summary.</summary>
    [JsonPropertyName("resultsSummary")]
    public ResultsSummary ResultsSummary { get; set; } = new();
}
