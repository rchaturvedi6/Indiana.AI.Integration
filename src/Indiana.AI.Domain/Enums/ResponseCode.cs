namespace Indiana.AI.Domain.Enums;

/// <summary>
/// Canonical acknowledgement / results response codes used by the AI
/// Verification Service (APIs 1 and 3). The integer value is the wire code.
/// </summary>
public enum ResponseCode
{
    /// <summary>Upload accepted acknowledgement (API 1).</summary>
    UploadAccepted = 1002,

    /// <summary>Validation completed and every document passed (API 3).</summary>
    ValidationPassed = 2000,

    /// <summary>Validation completed but one or more documents failed (API 3).</summary>
    ValidationFailed = 2001
}

/// <summary>
/// Convenience helpers for translating <see cref="ResponseCode"/> values to
/// their string wire representation.
/// </summary>
public static class ResponseCodeExtensions
{
    /// <summary>Returns the numeric wire code as a string (for example "1002").</summary>
    public static string ToCode(this ResponseCode code) => ((int)code).ToString();
}

/// <summary>
/// Field-level validation error codes emitted by the AI Verification Service in
/// the <c>validation_errors</c> array (API 3). Values are the observed wire
/// codes from the AI sample payloads.
/// </summary>
public static class ValidationErrorCode
{
    /// <summary>3001 — attachment type mismatch.</summary>
    public const string AttachmentTypeMismatch = "3001";

    /// <summary>3101 — name / legal name mismatch.</summary>
    public const string NameMismatch = "3101";

    /// <summary>3109 — address mismatch.</summary>
    public const string AddressMismatch = "3109";

    /// <summary>3116 — CLIA number mismatch.</summary>
    public const string CliaNumberMismatch = "3116";

    /// <summary>3120 — tax id mismatch.</summary>
    public const string TaxIdMismatch = "3120";

    /// <summary>3123 — license number mismatch.</summary>
    public const string LicenseNumberMismatch = "3123";

    /// <summary>3127 — effective date mismatch.</summary>
    public const string EffectiveDateMismatch = "3127";

    /// <summary>3129 — expiration / end date mismatch.</summary>
    public const string ExpirationDateMismatch = "3129";

    /// <summary>3133 — generic field mismatch.</summary>
    public const string GenericFieldMismatch = "3133";

    /// <summary>3150 — issue date mismatch.</summary>
    public const string IssueDateMismatch = "3150";
}
