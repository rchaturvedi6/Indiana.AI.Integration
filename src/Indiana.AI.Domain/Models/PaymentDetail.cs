using System.Text.Json.Serialization;

namespace Indiana.AI.Domain.Models;

/// <summary>Enrollment payment detail.</summary>
public class PaymentDetail
{
    /// <summary>Payment amount.</summary>
    [JsonPropertyName("amount")]
    public double Amount { get; set; }

    /// <summary>Payment date.</summary>
    [JsonPropertyName("paymentDate")]
    public string? PaymentDate { get; set; }

    /// <summary>Transaction id (optional).</summary>
    [JsonPropertyName("transactionId")]
    public string? TransactionId { get; set; }
}
