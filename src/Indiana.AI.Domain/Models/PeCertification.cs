using System.Text.Json.Serialization;

namespace Indiana.AI.Domain.Models;

/// <summary>Provider enrollment certification detail.</summary>
public class PeCertification
{
    /// <summary>Effective date.</summary>
    [JsonPropertyName("effectiveDate")]
    public string? EffectiveDate { get; set; }

    /// <summary>Expiration date.</summary>
    [JsonPropertyName("expirationDate")]
    public string? ExpirationDate { get; set; }

    /// <summary>Specialty code.</summary>
    [JsonPropertyName("specialtyCode")]
    public string? SpecialtyCode { get; set; }

    /// <summary>Specialty name.</summary>
    [JsonPropertyName("specialtyName")]
    public string? SpecialtyName { get; set; }

    /// <summary>Free-text certification description.</summary>
    [JsonPropertyName("othersCertificationText")]
    public string? OthersCertificationText { get; set; }

    /// <summary>Certification number.</summary>
    [JsonPropertyName("certificationNumber")]
    public string? CertificationNumber { get; set; }

    /// <summary>Whether the provider is exempt from accreditation ("True"/"False").</summary>
    [JsonPropertyName("isExemptfromAccreditation")]
    public string? IsExemptfromAccreditation { get; set; }
}
