using System.Text.Json.Serialization;

namespace Indiana.AI.Domain.Models;

/// <summary>Electronic funds transfer (EFT) banking detail.</summary>
public class EftDetail
{
    /// <summary>Account holder name.</summary>
    [JsonPropertyName("accountHolderName")]
    public string? AccountHolderName { get; set; }

    /// <summary>Bank routing number.</summary>
    [JsonPropertyName("routingNumber")]
    public string? RoutingNumber { get; set; }

    /// <summary>Bank account number.</summary>
    [JsonPropertyName("accountNumber")]
    public string? AccountNumber { get; set; }
}
