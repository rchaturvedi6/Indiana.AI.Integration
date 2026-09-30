using System;
using System.Collections.Generic;
using System.Linq;
using Indiana.AI.Application.Abstractions;
using Indiana.AI.Domain.Enums;
using Indiana.AI.Domain.Models;

namespace Indiana.AI.Infrastructure.Adapters;

/// <summary>
/// Mocked AI Document Verification Service. Compares the entered enrollment data
/// against a seeded <see cref="ExtractionProfile"/> for the document and emits
/// the corresponding field-level validation error codes, mirroring the AI
/// sample validation response.
/// </summary>
public class MockDocumentVerificationService : IDocumentVerificationService
{
    private readonly MockExtractionDataset _dataset;

    /// <summary>Creates a new <see cref="MockDocumentVerificationService"/>.</summary>
    public MockDocumentVerificationService(MockExtractionDataset dataset) => _dataset = dataset;

    /// <inheritdoc />
    public List<ValidationError> Verify(Attachment attachment, ProviderInformation provider, EdmsQueryResult artifact)
    {
        ArgumentNullException.ThrowIfNull(attachment);
        ArgumentNullException.ThrowIfNull(provider);

        var errors = new List<ValidationError>();

        var profile = _dataset.GetProfile(attachment.FileId);
        if (profile is null)
        {
            // No seeded mismatches → the document passes.
            return errors;
        }

        var clia = provider.CliaDetails.FirstOrDefault();
        var license = provider.LicenseDetails.FirstOrDefault();

        AddIfMismatch(errors, ValidationErrorCode.AttachmentTypeMismatch,
            attachment.AttachmentTypeCode, profile.ExtractedAttachmentTypeName);

        AddIfMismatch(errors, ValidationErrorCode.NameMismatch,
            provider.BusinessInformation?.LegalName, profile.ExtractedLegalName);

        AddIfMismatch(errors, ValidationErrorCode.AddressMismatch,
            FormatPrimaryAddress(provider), profile.ExtractedAddress);

        AddIfMismatch(errors, ValidationErrorCode.CliaNumberMismatch,
            clia?.CliaNumber, profile.ExtractedCliaNumber);

        AddIfMismatch(errors, ValidationErrorCode.TaxIdMismatch,
            provider.Ssn, profile.ExtractedTaxId);

        AddIfMismatch(errors, ValidationErrorCode.LicenseNumberMismatch,
            license?.LicenseNumber, profile.ExtractedLicenseNumber);

        AddIfMismatch(errors, ValidationErrorCode.EffectiveDateMismatch,
            ToDate(clia?.EffectiveDate), profile.ExtractedEffectiveDate);

        AddIfMismatch(errors, ValidationErrorCode.ExpirationDateMismatch,
            ToDate(license?.ExpirationDate), profile.ExtractedEndDate);

        AddIfMismatch(errors, ValidationErrorCode.IssueDateMismatch,
            ToDate(provider.BackdateRequest?.IssueDate), profile.ExtractedIssueDate);

        return errors;
    }

    private static void AddIfMismatch(List<ValidationError> errors, string code, string? entered, string? extracted)
    {
        // Only evaluate a field the AI actually contested (extracted provided).
        if (extracted is null)
        {
            return;
        }

        if (!string.Equals((entered ?? string.Empty).Trim(), extracted.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            errors.Add(new ValidationError { Code = code, Entered = entered ?? string.Empty, Extracted = extracted });
        }
    }

    private static string FormatPrimaryAddress(ProviderInformation provider)
    {
        var wrapper = provider.Addresses.FirstOrDefault(a => a.IsPrimaryLocation)
                      ?? provider.Addresses.FirstOrDefault();
        var a = wrapper?.Address;
        if (a is null)
        {
            return string.Empty;
        }

        var postal = a.PostalCode is { Length: >= 5 } ? a.PostalCode[..5] : a.PostalCode;
        return string.Join(", ", new[] { a.Line1, a.City, a.StateId, postal }
            .Where(s => !string.IsNullOrWhiteSpace(s)));
    }

    private static string? ToDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        return DateTime.TryParse(value, out var parsed)
            ? parsed.ToString("yyyy-MM-dd")
            : value;
    }
}
