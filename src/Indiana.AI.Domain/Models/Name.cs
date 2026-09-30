using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Indiana.AI.Domain.Models;

/// <summary>A person name.</summary>
public class Name
{
    /// <summary>Title (optional).</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>First name.</summary>
    [JsonPropertyName("first")]
    public string First { get; set; } = string.Empty;

    /// <summary>Middle name (optional).</summary>
    [JsonPropertyName("middle")]
    public string? Middle { get; set; }

    /// <summary>Last name.</summary>
    [JsonPropertyName("last")]
    public string Last { get; set; } = string.Empty;

    /// <summary>Suffix (optional).</summary>
    [JsonPropertyName("suffix")]
    public string? Suffix { get; set; }

    /// <summary>Additional family names.</summary>
    [JsonPropertyName("familyNames")]
    public List<string> FamilyNames { get; set; } = new();
}
