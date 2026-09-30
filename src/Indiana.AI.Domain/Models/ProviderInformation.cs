using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Indiana.AI.Domain.Models;

/// <summary>
/// Full provider information block (schema <c>DocProviderInformation</c>)
/// carried by the upload and validate payloads.
/// </summary>
public class ProviderInformation
{
    /// <summary>National Provider Identifier.</summary>
    [JsonPropertyName("npi")]
    public string Npi { get; set; } = string.Empty;

    /// <summary>Social security number / tax id.</summary>
    [JsonPropertyName("ssn")]
    public string? Ssn { get; set; }

    /// <summary>Provider id (optional).</summary>
    [JsonPropertyName("providerID")]
    public string? ProviderID { get; set; }

    /// <summary>Business / legal entity information.</summary>
    [JsonPropertyName("businessInformation")]
    public BusinessInformation BusinessInformation { get; set; } = new();

    /// <summary>Individual (person) information.</summary>
    [JsonPropertyName("individualInformation")]
    public IndividualInformation IndividualInformation { get; set; } = new();

    /// <summary>Specialties.</summary>
    [JsonPropertyName("specialties")]
    public List<Specialty> Specialties { get; set; } = new();

    /// <summary>CLIA certificate details.</summary>
    [JsonPropertyName("cliaDetails")]
    public List<CliaDetail> CliaDetails { get; set; } = new();

    /// <summary>License details.</summary>
    [JsonPropertyName("licenseDetails")]
    public List<LicenseDetail> LicenseDetails { get; set; } = new();

    /// <summary>DEA registration details.</summary>
    [JsonPropertyName("deaDetails")]
    public List<DeaDetail> DeaDetails { get; set; } = new();

    /// <summary>Provider enrollment certifications.</summary>
    [JsonPropertyName("peCertifications")]
    public List<PeCertification> PeCertifications { get; set; } = new();

    /// <summary>Accreditation information.</summary>
    [JsonPropertyName("accreditationInfo")]
    public List<object> AccreditationInfo { get; set; } = new();

    /// <summary>Malpractice insurance details.</summary>
    [JsonPropertyName("malPracticeInsurance")]
    public List<MalpracticeInsurance> MalPracticeInsurance { get; set; } = new();

    /// <summary>Medicaid program participations.</summary>
    [JsonPropertyName("medicaidProgramParticipations")]
    public List<object> MedicaidProgramParticipations { get; set; } = new();

    /// <summary>Addresses.</summary>
    [JsonPropertyName("addresses")]
    public List<AddressWrapper> Addresses { get; set; } = new();

    /// <summary>Document attachments.</summary>
    [JsonPropertyName("attachments")]
    public List<Attachment> Attachments { get; set; } = new();

    /// <summary>DME surety bonds.</summary>
    [JsonPropertyName("dmeSuretyBond")]
    public List<DmeSuretyBond> DmeSuretyBond { get; set; } = new();

    /// <summary>Education details.</summary>
    [JsonPropertyName("educationDetails")]
    public List<EducationDetail> EducationDetails { get; set; } = new();

    /// <summary>Payment details.</summary>
    [JsonPropertyName("paymentDetails")]
    public List<PaymentDetail> PaymentDetails { get; set; } = new();

    /// <summary>EFT banking details.</summary>
    [JsonPropertyName("eftDetails")]
    public List<EftDetail> EftDetails { get; set; } = new();

    /// <summary>Swing bed details (free-form object).</summary>
    [JsonPropertyName("swingBedDetails")]
    public object? SwingBedDetails { get; set; }

    /// <summary>Backdate request.</summary>
    [JsonPropertyName("backdateRequest")]
    public BackdateRequest? BackdateRequest { get; set; }
}
