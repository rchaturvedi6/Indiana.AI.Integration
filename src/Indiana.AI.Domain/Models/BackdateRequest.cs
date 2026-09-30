using System.Text.Json.Serialization;

namespace Indiana.AI.Domain.Models;

/// <summary>Backdate request detail.</summary>
public class BackdateRequest
{
    /// <summary>Requested issue date.</summary>
    [JsonPropertyName("issueDate")]
    public string? IssueDate { get; set; }
}
