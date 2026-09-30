using System.Text.Json.Serialization;

namespace Indiana.AI.Domain.Models;

/// <summary>Wraps a <see cref="FlatAddress"/> with its type and primary flag.</summary>
public class AddressWrapper
{
    /// <summary>Address type, for example "S".</summary>
    [JsonPropertyName("addressType")]
    public string? AddressType { get; set; }

    /// <summary>Whether this is the primary practice location.</summary>
    [JsonPropertyName("isPrimaryLocation")]
    public bool IsPrimaryLocation { get; set; }

    /// <summary>The address.</summary>
    [JsonPropertyName("address")]
    public FlatAddress Address { get; set; } = new();
}
